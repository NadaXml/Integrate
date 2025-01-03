using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Core.HUD {
    public class HUDMoveSystem<T> where T : MonoBehaviour {

        List<IHUD<T>> _huds;
        Camera _uiCamera;
        
        public Func<Vector3> RandomEndPosition;
        
        public HUDMoveSystem(IEnumerable<IHUD<T>> huds, Camera uiCamera) {
            _huds = new List<IHUD<T>>(huds);
            _huds.AddRange(huds);
            _uiCamera = uiCamera;
        }
        
        public void Awake() {
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
            }
            _huds.Clear();
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
                if (hud is HUDBase<HUDBinder>) {
                    se = hud.Asset.InstantiatedGameObject.transform.position;
                } else if (hud is HUDBase<HUDBinderCanvas>) {
                    se = hud.Asset.RectTransform.anchoredPosition;
                }
                
                var handle1 = LMotion.Create(se, hud.Random.EndPosition, 3f)
                    .WithOnComplete(() => {
                        hud.Random = new HUDRandomComponent() {
                            State = HUDRandomComponent.RandomState.Again,
                            EndPosition = RandomEndPosition()
                        };
                    });

                MotionHandle handle2 = default;
                if (hud is HUDBase<HUDBinder>) {
                    handle2 = handle1
                        .BindToPosition(hud.Asset.InstantiatedGameObject.transform)
                        .AddTo(hud.Asset.InstantiatedGameObject);
                } else if (hud is HUDBase<HUDBinderCanvas>) {
                    handle2 = handle1
                        .Bind(x => {
                            hud.Asset.RectTransform.anchoredPosition = x;
                        })
                        .AddTo(hud.Asset.InstantiatedGameObject);
                }
            
                hud.Random = new HUDRandomComponent() {
                    State = HUDRandomComponent.RandomState.Doing,
                    Handle = handle2
                };
            }
        }
    }
}
