using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Core.HUD {
    public class HUDBinderCanvas : MonoBehaviour {
        public Image hp;
        public TextMeshProUGUI name;
        public TextMeshProUGUI job;

        public RectTransform hpRect;
        public RectTransform nameRect;
        public RectTransform jobRect;

        public int hpSibling;
        public int nameSibling;
        public int jobSibling;

        public void Awake() {
            hpSibling = 2 - hpRect.GetSiblingIndex();
            nameSibling = 2 - nameRect.GetSiblingIndex();
            jobSibling = 2 - jobRect.GetSiblingIndex();
        }


        public void AttachToTextCanvas(Transform textCanvas) {
            job.transform.SetParent(textCanvas.transform, false);
            name.transform.SetParent(textCanvas.transform, false);
        }

        public void DetachFromTextCanvas() {
            job.transform.SetParent(transform, false);
            name.transform.SetParent(transform, false);
        }

        public void AttachToImageCanvas(Transform imageCanvas) {
            hp.transform.SetParent(imageCanvas.transform, false);
        }

        public void DetachFromImageCanvas() {
            hp.transform.SetParent(transform, false);
        }
    }
}
