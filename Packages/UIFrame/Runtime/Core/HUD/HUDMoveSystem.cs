using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Core.HUD {
    public class HUDMoveSystem<T> where T : MonoBehaviour {

        List<IHUD<T>> _huds;
        
        public Func<Vector3> RandomEndPosition;
        
        public HUDMoveSystem(IEnumerable<IHUD<T>> huds) {
            _huds = new List<IHUD<T>>(huds);
            _huds.AddRange(huds);
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

            hud.Random = new HUDRandomComponent() {
                State = HUDRandomComponent.RandomState.Again,
                EndPosition = RandomEndPosition()
            };
        }

        void DoUpdateMove(IHUD<T> hud) {
            if (hud.Random.State == HUDRandomComponent.RandomState.Again) {
                var handle = LMotion.Create(hud.Asset.InstantiatedGameObject.transform.position, hud.Random.EndPosition, 3f)
                    .WithScheduler(MotionScheduler.Update)
                    .WithOnComplete(() => {
                        hud.Random = new HUDRandomComponent() {
                            State = HUDRandomComponent.RandomState.Again,
                            EndPosition = RandomEndPosition()
                        };
                    })
                    .BindToPosition(hud.Asset.InstantiatedGameObject.transform)
                    .AddTo(hud.Asset.InstantiatedGameObject);
                hud.Random = new HUDRandomComponent() {
                    State = HUDRandomComponent.RandomState.Doing,
                    Handle = handle
                };
            }
        }
    }
}
