using AppFrame;
using Google.FlatBuffers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UIDocument.Script.Core.Config;
using UnityEngine.Profiling;
namespace UIDocument.Script.Core.ADT {
    
    /// <summary>
    /// 移动组件
    /// </summary>
    [Serializable]
    public struct MoveComponent : IComponent, IDumpable, IComparable<MoveComponent>, IEquatable<MoveComponent> {
        /// <summary>
        /// 当前行动值
        /// </summary>
        public ActionValue currentAction;

        /// <summary>
        /// 最大行动值
        /// </summary>
        public ActionValue maxAction;

        /// <summary>
        /// 速度
        /// </summary>
        public Speed speed;

        /// <summary>
        /// 出站位置
        /// </summary>
        public int position;

        public static MoveComponent FromConfig(in MoveComponentConfig config) {
            Speed s = Speed.FromValue(config.speed);
            return new MoveComponent() {
                currentAction = ActionValue.FromSpeed(s),
                maxAction = ActionValue.FromSpeed(s),
                speed = s,
                position = config.position,
            };
        }

        public void Forward() {
            currentAction -= 1;
        }

        public bool IsPass() {
            return currentAction.IsPass();
        }

        public void Reset() {
            currentAction = maxAction;
        }
        
        public string Dump() {
            return $"position is {position} maxAction is {maxAction.Dump()}, currentAction is {currentAction.Dump()}, speed is {speed.Dump()}";
        }
        
        public int CompareTo(MoveComponent other) {
            var a = currentAction.CompareTo(other.currentAction);
            if (a == 0) {
                return position - other.position;
            }
            return a;
        }
        public bool Equals(MoveComponent other) {
            return position == other.position;
        }
        public override int GetHashCode() {
            return HashCode.Combine(currentAction, maxAction, speed, position);
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

    [Serializable]
    public struct MoveComponentStream {
        public List<MoveComponent> MoveComponents;
    }
}
