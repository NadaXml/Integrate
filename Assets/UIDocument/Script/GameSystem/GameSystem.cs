using AppFrame;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.Core.ADT;
using UIDocument.Script.Core.Config;
using UIDocument.Script.EventService;
using UnityEngine;
using UnityEngine.Assertions;
using YooAsset;
namespace UIDocument.Script.GameSystem {
    public class GameSystem : ISystem {

        public void Update(float deltaTime) {
            // throw new System.NotImplementedException();
        }
        public void Awake() {
            _context.EventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_AnalyticsDmg, OnAnalyticsDmg);
            _context.EventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_AnalyticsMoveCount, OnAnalyticsMoveCount);
            _context.EventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_OnRoundOver, OnRoundOver);
            _context.EventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_Retry, OnRetry);
        }
        public void Destroy() {
            _context.EventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_AnalyticsDmg, OnAnalyticsDmg);
            _context.EventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_AnalyticsMoveCount, OnAnalyticsMoveCount);
            _context.EventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_OnRoundOver, OnRoundOver);
            _context.EventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_Retry, OnRetry);
            DestroyGame();
        }

        public IEnumerator Start() {
            // throw new System.NotImplementedException();
            yield return null;
        }

        public struct Context {
            public EventServiceProvider EventServiceProvider;
            public AssetService.AssetService AssetService;
        }

        Context _context;
        AssetHandle _battleConfig;
        
        ulong _actorId;
        ulong AllocateActorSequenceId() {
            #if DEBUG
                Assert.IsTrue(_actorId < ulong.MaxValue);
            #endif
            return _actorId++;
        }
        
        List<Actor> _actors = new List<Actor>();
        
        // 回合分析
        Analysis _analysis;
        
        public GameSystem(in Context context) {
            _context = context;
        }

        void OnAnalyticsDmg(object sender, GameEventBase e) {
            AnalyticsDmgEvent evt = e as AnalyticsDmgEvent;
            AddBattleComponentToAnalysis(evt.battleComponent);
        }
        
        void OnAnalyticsMoveCount(object sender, GameEventBase e) {
            AnalyticsMoveCountEvent evt = e as AnalyticsMoveCountEvent;
            AddMoveComponentToAnalysis(evt.moveComponent);
        }
        
        void OnRoundOver(object sender, GameEventBase e) {
            WriteMoveComponentAnalysis();
        }
        
        void OnRetry(object sender, GameEventBase e) {
            QueryRoundStatusEvent evt = new QueryRoundStatusEvent();
            _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_QueryRoundStatus, evt);
            if (evt.status != RoundStatus.None) {
                return;
            }
            
            DestroyGame();
            CreateGame().Forget();
        }
        
        public async UniTask CreateGame() {
            AssetHandle handle = _context.AssetService.LoadAssetAsync<ScriptableObject>("BattleConfig");
            await handle.ToUniTask();
            
            _battleConfig = handle;
            _actorId = 0;
            _analysis = ScriptableObject.CreateInstance<Analysis>();
            
            CreateActor();
            StartGame();
        }
        void StartGame() {
            _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_StartGame, null);
        }
        

        void CreateActor() {
            BattleConfig config = _battleConfig.AssetObject as BattleConfig;
            MoveComponent[] moveComponents = new MoveComponent[config.actorConfig.Count];
            BattleComponent[] battleComponents = new BattleComponent[config.actorConfig.Count];
            int count = 0;
            foreach (var actorConfig in config.actorConfig) {
                Actor actor = new Actor(new Actor.ActorContext() {
                    eventServiceProvider = _context.EventServiceProvider,
                });
                actor.sequenceId = AllocateActorSequenceId();
                
                MoveComponent moveComponent = MoveComponent.FromConfig(actorConfig.moveComponentConfig);
                moveComponent.sequenceId = actor.AllocateComponentId();
                moveComponent.actorSequenceId = actor.sequenceId;
                moveComponents[count] = moveComponent;
                actor.moveComponentRef = moveComponent.sequenceId;

                BattleComponent battleComponent = BattleComponent.FromConfig(actorConfig.battleComponentConfig);
                battleComponent.sequenceId = actor.AllocateComponentId();
                battleComponent.actorSequenceId = actor.sequenceId;
                battleComponents[count] = battleComponent;
                actor.battleComponentRef = battleComponent.sequenceId;

                count++;
                _actors.Add(actor);
            }
            
            CreateMoveEvent crateMoveEvt = new CreateMoveEvent();
            crateMoveEvt.components = moveComponents;
            _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_CreateMove, crateMoveEvt);
            
            CreateRoundEvent createRoundEvt = new CreateRoundEvent();
            createRoundEvt.roundConfig = config.roundConfig;
            _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_CreateRound, createRoundEvt);

            CreateBattleEvent createBattleEvt = new CreateBattleEvent();
            createBattleEvt.components = battleComponents;
            _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_CreateBattle, createBattleEvt);
        }
        
        public void DestroyGame() {
            _actors.Clear();
            _battleConfig.Release();
            _context.AssetService.UnloadAsset("BattleConfig");
        }
        
        void AddMoveComponentToAnalysis(in MoveComponent moveComponent) {
            if (_analysis == null) {
                _analysis = ScriptableObject.CreateInstance<Analysis>();
            }

            ulong actorSequenceId = moveComponent.actorSequenceId;
            var counter = _analysis.counter.Find((x) => x.actorSequence == actorSequenceId);
            if (counter != null) {
                counter.moveCount++;
            }
            else {
                _analysis.counter.Add(new ActionCounter() {
                    actorSequence = actorSequenceId,
                    position = moveComponent.position,
                    moveCount = 1,
                });
            }
        }

        void AddBattleComponentToAnalysis(in BattleComponent battleComponent) {
            if (_analysis == null) {
                _analysis = ScriptableObject.CreateInstance<Analysis>();
            }
            
            ulong actorSequence = battleComponent.actorSequenceId;
            var counter = _analysis.counter.Find((x) => x.actorSequence == actorSequence);
            if (counter != null) {
                counter.totalDmg += battleComponent.atk.value;
            }
            else {
                _analysis.counter.Add(new ActionCounter() {
                    actorSequence = actorSequence,
                    totalDmg = battleComponent.atk.value
                });
            }
        }

        void WriteMoveComponentAnalysis() {
            #if UNITY_EDITOR
            if (_analysis != null) {
                string path = "Assets/UIDocument/analysis.asset";
                UnityEditor.AssetDatabase.CreateAsset(_analysis, path);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            #endif
        }
    }
}
