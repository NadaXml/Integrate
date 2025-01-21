using AppFrame;
using UIDocument.Script.RoundSystem.ADT;
using UnityEngine;
namespace UIDocument.Script.RoundSystem {
    /// <summary>
    /// 轮次
    /// </summary>
    public struct Turn : IComponent {
        /// <summary>
        /// 当前回合剩余行动值
        /// </summary>
        ActionValue _currentActionValue;
        ActionValue _targetActionValue;

        #region static constructor
        
        public static Turn FromMaxActionValue(int maxActionValue) {
            return new Turn() {
                _currentActionValue = new ActionValue() {
                    value = maxActionValue
                },
                _targetActionValue = new ActionValue() {
                    value = maxActionValue
                }
            };
        }
        
        #endregion
        
        /// <summary>
        /// 对回合的行动值进行消耗
        /// </summary>
        /// <param name="value"></param>
        public void Forward(int value) {
            _targetActionValue -= value;
        }

        /// <summary>
        /// 是否超过这个回合
        /// </summary>
        /// <returns></returns>
        public bool IsPassTurn() {
            return _currentActionValue.IsPass();
        }

        /// <summary>
        /// 继承上一个回合不足以消耗完的行动值
        /// </summary>
        /// <param name="prevTurn"></param>
        public void InheritPrevTurnValue(Turn prevTurn) {
            if (!prevTurn.IsPassTurn()) {
                return;
            }
            _currentActionValue -= prevTurn._currentActionValue;
        }

        /// <summary>
        /// 回合行动
        /// </summary>
        /// <returns>回合可用的行动值</returns>
        public ActionValue StepTurnActionValue() {
            // 目标行动值小于当前行动值，回合进入行动
            if (_currentActionValue <= _targetActionValue) {
                return ActionValue.ZERO;
            }
            ActionValue delta = _currentActionValue - _targetActionValue;
            _currentActionValue = _targetActionValue;
            return delta;
        }
    }
}
