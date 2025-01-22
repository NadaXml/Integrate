using AppFrame;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UIDocument.Script.RoundSystem.ADT;
using UIDocument.Script.RoundSystem.Config;
using UnityEngine;
namespace UIDocument.Script.RoundSystem {
    public class RoundSystem : ISystem {

        public void Update(float deltaTime) {
            // 通过给定的帧率运行
            float loopTime = deltaTime + _timeRemainder;
            while (loopTime >= _frameRate) {
                
                Step();
                
                loopTime -= _frameRate;
                if (loopTime <= 0) {
                    _timeRemainder = loopTime + _frameRate;
                    break;
                }
            }
        }
        public void Awake() {
            
        }
        public void Destroy() {
            
        }
        public IEnumerator Start() {
            yield return null;
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
        List<MoveComponent> _moveComponents = new List<MoveComponent>(); 
        /// <summary>
        /// 所有轮次
        /// </summary>
        List<Turn> _turns = new List<Turn>();
        /// <summary>
        /// 回合运行数据
        /// </summary>
        RoundContext _roundContext = new RoundContext();
        
        /// <summary>
        /// 当期行动值产生的行动者
        /// </summary>
        Queue<MoveComponent> _actionMoves = new Queue<MoveComponent>();
        

        // 系统运行时间参数
        float _timeRemainder;
        float _frameRate;
        
        #endregion

        /// <summary>
        /// 创建回合
        /// </summary>
        /// <param name="config"></param>
        public void CreateSet(in BattleConfig config) {
            // 创建轮次
            var maxTurn = config.roundConfig.maxTurn;
            for (int i = 0; i < maxTurn; i++) {
                int turnActionValue = i == 0 ? config.roundConfig.firstActionValue.value : config.roundConfig.perActionValue.value;
                Turn turn = Turn.FromMaxActionValue(turnActionValue);
                _turns.Add(turn);
            }
            
            // 创建可移动者
            foreach (var moveConfig in config.moveComponentConfig) {
                var moveComponent = MoveComponent.FromConfig(moveConfig);
                _moveComponents.Add(moveComponent);
            }

            _roundContext.Status = RoundContext.RoundStatus.None;
            _timeRemainder = 0f;

            _frameRate = 1f/config.roundConfig.logicFrameRate;
        }

        /// <summary>
        /// 向前行动值运行
        /// </summary>
        void Step() {
            
            if(_roundContext.Status != RoundContext.RoundStatus.Running) return;

            // 校验回合合法
            int turnIndex = -1;
            for(int i = _roundContext.TurnIndex ; i < _turns.Count ; i++) {
                if (_turns[i].CheckForward()) {
                    turnIndex = i;
                    break;
                }
            }

            if (turnIndex == -1) {
                // 所有回合都已经结束了
                _roundContext.Status = RoundContext.RoundStatus.None;
                return;
            }
            
            // 行动者消耗行动值
            foreach (var moveComponent in _moveComponents) {
                moveComponent.Forward();
                if (moveComponent.IsPass()) {
                    _actionMoves.Enqueue(moveComponent);
                }
            }
            // 回合消耗行动值
            _turns[turnIndex].Forward();
            
            // 结算行动
            if (_actionMoves.Count > 0) {
                _roundContext.Status = RoundContext.RoundStatus.Option;
            }
        }

        void HandleOption() {
            if (_roundContext.Status != RoundContext.RoundStatus.Option) {
                return;
            }
            MoveComponent component = _actionMoves.Dequeue();
            DoAction(ref component);

            if (_actionMoves.Count == 0) {
                _roundContext.Status = RoundContext.RoundStatus.None;
            }

            Dump();
        }

        void Dump() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Round Context : {_roundContext.Dump()}");
            builder.AppendLine($"Round Total move : {DumpUtility.DumpList(_moveComponents)}");
            builder.AppendLine($"Round action move : {DumpUtility.DumpList(_actionMoves)}");
            builder.AppendLine($"Round total trun : {DumpUtility.DumpList(_turns)}");
            Debug.Log(builder.ToString());
        }

        void DoAction(ref MoveComponent moveComponent) {
            Debug.Log("pass one ");
        }
    }
}
