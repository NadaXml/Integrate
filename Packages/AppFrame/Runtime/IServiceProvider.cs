namespace AppFrame {
    public interface IServiceProvider {
        T GetService<T>(int serviceType) where T : class, IService;
        T GetSystem<T>(int systemType) where T : class, ISystem;
    }
}
