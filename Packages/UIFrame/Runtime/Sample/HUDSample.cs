using Core.HUD;
using System;
using UnityEngine;
using YooAsset;
using Random = UnityEngine.Random;
namespace Sample {
    public class HUDSample : MonoBehaviour {
        
        public Canvas Canvas;
        public GameObject Root;
        public GameObject RootCanvas;
        public Camera UICamera;
        
        public 

        void Awake() {
            
        }

        void Start() {
            Application.targetFrameRate = 60;
            // CreateSystem();
            CreateSystemCanvas();
        }

        void Update() {
            _hudRenderSystem?.Update();
            _hudMoveSystem?.Update();
            
            _hudRenderSystemCanvas?.Update();
            _hudMoveSystemCanvas?.Update();
        }

        void OnDestroy() {
            _hudRenderSystem?.Destroy();
            _hudMoveSystem?.Destroy();
            
            _hudRenderSystemCanvas?.Destroy();
            _hudMoveSystemCanvas?.Destroy();
        }

        HUDRenderSystem<HUDBinder> _hudRenderSystem;
        HUDMoveSystem<HUDBinder> _hudMoveSystem;
        
        HUDRenderSystem<HUDBinderCanvas> _hudRenderSystemCanvas;
        HUDMoveSystem<HUDBinderCanvas> _hudMoveSystemCanvas;

        void CreateSystem() {
            _hudRenderSystem = new HUDRenderSystem<HUDBinder>(Canvas, Root, UICamera);
            _hudRenderSystem.Awake();
            
            HUDBase<HUDBinder>.HUDCreateParam[] createParams = new HUDBase<HUDBinder>.HUDCreateParam[100];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/HUDUI.prefab"
                };
            }
            _hudRenderSystem.CreateHUD(createParams);
            _hudRenderSystem.RenderHUD();

            _hudMoveSystem = new HUDMoveSystem<HUDBinder>(_hudRenderSystem.huds);
            _hudMoveSystem.Awake();
            _hudMoveSystem.RandomEndPosition = () => new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
            
            _hudMoveSystem.CreateRandomMove();
        }
        
        void CreateSystemCanvas() {
            _hudRenderSystemCanvas = new HUDRenderSystem<HUDBinderCanvas>(Canvas, RootCanvas, UICamera);
            _hudRenderSystemCanvas.Awake();
            
            HUDBase<HUDBinderCanvas>.HUDCreateParam[] createParams = new HUDBase<HUDBinderCanvas>.HUDCreateParam[100];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/HUDUICanvas.prefab"
                };
            }
            _hudRenderSystemCanvas.CreateHUD(createParams);
            _hudRenderSystemCanvas.RenderHUD();

            _hudMoveSystemCanvas = new HUDMoveSystem<HUDBinderCanvas>(_hudRenderSystemCanvas.huds);
            _hudMoveSystemCanvas.Awake();
            _hudMoveSystemCanvas.RandomEndPosition = () => new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
            
            _hudMoveSystemCanvas.CreateRandomMove();
        }
    }
}
