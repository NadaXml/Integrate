namespace UIFrame.Core {
    public interface IPresenter<TView, TModel>
        where TView : IView
        where TModel : IModel {
        TView View { get; }
        TModel Model { get; }
    }
}
