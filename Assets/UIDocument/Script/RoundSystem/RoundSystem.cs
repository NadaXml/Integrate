using AppFrame;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UIDocument.Script.Core.ADT;
using UIDocument.Script.Core.Config;
using UIDocument.Script.EventService;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
namespace UIDocument.Script.RoundSystem {
    public class RoundSystem : ISystem {
        
        public void Awake() {
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_DumpRound, OnDumpRound);
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_DumpRoundInspector, OnDumpInspector);
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_CreateRound, OnCreateRound);
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_CreateMove, OnCreateMove);
            _eventServiceProvider.GetEventService().RegisterEvent(EventNameDef.N_StartGame, OnStartGame);
        }

        public void Destroy() {
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_DumpRound, OnDumpRound);
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_DumpRoundInspector, OnDumpInspector);
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_CreateRound, OnCreateRound);
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_CreateMove, OnCreateMove);
            _eventServiceProvider.GetEventService().UnRegisterEvent(EventNameDef.N_StartGame, OnStartGame);
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
        
        // 回合限定次数
        int _needTurn;
        
        #endregion

        
        void OnCreateRound(object sender, GameEventBase e) {
            CreateRoundEvent evt = e as CreateRoundEvent;
            CreateRound(evt.roundConfig);
        }

        void CreateRound(in RoundConfig roundConfig) {
            // 创建轮次
            var maxTurn = roundConfig.maxTurn;
            _turns = new NativeArray<Turn>(maxTurn, Allocator.Persistent);
            for (int i = 0; i < maxTurn; i++) {
                int turnActionValue = i == 0 ? roundConfig.firstActionValue.value : roundConfig.perActionValue.value;
                Turn turn = Turn.FromMaxActionValue(turnActionValue);
                _turns[i] = turn;
            }
            
            _roundContext.Status = RoundContext.RoundStatus.None;
            _timeRemainder = 0f;

            _frameRate = 1f/roundConfig.logicFrameRate;

            _needTurn = roundConfig.needTurn;
        }

        void OnCreateMove(object sender, GameEventBase e) {
            CreateMoveEvent evt = e as CreateMoveEvent;
            CreateMove(evt.components);
        }
        
        void CreateMove(in MoveComponent[] components) {
            _moveComponents = new NativeArray<MoveComponent>(components.Length, Allocator.Persistent);
            _moveComponents.CopyFrom(components);
            // 创建可移动者
            foreach (var move in _moveComponents) {
                _testSortedMove.Add(move);
            }
        }
        
        void OnStartGame(object sender, GameEventBase e) {
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
            
            DefaultEvent evt = new DefaultEvent();
            evt.EventId = EventNameDef.ID_OnRoundOver;
            _eventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_OnRoundOver, evt);
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
            
            OnRoundOptionEvent evt = new OnRoundOptionEvent();
            evt.component = moveComponent;
            _eventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_OnRoundOption, evt);
            
            // 暂时先这么写
            var evt2 = new AnalyticsMoveCountEvent();
            evt2.moveComponent = moveComponent;
            _eventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.N_AnalyticsMoveCount, evt2);
        }

        void OnDumpRound(object sender, GameEventBase args) {
            Dump();
        }
        
        void OnDumpInspector(object sender, GameEventBase e) {
            DumpRoundInspectorEvent evt = e as DumpRoundInspectorEvent;
            if (evt == null) {
                return;
            }
            MoveComponentStream stream = new MoveComponentStream();
            stream.MoveComponents = new List<MoveComponent>();
            for (int i = 0; i < _moveComponents.Length; i++) {
                stream.MoveComponents.Add(_moveComponents[i]);
            }
            evt.result = stream;
        }
    }
}
