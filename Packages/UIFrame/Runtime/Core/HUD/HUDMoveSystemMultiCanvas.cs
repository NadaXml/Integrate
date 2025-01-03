using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.HUD {
    public class HUDMoveSystemMultiCanvas<T> : ISystem
        where T : MonoBehaviour {
        
        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Update() {
            foreach (var hud in _huds) {
                DoUpdateMove(hud);
            }
        }
        public void Destroy() {
            foreach (IHUD<T> hud in _huds) {
                if (hud.Random.Handle.IsActive()) {
                    hud.Random.Handle.Cancel();
                }
                if (hud.Random.ScaleHandle.IsActive()) {
                    hud.Random.ScaleHandle.Cancel();
                }
            }
            _huds.Clear();
        }
        
        List<IHUD<T>> _huds;
        public Func<Vector3> RandomEndPosition;
        Camera _uiCamera;

        public HUDMoveSystemMultiCanvas(IEnumerable<IHUD<T>> huds, Camera uiCamera) {
            _huds = new List<IHUD<T>>();
            _huds.AddRange(huds);
            _uiCamera = uiCamera;
        }

        public void CreateRandomMove() {
            for (int i = 0; i < _huds.Count; i++) {
                _huds[i].Random = new HUDRandomComponent() {
                    EndPosition = RandomEndPosition()
                };
                AppendRandomMove(_huds[i]);
            }
        }
        
        async void AppendRandomMove(IHUD<T> hud) {
            await hud.Asset.AssetHandle.Task;
            await UniTask.WaitUntil(() => hud.Asset.InstantiatedGameObject != null);

            hud.Random = new HUDRandomComponent() {
                State = HUDRandomComponent.RandomState.Again,
                EndPosition = RandomEndPosition()
            };
        }
        
        void DoUpdateMove(IHUD<T> hud) {
            if (hud.Random.State == HUDRandomComponent.RandomState.Again) { 
                Vector3 se = Vector3.zero;
                if (hud is HUDBase<HUDBinderCanvas>) {
                    HUDBinderCanvas binder = hud.Asset.Binder as HUDBinderCanvas;
                    var handle1 = LMotion.Create(Vector3.zero, hud.Random.EndPosition, 3f)
                        .WithOnComplete(() => {
                            var random = hud.Random;
                            random.EndPosition = RandomEndPosition();
                            random.State = HUDRandomComponent.RandomState.Again;
                            hud.Random = random;
                        })
                        .Bind(x => {
                            binder.jobRect.anchoredPosition3D = x + new Vector3(0f,130f, hud.Random.Distance);
                            binder.nameRect.anchoredPosition3D = x + new Vector3(0f, -119f, hud.Random.Distance); 
                            binder.hpRect.anchoredPosition3D = x + new Vector3(0f, 0f, hud.Random.Distance);
                        })
                        .AddTo(hud.Asset.InstantiatedGameObject);

                    var handle2 = LMotion.Create(1f, hud.Random.EndScale, 3f)
                        .WithOnComplete(() => {
                            var random = hud.Random;
                            random.EndScale = Random.Range(1f, 2f);
                            hud.Random = random;
                        })
                        .Bind(x => {
                            binder.jobRect.localScale = new Vector3(x, x, x);
                            binder.nameRect.localScale = new Vector3(x, x, x);
                            binder.hpRect.localScale = new Vector3(x, x, x);
                        })
                        .AddTo(hud.Asset.InstantiatedGameObject);
                    
                    hud.Random = new HUDRandomComponent() {
                        State = HUDRandomComponent.RandomState.Doing,
                        Handle = handle1,
                        ScaleHandle = handle2
                    };
                }
            }
        }
    }
}
