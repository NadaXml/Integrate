namespace Core {
    public interface ILifecycle {
        void Awake();
        void Start();
        void Destroy();
    }

    public interface IRedTipLifecycle : ILifecycle {
        void Calc();
        void CalcSchedule();
    }
}
