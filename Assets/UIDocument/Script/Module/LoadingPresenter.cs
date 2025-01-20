using UIFrame.Core;
namespace UIDocument.Script.Module {
    public class LoadingPresenter : IPresenter<LoadingView, LoadingModel> {
        public LoadingView View
        {
            get;
            set;
        }
        public LoadingModel Model
        {
            get;
            set;
        }
        public void Awake() {
            
        }
        public void Destroy() {
            View.Destroy();
        }

        StartUp.Context _context;
        public LoadingPresenter(StartUp.Context context) {
            _context = context;
        }

        public void Bind(LoadingView view, LoadingModel model) {
            View = view;
            Model = model;
        }

        public void Render() {
            View.Render();
        }
        
        public void Update(float deltaTime) {
            if (!(_context.loadingHandle is {IsValid: true})) {
                return;
            }
            if (!View.IsRootOK) {
                return;
            }
            if (_context.loadingHandle.IsDone) {
                View.RenderProgress(1f);
            } else {
                View.RenderProgress(_context.loadingHandle.Progress);
            }
        }
    }
}
