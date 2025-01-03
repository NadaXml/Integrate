using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Random = UnityEngine.Random;

namespace Core.HUD {
    public class HUDMoveSystemMulti<T> : ISystem 
        where T : MonoBehaviour  {

        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Update() {
            foreach (var hud in _huds) {
                DoUpdateMove(hud);
            }
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
        }

        List<IHUD<T>> _huds;
        public Func<Vector3> RandomEndPosition;
        Camera _uiCamera;
        
        public HUDMoveSystemMulti(IEnumerable<IHUD<T>> huds, Camera uiCamera) {
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
                if (hud is HUDBase<HUDBinder>) {
                    HUDBinder binder = hud.Asset.Binder as HUDBinder;
                    var handle1 = LMotion.Create(Vector3.zero, hud.Random.EndPosition, 3f)
                        .WithOnComplete(() => {
                            var random = hud.Random;
                            random.EndPosition = RandomEndPosition();
                            random.State = HUDRandomComponent.RandomState.Again;
                            hud.Random = random;
                        })
                        .Bind(x => {
                            binder.job.transform.localPosition = x + new Vector3(0f, 10.7f, hud.Random.Distance);
                            binder.name.transform.localPosition = x + new Vector3(0f, -9.5f, hud.Random.Distance); 
                            binder.hp.transform.localPosition = x + new Vector3(0f, -1.4f, hud.Random.Distance);
                        })
                        .AddTo(hud.Asset.InstantiatedGameObject);

                    var handle2 = LMotion.Create(1f, hud.Random.EndScale, 3f)
                        .WithOnComplete(() => {
                            var random = hud.Random;
                            random.EndScale = Random.Range(1f, 2f);
                            hud.Random = random;
                        })
                        .Bind(x => {
                            binder.job.transform.localScale = new Vector3(x, x, x);
                            binder.name.transform.localScale = new Vector3(x, x, x);
                            binder.hp.transform.localScale = new Vector3(x * 18f, x * 4f, x);
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
