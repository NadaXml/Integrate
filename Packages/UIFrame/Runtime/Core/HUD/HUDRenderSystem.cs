using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using LitMotion;
using LitMotion.Extensions;
using Random = UnityEngine.Random;

namespace Core.HUD {
    public class HUDRenderSystem<T> where T : MonoBehaviour {
        
        public List<IHUD<T>> huds { get; set; }

        Canvas _canvas;
        GameObject _root; 
        Camera _uiCamera;
        
        public HUDRenderSystem(Canvas canvas, GameObject root, Camera uiCamera) {
            _canvas = canvas;
            _root = root;
            _uiCamera = uiCamera;
        }
        
        public void Awake() {
            huds = new List<IHUD<T>>();
        }
        
        public void Update() {
            // billboard hud
            Transform transform1 = _root.transform;
            transform1.rotation = Quaternion.Euler(_uiCamera.transform.eulerAngles);
            
            UpdateBillboard();
        }

        void UpdateBillboard() {
            foreach (var hud in huds) {
                if (hud.Asset.AssetHandle.IsDone) {
                    UpdateHUD(hud.Asset.Binder);
                }
            }
        }

        public void Destroy() {
            if (huds != null) {
                foreach (IHUD<T> hud in huds) {
                    hud.Release();
                }
                huds.Clear();
            }
        }

        public void CreateHUD(IEnumerable<HUDBase<T>.HUDCreateParam> createParams) {
            Destroy();
            foreach (HUDBase<T>.HUDCreateParam hudCreateParam in createParams) {
                IHUD<T> hud = new HUDBase<T>(hudCreateParam);
                HUDComponent component = hud.Component;
                var assetHandle = YooAssets.LoadAssetAsync<GameObject>(component.PrefabName);
                hud.Asset = new HUDAssetComponent<T>() {
                    AssetHandle = assetHandle,
                };
                huds.Add(hud);
            }
        }

        public void RenderHUD() {
            RenderHUDImp();
        }

        async void RenderHUDImp() {
            int index = 0;
            foreach (var hud in huds) {
                GameObject go = hud.Asset.InstantiatedGameObject;
                if (go == null) {
                    await hud.Asset.AssetHandle.Task;
                    go = hud.Asset.AssetHandle.InstantiateSync();
                    await UniTask.DelayFrame(1);

                    if (hud.Asset.IsDestroyed) {
                        Object.Destroy(go);
                        continue;
                    }
                    go.transform.SetParent(_root.transform, false);
                    go.transform.localPosition = Vector3.zero;
                    HUDAssetComponent<T> asset = hud.Asset;
                    asset.InstantiatedGameObject = go;
                    asset.Binder = go.GetComponent<T>();
                    asset.RectTransform = go.GetComponent<RectTransform>();
                    hud.Asset = asset;

                    if (asset.Binder is HUDBinderCanvas) {
                        HUDBinderCanvas binder = asset.Binder as HUDBinderCanvas;
                        binder.job.text = hud.Component.Job;
                        binder.name.text = hud.Component.Name;
                    } else if (asset.Binder is HUDBinder) {
                        HUDBinder binder = asset.Binder as HUDBinder;
                        binder.job.text = hud.Component.Job;
                        binder.name.text = hud.Component.Name;
                    }
                    index++;
                }
            }
        }
        
        void UpdateHUD(T hudBinder) {
            // scale hud 没用模版测试，为了3d遮挡关联，通过scale和distance，控制大小
        }
        
    }  
}
