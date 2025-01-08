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

        public float hudDistance;

        int hudCount = 10; 

        void Awake() {
            
        }

        void Start() {
            Application.targetFrameRate = 60;
            // CreateSystem();
            // CreateSystemCanvas();
            CreateSystemCanvasMulti();
            // CreateSystemMulti();
        }

        void Update() {

            hudDistance = (UICamera.transform.position - RootCanvas.transform.position).magnitude;
            
            _hudRenderSystem?.Update();
            _hudMoveSystem?.Update();
            _hudSeparateSystem?.Update();
            _hudMoveMultiSystem?.Update();
            
            _hudSeparateCanvasSystem?.Update();
            _hudRenderSystemCanvas?.Update();
            _hudMoveSystemCanvas?.Update();
            _hudMoveSystemCanvasMulti?.Update();
        }

        void OnDestroy() {
            _hudRenderSystem?.Destroy();
            _hudMoveSystem?.Destroy();
            _hudMoveMultiSystem?.Destroy();
            _hudSeparateSystem?.Destroy();
            
            _hudRenderSystemCanvas?.Destroy();
            _hudMoveSystemCanvas?.Destroy();
            _hudMoveSystemCanvasMulti?.Destroy();
            
            _hudSeparateCanvasSystem?.Destroy();
            
            
            _hudCanvasDistanceOrderSystem?.Destroy();
            _hudDistanceOrderSystem?.Destroy();
            
            _hudStencilOrderSystem?.Destroy();
        }

        HUDRenderSystem<HUDBinder> _hudRenderSystem;
        HUDMoveSystem<HUDBinder> _hudMoveSystem;
                
        HUDSeparateSystem<HUDBinder> _hudSeparateSystem;
        HUDMoveSystemMulti<HUDBinder> _hudMoveMultiSystem;
        
        HUDRenderSystem<HUDBinderCanvas> _hudRenderSystemCanvas;
        HUDMoveSystem<HUDBinderCanvas> _hudMoveSystemCanvas;
        HUDMoveSystemMultiCanvas<HUDBinderCanvas> _hudMoveSystemCanvasMulti;
        HUDCanvasSeparateSystem<HUDBinderCanvas> _hudSeparateCanvasSystem;
        
        
        HUDDistanceOrder<HUDBinderCanvas> _hudCanvasDistanceOrderSystem;
        HUDDistanceOrder<HUDBinder> _hudDistanceOrderSystem;
        
        HUDStencilOrderSystem<HUDBinder> _hudStencilOrderSystem;

        void CreateSystem() {
            _hudRenderSystem = new HUDRenderSystem<HUDBinder>(Canvas, Root, UICamera);
            _hudRenderSystem.Awake();
            
            HUDBase<HUDBinder>.HUDCreateParam[] createParams = new HUDBase<HUDBinder>.HUDCreateParam[hudCount];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/Misc/HUDUI.prefab"
                };
            }
            _hudRenderSystem.CreateHUD(createParams);
            _hudRenderSystem.RenderHUD();

            _hudMoveSystem = new HUDMoveSystem<HUDBinder>(_hudRenderSystem.huds, UICamera);
            _hudMoveSystem.Awake();
            _hudMoveSystem.RandomEndPosition = () => new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
            
            _hudMoveSystem.CreateRandomMove();
        }
        
        void CreateSystemCanvas() {
            _hudRenderSystemCanvas = new HUDRenderSystem<HUDBinderCanvas>(Canvas, RootCanvas, UICamera);
            _hudRenderSystemCanvas.Awake();
            
            HUDBase<HUDBinderCanvas>.HUDCreateParam[] createParams = new HUDBase<HUDBinderCanvas>.HUDCreateParam[hudCount];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/Misc/HUDUICanvas.prefab"
                };
            }
            _hudRenderSystemCanvas.CreateHUD(createParams);
            _hudRenderSystemCanvas.RenderHUD();

            _hudMoveSystemCanvas = new HUDMoveSystem<HUDBinderCanvas>(_hudRenderSystemCanvas.huds, UICamera);
            _hudMoveSystemCanvas.Awake();
            _hudMoveSystemCanvas.RandomEndPosition = () => new Vector3(Random.Range(-300f, 300f), Random.Range(-300f, 300f), Random.Range(-20f,20f));
            
            _hudMoveSystemCanvas.CreateRandomMove();
        }

        void CreateSystemCanvasMulti() {
            _hudRenderSystemCanvas = new HUDRenderSystem<HUDBinderCanvas>(Canvas, RootCanvas, UICamera);
            _hudRenderSystemCanvas.Awake();
            
            HUDBase<HUDBinderCanvas>.HUDCreateParam[] createParams = new HUDBase<HUDBinderCanvas>.HUDCreateParam[hudCount];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job1{i}",
                    PrefabName = "Assets/UIDocument/Res/Misc/HUDUICanvas.prefab"
                };
            }
            _hudRenderSystemCanvas.CreateHUD(createParams);
            _hudRenderSystemCanvas.RenderHUD();

            _hudSeparateCanvasSystem = new HUDCanvasSeparateSystem<HUDBinderCanvas>(_hudRenderSystemCanvas.huds, this.Canvas);
            _hudSeparateCanvasSystem.SeparateHUDCanvas();

            _hudCanvasDistanceOrderSystem = new HUDDistanceOrder<HUDBinderCanvas>(_hudRenderSystemCanvas.huds, UICamera);
            _hudCanvasDistanceOrderSystem.DistanceOrder();

            // _hudMoveSystemCanvasMulti = new HUDMoveSystemMultiCanvas<HUDBinderCanvas>(_hudRenderSystemCanvas.huds, UICamera);
            // _hudMoveSystemCanvasMulti.Awake();
            // _hudMoveSystemCanvasMulti.RandomEndPosition = () => new Vector3(Random.Range(-300f, 300f), Random.Range(-300f, 300f), Random.Range(-20f,20f));

            // _hudMoveSystemCanvasMulti.CreateRandomMove();
        }

        void CreateSystemMulti() {
            _hudRenderSystem = new HUDRenderSystem<HUDBinder>(Canvas, Root, UICamera);
            _hudRenderSystem.Awake();
            
            HUDBase<HUDBinder>.HUDCreateParam[] createParams = new HUDBase<HUDBinder>.HUDCreateParam[hudCount];
            for (int i = 0; i < createParams.Length; i++) {
                createParams[i].Component = new HUDComponent() {
                    Hp = 30f,
                    Name = $"name{i}",
                    Job = $"job{i}",
                    PrefabName = "Assets/UIDocument/Res/Misc/HUDUI.prefab"
                };
            }
            _hudRenderSystem.CreateHUD(createParams);
            _hudRenderSystem.RenderHUD();
            
            _hudSeparateSystem = new HUDSeparateSystem<HUDBinder>(_hudRenderSystem.huds, UICamera);
            _hudSeparateSystem.SeparateHUDCanvas();

            _hudDistanceOrderSystem = new HUDDistanceOrder<HUDBinder>(_hudRenderSystem.huds, UICamera);
            _hudDistanceOrderSystem.DistanceOrder();

            // _hudStencilOrderSystem = new HUDStencilOrderSystem<HUDBinder>(_hudRenderSystem.huds, UICamera);
            // _hudStencilOrderSystem.StencilOrder();
            
            _hudMoveMultiSystem = new HUDMoveSystemMulti<HUDBinder>(_hudRenderSystem.huds, UICamera);
            _hudMoveMultiSystem.Awake();
            _hudMoveMultiSystem.RandomEndPosition = () => new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0f);
            
            _hudMoveMultiSystem.CreateRandomMove();
        }
    }
}
