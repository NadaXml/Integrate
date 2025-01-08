using AppFrame;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Core.HUD {
    public class HUDDistanceOrder<T> : ISystem 
        where T : MonoBehaviour {
        
        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Update() {
            // throw new System.NotImplementedException();
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
        }

        List<IHUD<T>> _huds;
        
        public float _start = -20f;
        public float _end = 20f;

        public float _gap = 10f;

        Camera _uiCamera;
        
        public HUDDistanceOrder(IEnumerable<IHUD<T>> huds, Camera uiCamera) {
            _huds = new List<IHUD<T>>();
            _huds.AddRange(huds);
            _uiCamera = uiCamera;
        }

        public async void DistanceOrder() {
            for (int index = 0; index < _huds.Count; index++) {
                IHUD<T> hud = _huds[index];
                var random = hud.Random;
                random.Distance = _start + index * _gap;
                hud.Random = random;
                await UniTask.WaitUntil(() => hud.Asset.InstantiatedGameObject != null);

                if (hud.Asset.Binder is HUDBinderCanvas) {
                    HUDBinderCanvas binder = hud.Asset.Binder as HUDBinderCanvas;
                    binder.jobRect.anchoredPosition3D = Vector3.zero + new Vector3(0f, 130f, 0f);
                    binder.jobRect.localPosition += new Vector3(0f, 0f, binder.jobSibling / 2f * _gap + _gap * index);
                    binder.nameRect.anchoredPosition3D = Vector3.zero + new Vector3(0f, -119f, 0f);
                    binder.nameRect.localPosition += new Vector3(0f, 0f, binder.nameSibling / 2f * _gap + _gap * index);
                    binder.hpRect.anchoredPosition3D = Vector3.zero + new Vector3(0f, 0f, 0f);
                    binder.hpRect.localPosition += new Vector3(0f, 0f, binder.hpSibling / 2f * _gap + _gap * index);
                } else if (hud.Asset.Binder is HUDBinder) {
                    HUDBinder binder = hud.Asset.Binder as HUDBinder;
                    binder.job.transform.localPosition = Vector3.zero + new Vector3(0f, 10.7f, binder.jobSibling / 2f * _gap + _gap * index);
                    binder.name.transform.localPosition = Vector3.zero + new Vector3(0f, -9.5f, binder.nameSibling / 2f * _gap + _gap * index);
                    binder.hp.transform.localPosition = Vector3.zero + new Vector3(0f, -1.4f, binder.hpSibling / 2f * _gap + _gap * index);
                }
            }
        }
    }
}
