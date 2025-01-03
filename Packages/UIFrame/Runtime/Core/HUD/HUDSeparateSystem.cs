using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
namespace Core.HUD {
    public class HUDSeparateSystem<T> : ISystem 
        where T : MonoBehaviour {

        public void Awake() {
            // throw new System.NotImplementedException();
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
            // throw new System.NotImplementedException();
        }

        public List<IHUD<T>> _huds;

        Canvas _root;
        GameObject _canvasText;
        GameObject _canvasImage;

        public HUDSeparateSystem(IEnumerable<IHUD<T>> huds, Canvas root) {
            _huds = new List<IHUD<T>>();
            _huds.AddRange(huds);
            _root = root;
        }
        
        public void SeparateHUDCanvas() {
            // 创建TextCanvas，ImageCanvas，用于合批
            CreateMulti();
            // 将HUD的部件attach到不同canvas上
            SeparateHUDImp();    
        }
        
        void CreateMulti() {
            if (_canvasText == null) {
                _canvasText = new GameObject();
                Object.DontDestroyOnLoad(_canvasText);
                _canvasText.transform.position = _root.transform.position;
                _canvasText.transform.rotation = _root.transform.rotation;
            }
            if (_canvasImage == null) {
                _canvasImage = new GameObject();
                Object.DontDestroyOnLoad(_canvasImage);
                _canvasImage.transform.position = _root.transform.position;
                _canvasImage.transform.rotation = _root.transform.rotation;
            }
        }
        
        async void SeparateHUDImp() {
            foreach (IHUD<T> hud in _huds) {
                if (hud is HUDBase<HUDBinder>) {
                    await UniTask.WaitUntil(() => hud.Asset.InstantiatedGameObject != null);
                    HUDBinder binder = hud.Asset.InstantiatedGameObject.GetComponent<HUDBinder>();
                    binder.AttachToImage(_canvasImage.transform);
                    binder.AttachToText(_canvasText.transform);
                }
            }
        }
    }
}
