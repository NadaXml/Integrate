using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
namespace Core.HUD {
    public class HUDCanvasSeparateSystem<T> where T : MonoBehaviour {
        List<IHUD<T>> _huds;

        Canvas _canvasText;
        Canvas _canvasImage;

        Canvas _root;

        public HUDCanvasSeparateSystem(IEnumerable<IHUD<T>> huds, Canvas root) {
            _huds = new List<IHUD<T>>();
            _huds.AddRange(huds);
            _root = root;
        }

        public void Awake() {
        }

        public void Update() {
            if (_canvasText != null) {
                _canvasText.transform.rotation = _root.transform.rotation;
            }
            if (_canvasImage != null) {
                _canvasImage.transform.rotation = _root.transform.rotation;
            }
        }

        public void Destroy() {
            _huds.Clear();
        }

        public void SeparateHUDCanvas() {
            // 创建TextCanvas，ImageCanvas，用于合批
            CreateMultiCanvas();
            // 将HUD的部件attach到不同canvas上
            SeparateHUDCanvasImp();    
        }

        void CreateMultiCanvas() {
            if (_canvasText == null) {
                _canvasText = new GameObject().AddComponent<Canvas>();
                Object.DontDestroyOnLoad(_canvasText.gameObject);
                _canvasText.transform.position = _root.transform.position;
                _canvasText.transform.rotation = _root.transform.rotation;
                _canvasText.transform.localScale = _root.transform.localScale;
            }
            if (_canvasImage == null) {
                _canvasImage = new GameObject().AddComponent<Canvas>();
                Object.DontDestroyOnLoad(_canvasImage.gameObject);
                _canvasImage.transform.position = _root.transform.position;
                _canvasImage.transform.rotation = _root.transform.rotation;
                _canvasImage.transform.localScale = _root.transform.localScale;
            }
        }
        
        async void SeparateHUDCanvasImp() {
            foreach (IHUD<T> hud in _huds) {
                if (hud is HUDBase<HUDBinderCanvas>) {
                    await UniTask.WaitUntil(() => hud.Asset.InstantiatedGameObject != null);
                    HUDBinderCanvas binder = hud.Asset.InstantiatedGameObject.GetComponent<HUDBinderCanvas>();
                    binder.AttachToImageCanvas(_canvasImage.transform);
                    binder.AttachToTextCanvas(_canvasText.transform);
                }
            }
        }
    }
}
