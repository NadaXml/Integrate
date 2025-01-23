using AppFrame;
using Cysharp.Threading.Tasks;
using UIDocument.Script.App;
using UIDocument.Script.RoundSystem.Config;
using UIFrame.Core;
using UnityEngine;
using YooAsset;
namespace UIDocument.Script.Module {
    public class StartUp {
        
        public class Context : IContext {
            public UISystem UISystem;
            public SceneSystem.SceneSystem SceneSystem;
            public AssetService.AssetService AssetService;
            public RoundSystem.RoundSystem RoundSystem;
            public SceneHandle loadingHandle;
        }
        
        public struct CreateParam {
            public StartUp.Context StartUpContext;
            public AppContext AppContext;
        }
        
        Context _startUpContext;
        AppContext _appContext;
        
        LoadingPresenter _loadingPresenter;
        
        public StartUp(in CreateParam createParam) {
            _startUpContext = createParam.StartUpContext;
            _appContext = createParam.AppContext;
        }

        public async void Play() {
            CreateLoadingStartUp();

            await UniTask.WaitUntil(()=>_loadingPresenter.View.IsRootOK);
            
            SceneHandle sceneHandle = _startUpContext.SceneSystem.LoadSceneAsync("Login");
            _startUpContext.loadingHandle = sceneHandle;
            
            await sceneHandle.ToUniTask();
            Debug.Log("场景加载成功");

            // DestroyLoadingStartUp();
            
            AssetHandle handle = _startUpContext.AssetService.LoadAssetAsync<ScriptableObject>("BattleConfig");
            await handle.ToUniTask();
            
            Debug.Log("加载Demo配置成功");
            
            BattleConfig roundConfig = handle.AssetObject as BattleConfig; 
            if (roundConfig != null) {
                _startUpContext.RoundSystem.CreateSet(in roundConfig);
                _startUpContext.RoundSystem.StartSet();
            }
            handle.Release();
        }

        public void Destroy() {
            DestroyLoadingStartUp();
        }
        
        void CreateLoadingStartUp() {
            LoadingPresenter presenter = new LoadingPresenter(_startUpContext);
            LoadingView loadingView = new LoadingView(_startUpContext);
            LoadingModel loadingModel = new LoadingModel(_startUpContext);
            presenter.Bind(loadingView, loadingModel);
            _startUpContext.UISystem.RegisterPresenter(presenter);
            _loadingPresenter = presenter;
            _loadingPresenter.Render();
        }

        void DestroyLoadingStartUp() {
            
            Debug.Log("UI destroy");
            if (_loadingPresenter != null) {
                _loadingPresenter.Destroy();
                _startUpContext.UISystem.UnRegisterPresenter(_loadingPresenter);
                _loadingPresenter = null;
            }
        }
    }
}
