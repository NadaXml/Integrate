using AppFrame;
namespace UIFrame.Core {
    public interface IPresenter<out TView, out TModel> : IUpdater
        where TView : IView
        where TModel : IModel {
        TView View { get; }
        TModel Model { get; }
        
        void Awake();
        void Destroy();
    }
}
