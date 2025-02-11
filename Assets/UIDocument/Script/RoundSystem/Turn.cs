using AppFrame;
using System;
using UIDocument.Script.Core.ADT;
using UnityEngine;
namespace UIDocument.Script.RoundSystem {
    /// <summary>
    /// 轮次
    /// </summary>
    [Serializable]
    public struct Turn : IComponent, IDumpable {
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
        public void Forward() {
            _targetActionValue -= 1;
        }

        /// <summary>
        /// 判断当前回合是否足够消耗行动值
        /// </summary>
        /// <returns>返回当前回合可以消耗的行动值</returns>
        public bool CheckForward() {
            return !_currentActionValue.IsPass();
        }

        public void Settle() {
            _currentActionValue = _targetActionValue;
            // TODO Other code
        }
        
        public string Dump() {
            return $"current is {_currentActionValue.value}, target is {_targetActionValue.value}";
        }
        public ulong actorSequenceId
        {
            get;
            set;
        }
        public ulong sequenceId
        {
            get;
            set;
        }
    }
}
