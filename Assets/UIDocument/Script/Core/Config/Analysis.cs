using System;
using System.Collections.Generic;
using UnityEngine;
namespace UIDocument.Script.Core.Config {
    
    [Serializable]
    public class ActionCounter {
        public int Position;
        public int moveCount;
        public int totalDmg;
    }
    
    [CreateAssetMenu(fileName = "Analysis", menuName = "Analysis", order = 0)]
    public class Analysis : ScriptableObject {
        
        [SerializeField]
        public List<ActionCounter> Counter;

        public Analysis() {
            Counter = new List<ActionCounter>();
        }
    }
}
