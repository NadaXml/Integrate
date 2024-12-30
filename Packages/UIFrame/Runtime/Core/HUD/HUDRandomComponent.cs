using LitMotion;
using UnityEngine;
namespace Core.HUD {
    
    public struct HUDRandomComponent {

        public enum RandomState {
            None = 0,
            Doing = 1,
            Again = 2,
        }
        
        public Vector3 EndPosition;
        public RandomState State;
        public MotionHandle Handle;
    }
}
