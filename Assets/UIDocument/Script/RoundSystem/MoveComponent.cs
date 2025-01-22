using AppFrame;
using System.Text;
using UIDocument.Script.RoundSystem.ADT;
using UIDocument.Script.RoundSystem.Config;
namespace UIDocument.Script.RoundSystem {
    /// <summary>
    /// 移动组件
    /// </summary>
    public struct MoveComponent : IComponent, IDumpable {
        /// <summary>
        /// 当前行动值
        /// </summary>
        ActionValue _currentAction;
        
        /// <summary>
        /// 最大行动值
        /// </summary>
        ActionValue _maxAction;
        
        /// <summary>
        /// 速度
        /// </summary>
        Speed _speed;

        public static MoveComponent FromConfig(in MoveComponentConfig config) {
            return new MoveComponent() {
                _currentAction = ActionValue.FromValue(config.speed),
                _maxAction = ActionValue.FromValue(config.speed),
                _speed = Speed.FromValue(config.speed)
            };
        }

        public void Forward() {
            _currentAction -= 1;
        }

        public bool IsPass() {
            return _currentAction.IsPass();
        }
        public string Dump() {
            return $"maxActoin is {_maxAction.Dump()}, currentAciton is {_currentAction.Dump()}, speed is {_speed.Dump()}";
        }
    }
}
