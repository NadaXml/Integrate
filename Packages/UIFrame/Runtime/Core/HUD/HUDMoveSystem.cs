using LitMotion;
using LitMotion.Extensions;
using System.Collections.Generic;
using UnityEngine;
namespace Core.HUD {
    public class HUDMoveSystem {

        List<IHUD> _huds = new List<IHUD>();
        
        public HUDMoveSystem(IEnumerable<IHUD> huds) {
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
            foreach (IHUD hud in _huds) {
                if (hud.Random.Handle.IsActive()) {
                    hud.Random.Handle.Cancel();
                }
            }
            _huds.Clear();
        }
        
        public void CreateRandomMove() {
            for (int i = 0; i < _huds.Count; i++) {
                _huds[i].Random = new HUDRandomComponent() {
                    EndPosition = Random.insideUnitSphere * 3f
                };
                AppendRandomMove(_huds[i]);
            }
        }

        async void AppendRandomMove(IHUD hud) {
            await hud.Asset.AssetHandle.Task;

            hud.Random = new HUDRandomComponent() {
                State = HUDRandomComponent.RandomState.Again,
                EndPosition = Random.insideUnitSphere * 3f
            };
        }

        void DoUpdateMove(IHUD hud) {
            if (hud.Random.State == HUDRandomComponent.RandomState.Again) {
                var handle = LMotion.Create(Vector3.zero, hud.Random.EndPosition, 2f)
                    .WithScheduler(MotionScheduler.Update)
                    .WithOnComplete(() => {
                        hud.Random = new HUDRandomComponent() {
                            State = HUDRandomComponent.RandomState.Again,
                            EndPosition = Random.insideUnitSphere * 3f
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
