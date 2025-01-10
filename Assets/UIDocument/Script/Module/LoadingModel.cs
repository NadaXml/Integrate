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
    }
}
