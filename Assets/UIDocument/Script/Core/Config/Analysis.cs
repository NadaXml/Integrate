using System;
using System.Collections.Generic;
using UnityEngine;
namespace UIDocument.Script.Core.Config {
    
    [Serializable]
    public class ActionCounter {
        public ulong actorSequence;
        public int position;
        public int moveCount;
        public int totalDmg;
    }
    
    /// <summary>
    /// 结算后技术统计
    /// </summary>
    [CreateAssetMenu(fileName = "Analysis", menuName = "Analysis", order = 0)]
    public class Analysis : ScriptableObject {
        
        [SerializeField]
        public List<ActionCounter> counter;

        public Analysis() {
            counter = new List<ActionCounter>();
        }
    }
}
