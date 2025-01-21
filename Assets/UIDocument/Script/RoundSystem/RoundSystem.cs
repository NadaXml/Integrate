using AppFrame;
using System.Collections;
using System.Collections.Generic;
using UIDocument.Script.RoundSystem.ADT;
using UIDocument.Script.RoundSystem.Config;
namespace UIDocument.Script.RoundSystem {
    public class RoundSystem : ISystem {

        public void Update(float deltaTime) {
            // 通过给定的帧率运行
            float loopTime = deltaTime + _timeRemainder;
            while (loopTime >= _frameRate) {
                
                Run();
                
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
        
        public struct RoundContext {
            public enum RoundStatus {
                None = 0,
                Running = 1,
                Suspend = 2,
                Pause = 3,
            }

            public RoundStatus Status;
            public int TurnIndex;
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

        // 系统运行时间问题
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
        void Run() {
            
            // 结算回合，获得可用的行动值
            Turn turn = _turns[_roundContext.TurnIndex];
            turn.Forward(1);
            turn.StepTurnActionValue();
        }

        void StepMoveable() {
            
        }
        
        void StepTurn() {

        }
    }
}
