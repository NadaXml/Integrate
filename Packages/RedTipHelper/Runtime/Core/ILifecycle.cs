namespace RedTipHelper.Core {
    public interface ILifecycle {
        void Awake();
        void Start();
        void Destroy();
    }

    public interface IRedTipLifecycle : ILifecycle {
        bool Calc();
        void CalcSchedule();
    }
}
