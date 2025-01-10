using AppFrame;
using Cysharp.Threading.Tasks;
using UIDocument.Script.App;
using UIFrame.Core;
using Unity.VisualScripting;
using UnityEngine;
using YooAsset;
namespace UIDocument.Script.Module {
    public class StartUp {
        
        public class Context : IContext {
            public UISystem UISystem;
            public SceneService.SceneService SceneService;
        }
        
        public struct CreateParam {
            public StartUp.Context StartUpContext;
            public AppContext AppContext;
        }
        
        Context _startUpContext;
        AppContext _appContext;
        
        public StartUp(in CreateParam createParam) {
            _startUpContext = createParam.StartUpContext;
            _appContext = createParam.AppContext;
        }

        public async void Play() {
            CreateLoadingStartUp();
            
            SceneHandle sceneHandle = _startUpContext.SceneService.LoadSceneAsync("HUD.unity");
            await sceneHandle.ToUniTask();
            Debug.Log("场景加载成功");
        }

        public void CreateLoadingStartUp() {
            LoadingPresenter presenter = new LoadingPresenter(_startUpContext);
            LoadingView loadingView = new LoadingView(_startUpContext);
            LoadingModel loadingModel = new LoadingModel(_startUpContext);
            presenter.Bind(loadingView, loadingModel);
            
        }
    }
}
