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

        StartUp.Context _context;
        public LoadingPresenter(StartUp.Context context) {
            _context = context;
        }

        public void Bind(LoadingView view, LoadingModel model) {
            View = view;
            Model = model;
        }

        public void UpdateProgress() {
        }
    }
}
