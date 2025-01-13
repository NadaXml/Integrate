using System;
using UnityEngine;
using UnityEngine.UI;
namespace UIFrame.Core.UIComponent {
    [Serializable]
    public class Slider : IUIComponent {

        public Image bg;
        public Image progress;
        public RectTransform progressRect;

        public void Awake() {
            progressRect = progress.GetComponent<RectTransform>();
        }
        public void Destroy() {
           
        }

        public void SetProgress(float value) {
            progressRect.anchorMax = new Vector2(value, progressRect.anchorMax.y);
        }
    }
}
