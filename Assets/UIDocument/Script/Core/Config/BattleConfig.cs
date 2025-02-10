using System.Collections.Generic;
using UnityEngine;
namespace UIDocument.Script.Core.Config {
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "BattleConfig", order = 0)]
    public class BattleConfig : ScriptableObject {
        public RoundConfig roundConfig;
        public List<ActorConfig> actorConfig;
    }
}
