using Cysharp.Threading.Tasks;
using UIDocument.Script.App;
using UIFrame.Core;
using UnityEngine;
namespace UIDocument.Script.Module {
    public class StartUp {

        public struct CreateParam {
            public Provider Provider;
            public AppContext AppContext;
        }

        public struct Provider {
            public UISystem UISystem;
            public SceneService.SceneService SceneService;
        }

        Provider _provider;
        AppContext _appContext;
        
        public StartUp(in CreateParam createParam) {
            _provider = createParam.Provider;
            _appContext = createParam.AppContext;
        }

        public async void Play() {
            // UISystem uiSystem = system as UISystem;;
            
            await _provider.SceneService.LoadSceneAsync(_appContext.ServiceContext, "HUD.unity").ToUniTask();
            Debug.Log("场景加载成功");
        }
    }
}
