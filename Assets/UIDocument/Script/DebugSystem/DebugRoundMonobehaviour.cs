using UIDocument.Script.RoundSystem;
using UnityEngine;
namespace UIDocument.Script.DebugSystem {
    public class DebugRoundMonobehaviour : MonoBehaviour {
        public MoveComponentStream components;
        public void FromString(string result) {
            components = MoveComponentStream.DeSerializeFromString(result);
        }
    }
}
