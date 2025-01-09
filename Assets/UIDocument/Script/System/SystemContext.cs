using AppFrame;
namespace UIDocument.Script.System {
    public class SystemContext : ISystemContext {

        public SystemContext(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
        }
        
        public IServiceProvider ServiceProvider
        {
            get;
        }
    }
}
