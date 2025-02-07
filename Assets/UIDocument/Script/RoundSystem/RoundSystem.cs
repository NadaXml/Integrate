using AppFrame;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UIDocument.Script.EventService;
using UIDocument.Script.RoundSystem.Config;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
namespace UIDocument.Script.RoundSystem {
    public class RoundSystem : ISystem {
        
        public void Awake() {
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.DumpRound, OnDumpRound);
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.DumpRoundInspector, OnDumpInspector);
        }

        public void Destroy() {
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.DumpRound, OnDumpRound);
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.DumpRoundInspector, OnDumpInspector);
            _moveComponents.Dispose();
            _turns.Dispose();
        }
        public IEnumerator Start() {
            yield return null;
        }
        
        public void Update(float deltaTime) {
            
            if(_roundContext.Status != RoundContext.RoundStatus.Running) return;
            
            // 通过给定的帧率运行
            float loopTime = deltaTime + _timeRemainder;
            if (loopTime < _frameRate) {
                _timeRemainder += deltaTime;
            }
            else {
                while (loopTime >= _frameRate) {
                    Step();
                    loopTime -= _frameRate;
                    if (loopTime <= 0) {
                        _timeRemainder = loopTime + _frameRate;
                        break;
                    }
                    if(_roundContext.Status != RoundContext.RoundStatus.Running) break;
                }
                if (loopTime > 0) {
                    _timeRemainder += loopTime;
                }
            }
        }
        
        public struct CreateParam {
            public EventService.EventServiceProvider EventServiceProvider;
        }
        
        public RoundSystem(in CreateParam param) {
            _eventServiceProvider = param.EventServiceProvider;
            _roundContext.TurnIndex = 0;
            _roundContext.Status = RoundContext.RoundStatus.None;
        }
        
        #region including def
        
        public struct RoundContext : IDumpable {
            public enum RoundStatus {
                None        = 0,    // 初始状态
                Running     = 1,    // 运行中
                Animation   = 2,    // 回合事件表现阶段
                Option      = 3,    // 回合选择中
                Pause       = 4,    // 暂停表现
            }

            public RoundStatus Status;
            public int TurnIndex;
            
            public string Dump() {
                return $"Round Status : {Status} Round Turn : {TurnIndex} ";
            }
        }
        
        #endregion

        #region field


        /// <summary>
        /// 可移动者
        /// </summary>
        NativeArray<MoveComponent> _moveComponents;
        /// <summary>
        /// 所有轮次
        /// </summary>
        NativeArray<Turn> _turns;
        /// <summary>
        /// 回合运行数据
        /// </summary>
        RoundContext _roundContext = new RoundContext();
        
        /// <summary>
        /// 当期行动值产生的行动者
        /// </summary>
        Queue<MoveComponent> _actionMoves = new Queue<MoveComponent>();

        SortedSet<MoveComponent> _testSortedMove = new SortedSet<MoveComponent>();
        
        EventService.EventServiceProvider _eventServiceProvider;
        
        // 系统运行时间参数
        float _timeRemainder;
        float _frameRate;
        
        // 回合分析
        Analysis _analysis;
        
        // 回合限定次数
        int _needTurn;
        
        #endregion

        /// <summary>
        /// 创建回合
        /// </summary>
        /// <param name="config"></param>
        public void CreateSet(in BattleConfig config) {
            
            // 创建轮次
            var maxTurn = config.roundConfig.maxTurn;
            _turns = new NativeArray<Turn>(maxTurn, Allocator.Persistent);
            for (int i = 0; i < maxTurn; i++) {
                int turnActionValue = i == 0 ? config.roundConfig.firstActionValue.value : config.roundConfig.perActionValue.value;
                Turn turn = Turn.FromMaxActionValue(turnActionValue);
                _turns[i] = turn;
            }

            _moveComponents = new NativeArray<MoveComponent>(config.moveComponentConfig.Count, Allocator.Persistent);
            int index = 0;
            // 创建可移动者
            foreach (var moveConfig in config.moveComponentConfig) {
                var moveComponent = MoveComponent.FromConfig(moveConfig);
                _moveComponents[index++] = moveComponent;
                _testSortedMove.Add(moveComponent);
            }

            _roundContext.Status = RoundContext.RoundStatus.None;
            _timeRemainder = 0f;

            _frameRate = 1f/config.roundConfig.logicFrameRate;

            _needTurn = config.roundConfig.needTurn;
        }
        
        public void StartSet() {
            _roundContext.Status = RoundContext.RoundStatus.Running;
        }

        /// <summary>
        /// 向前行动值运行
        /// </summary>
        void Step() {
            // 校验回合合法
            int turnIndex = -1;
            for(int i = _roundContext.TurnIndex ; i < _turns.Length ; i++) {
                if (_turns[i].CheckForward()) {
                    turnIndex = i;
                    break;
                }
            }

            if (turnIndex == -1) {
                // 所有回合都已经结束了
                RoundOver();
                return;
            } else if (turnIndex >= _needTurn) {
                // 达到指定回合结束了
                RoundOver();
                return;
            }
            
            // 行动者消耗行动值
            for (int i = 0; i < _moveComponents.Length; i++) {
                MoveComponent component = _moveComponents[i];
                component.Forward();
                if (component.IsPass()) {
                    _actionMoves.Enqueue(component);
                }
                _moveComponents[i] = component;
            }

            unsafe {
                // 回合消耗行动值
                Turn* turn = (Turn*)_turns.GetUnsafePtr();
                turn[turnIndex].Forward();
                turn[turnIndex].Settle();
            }
 
            
            // 结算行动
            if (_actionMoves.Count > 0) {
                _roundContext.Status = RoundContext.RoundStatus.Option;
                int c = _actionMoves.Count;
                while (_roundContext.Status == RoundContext.RoundStatus.Option) {
                    HandleOption();
                    c--;
                    if (c < 0) break;
                }
            }
        }

        void HandleOption() {
            if (_roundContext.Status != RoundContext.RoundStatus.Option) {
                return;
            }
            MoveComponent component = _actionMoves.Dequeue();
            DoAction(ref component);

            unsafe {
                // 目前是认为瞬间执行，立刻重新开始回合
                int index = FindMoveComponentIndex(component.position);
                if (index > -1) {
                    MoveComponent* temp = (MoveComponent*)_moveComponents.GetUnsafePtr();
                    temp[index].Reset();
                }
            }
 
            
            if (_actionMoves.Count == 0) {
                _roundContext.Status = RoundContext.RoundStatus.Running;
            }
        }

        int FindMoveComponentIndex(int position) {
            for (int i = 0; i < _moveComponents.Length; i++) {
                if (_moveComponents[i].position == position) {
                    return i;
                }
            }
            return -1;
        }
        
        void RoundOver() {
            _roundContext.Status = RoundContext.RoundStatus.None;
            Dump();
            WriteMoveComponentAnalysis();
        }

        void Dump() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Round Context : {_roundContext.Dump()}");
            builder.AppendLine($"Round Total move : {DumpUtility.DumpList(_moveComponents)}");
            builder.AppendLine($"Round action move : {DumpUtility.DumpList(_actionMoves)}");
            builder.AppendLine($"Round total turn : {DumpUtility.DumpList(_turns)}");
            builder.AppendLine($"Round total test move : {DumpUtility.DumpList(_testSortedMove)}");
            Debug.Log(builder.ToString());
        }

        void DoAction(ref MoveComponent moveComponent) {
            Debug.Log($"do action {moveComponent.Dump()}");
            AddMoveComponentToAnalysis(moveComponent);
        }

        void OnDumpRound(object sender, GameEventBase args) {
            Dump();
        }
        
        void OnDumpInspector(object sender, GameEventBase e) {
            FetchRoundInspector evt = e as FetchRoundInspector;
            if (evt == null) {
                return;
            }
            MoveComponentStream stream = new MoveComponentStream();
            stream.MoveComponents = new List<MoveComponent>();
            for (int i = 0; i < _moveComponents.Length; i++) {
                stream.MoveComponents.Add(_moveComponents[i]);
            }
            evt.Result = JsonConvert.SerializeObject(stream);
        }

        void AddMoveComponentToAnalysis(in MoveComponent moveComponent) {
            if (_analysis == null) {
                _analysis = ScriptableObject.CreateInstance<Analysis>();
            }

            int position = moveComponent.position;
            var counter = _analysis.Counter.Find((x) => x.Position == position);
            if (counter != null) {
                counter.moveCount++;
                counter.totalDmg += moveComponent.dmg;
            }
            else {
                _analysis.Counter.Add(new ActionCounter() {
                    Position = position,
                    moveCount = 1,
                    totalDmg = moveComponent.dmg
                });
            }
        }

        void WriteMoveComponentAnalysis() {
            #if UNITY_EDITOR
            if (_analysis != null) {
                string path = "Assets/UIDocument/analysis.asset";
                AssetDatabase.CreateAsset(_analysis, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            #endif
        }
    }
}
