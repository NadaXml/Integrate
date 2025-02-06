using AppFrame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UIDocument.Script.RoundSystem.ADT;
using UIDocument.Script.RoundSystem.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIDocument.Script.RoundSystem {
    
    [Serializable]
    /// <summary>
    /// 移动组件
    /// </summary>
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
                position = config.position
            };
        }

        public void Forward() {
            currentAction -= 1;
        }

        public bool IsPass() {
            return currentAction.IsPass();
        }
        public string Dump() {
            return $"maxActoin is {maxAction.Dump()}, currentAciton is {currentAction.Dump()}, speed is {speed.Dump()}";
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

        public static string SerializeToString(in MoveComponent component) {
            return JsonConvert.SerializeObject(component);
        }
        
        public static MoveComponent DeSerializeFromString(string json) {
            return JsonConvert.DeserializeObject<MoveComponent>(json);
        }
    }

    [Serializable]
    public struct MoveComponentStream {
        public List<MoveComponent> MoveComponents;
        
        public static string SerializeToString(in MoveComponentStream component) {
            return JsonConvert.SerializeObject(component);
        }
    
        public static MoveComponentStream DeSerializeFromString(string json) {
            return JsonConvert.DeserializeObject<MoveComponentStream>(json);
        }
        
    }
}
