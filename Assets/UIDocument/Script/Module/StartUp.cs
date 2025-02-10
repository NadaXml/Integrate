using AppFrame;
using Cysharp.Threading.Tasks;
using UIDocument.Script.App;
using UIFrame.Core;
using UnityEngine;
using YooAsset;

namespace UIDocument.Script.Module {
    public class StartUp {
        
        public class Context : IContext {
            public UISystem uiSystem;
            public SceneSystem.SceneSystem sceneSystem;
            public AssetService.AssetService assetService;
            public RoundSystem.RoundSystem roundSystem;
            public SceneHandle loadingHandle;
            public GameSystem.GameSystem gameSystem;
        }
        
        public struct CreateParam {
            public StartUp.Context startUpContext;
            public AppContext appContext;
        }
        
        Context _startUpContext;
        AppContext _appContext;
        
        LoadingPresenter _loadingPresenter;
        
        public StartUp(in CreateParam createParam) {
            _startUpContext = createParam.startUpContext;
            _appContext = createParam.appContext;
        }

        public async void Play() {
            CreateLoadingStartUp();

            await UniTask.WaitUntil(()=>_loadingPresenter.View.IsRootOK);
            
            SceneHandle sceneHandle = _startUpContext.sceneSystem.LoadSceneAsync("Login");
            _startUpContext.loadingHandle = sceneHandle;
            
            await sceneHandle.ToUniTask();
            Debug.Log("场景加载成功");

            // DestroyLoadingStartUp();

            await _startUpContext.gameSystem.CreateGame();
            
            Debug.Log("加载Game成功");
            
        }

        public void Destroy() {
            DestroyLoadingStartUp();
        }
        
        void CreateLoadingStartUp() {
            LoadingPresenter presenter = new LoadingPresenter(_startUpContext);
            LoadingView loadingView = new LoadingView(_startUpContext);
            LoadingModel loadingModel = new LoadingModel(_startUpContext);
            presenter.Bind(loadingView, loadingModel);
            _startUpContext.uiSystem.RegisterPresenter(presenter);
            _loadingPresenter = presenter;
            _loadingPresenter.Render();
        }

        void DestroyLoadingStartUp() {
            
            Debug.Log("UI destroy");
            if (_loadingPresenter != null) {
                _loadingPresenter.Destroy();
                _startUpContext.uiSystem.UnRegisterPresenter(_loadingPresenter);
                _loadingPresenter = null;
            }
        }
    }
}
