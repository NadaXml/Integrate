using TMPro;
using UnityEngine;
namespace Core.HUD {
    public class HUDBinder : MonoBehaviour {
        public SpriteRenderer hp;
        public TextMeshPro name;
        public TextMeshPro job;
        
        public int hpSibling;
        public int nameSibling;
        public int jobSibling;
        
        public void Awake() {
            hpSibling = 2 - hp.transform.GetSiblingIndex();
            nameSibling = 2 - name.transform.GetSiblingIndex();
            jobSibling = 2 - job.transform.GetSiblingIndex();
        }
        
        public void AttachToText(Transform textCanvas) {
            job.transform.SetParent(textCanvas.transform, false);
            name.transform.SetParent(textCanvas.transform, false);
        }

        public void DetachFromText() {
            job.transform.SetParent(transform, false);
            name.transform.SetParent(transform, false);
        }

        public void AttachToImage(Transform imageCanvas) {
            hp.transform.SetParent(imageCanvas.transform, false);
        }

        public void DetachFromImage() {
            hp.transform.SetParent(transform, false);
        }
    }
}
