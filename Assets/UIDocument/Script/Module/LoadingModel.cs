using UIFrame.Core;
namespace UIDocument.Script.Module {
    public class LoadingModel : IModel {
        public struct LoadingProgress {
            public float Progress;
        }

        public LoadingProgress Progress;
        
        StartUp.Context _context;
        public LoadingModel(StartUp.Context context) {
            _context = context;
        }

        public float GetProgress() {
            if (!_context.loadingHandle.IsValid) {
                return 0f;
            }
            if (_context.loadingHandle.IsDone) {
                return 1f;
            } else {
                return _context.loadingHandle.Progress;
            }
        }
    }
}
