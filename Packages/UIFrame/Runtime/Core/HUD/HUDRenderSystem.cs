using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using LitMotion;
using LitMotion.Extensions;
using Random = UnityEngine.Random;

namespace Core.HUD {
    public class HUDRenderSystem {
        
        List<IHUD> huds { get; set; }

        Canvas _canvas;
        GameObject _root; 
        Camera _uiCamera;
        
        public HUDRenderSystem(Canvas canvas, GameObject root, Camera uiCamera) {
            _canvas = canvas;
            _root = root;
            _uiCamera = uiCamera;
        }
        
        public void Awake() {
            huds = new List<IHUD>();
        }
        
        public void Update() {
  
            UpdateBillboard();
        }

        void UpdateBillboard() {
            foreach (var hud in huds) {
                if (hud.Asset.AssetHandle.IsDone) {
                    UpdateHUD(hud.Asset.HUDBinder);
                }
            }
        }

        public void Destroy() {
            if (huds != null) {
                foreach (IHUD hud in huds) {
                    hud.Release();
                }
                huds.Clear();
            }
        }

        public void CreateHUD(IEnumerable<HUDBase.HUDCreateParam> createParams) {
            Destroy();
            foreach (HUDBase.HUDCreateParam hudCreateParam in createParams) {
                IHUD hud = new HUDBase(hudCreateParam);
                HUDComponent component = hud.Component;
                var assetHandle = YooAssets.LoadAssetAsync<GameObject>(component.PrefabName);
                hud.Asset = new HUDAssetComponent() {
                    AssetHandle = assetHandle,
                };
                huds.Add(hud);
            }
        }

        public void RenderHUD() {
            RenderHUDImp();
        }

        async void RenderHUDImp() {
            foreach (var hud in huds) {
                GameObject go = hud.Asset.InstantiatedGameObject;
                if (go == null) {
                    await hud.Asset.AssetHandle.Task;
                    go = hud.Asset.AssetHandle.InstantiateSync();
                    HUDAssetComponent asset = hud.Asset;
                    asset.InstantiatedGameObject = go;
                    asset.HUDBinder = go.GetComponent<HUDBinder>();
                    hud.Asset = asset;
                    go.transform.SetParent(_root.transform);
                    go.transform.localPosition = Random.insideUnitCircle * 100;
                }
            }
        }
        
        void UpdateHUD(HUDBinder hudBinder) {
            
            Transform transform = hudBinder.transform;
            // billboard hud
            transform.rotation = Quaternion.Euler(_uiCamera.transform.eulerAngles);




            // scale hud 没用模版测试，为了3d遮挡关联，通过scale和distance，控制大小

            // var distance = (transform.position - _uiCamera.transform.position).magnitude;
            // var scale = distance / 23f * 1.5f * 1.5f;
            // scale = scale > 3f ? 2.3f : scale;
            // var scaleV3 = new Vector3(
            //     scale / transform.lossyScale.x * transform.localScale.x, 
            //     scale / transform.lossyScale.y * transform.localScale.y,
            //     scale / transform.lossyScale.z * transform.localScale.z);
            // transform.localScale = scaleV3;
        }
        
    }  
}
