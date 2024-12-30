using Core.HUD;
using System;
using UnityEngine;
using YooAsset;
namespace Sample {
    public class HUDSample : MonoBehaviour {
        
        public Canvas Canvas;
        public GameObject Root;
        public Camera UICamera;

        void Awake() {
            
        }

        void Start() {
            CreateSystem();
        }

        void Update() {
            hudRenderSystem.Update();
        }

        void OnDestroy() {
            hudRenderSystem?.Destroy();
        }

        HUDRenderSystem hudRenderSystem;

        void CreateSystem() {
           
            hudRenderSystem = new HUDRenderSystem(Canvas, Root, UICamera);
            hudRenderSystem.Awake();
            
            HUDBase.HUDCreateParam[] createParams = new HUDBase.HUDCreateParam[10];
            for (int i = 0; i < 10; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/HUDUI.prefab"
                };
            }
            hudRenderSystem.CreateHUD(createParams);
            hudRenderSystem.RenderHUD();
        }
    }
}
