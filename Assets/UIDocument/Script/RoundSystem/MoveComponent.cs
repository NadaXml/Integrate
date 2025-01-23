using AppFrame;
using System;
using UIDocument.Script.RoundSystem.ADT;
using UIDocument.Script.RoundSystem.Config;
namespace UIDocument.Script.RoundSystem {
    /// <summary>
    /// 移动组件
    /// </summary>
    public struct MoveComponent : IComponent, IDumpable, IComparable<MoveComponent>, IEquatable<MoveComponent> {
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

        /// <summary>
        /// 出站位置
        /// </summary>
        int _position;

        public static MoveComponent FromConfig(in MoveComponentConfig config) {
            return new MoveComponent() {
                _currentAction = ActionValue.FromValue(config.speed),
                _maxAction = ActionValue.FromValue(config.speed),
                _speed = Speed.FromValue(config.speed),
                _position = config.position
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
        
        public int CompareTo(MoveComponent other) {
            var a = _currentAction.CompareTo(other._currentAction);
            if (a == 0) {
                return _position - other._position;
            }
            return a;
        }
        public bool Equals(MoveComponent other) {
            return _position == other._position;
        }
        public override int GetHashCode() {
            return HashCode.Combine(_currentAction, _maxAction, _speed, _position);
        }
    }
}
