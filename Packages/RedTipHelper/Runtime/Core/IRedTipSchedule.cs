namespace RedTipHelper.Core {
    public interface IRedTipSchedule : IService {
        void AddRedTip(RedTipBase redTip);

        void Schedule();
    }
}
