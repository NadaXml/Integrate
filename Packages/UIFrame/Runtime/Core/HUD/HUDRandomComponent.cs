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
        public float EndScale;
        public RandomState State;
        public MotionHandle Handle;
        public MotionHandle ScaleHandle;

        public float Distance;
    }
}
