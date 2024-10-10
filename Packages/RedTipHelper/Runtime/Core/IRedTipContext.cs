using System.Collections.Generic;
namespace Core {
    public interface IRedTipContext {
        Dictionary<string, string> RefObservers { get; }
        IRedTipSchedule RefreshSchedule { get; }

        RedTipBase GetRedTip();
        void RegisterRefObserver(RedTipBase subject, RedTipBase observer);
        void UnRegisterObserver(RedTipBase subject, RedTipBase observer);
        void TriggerRef(RedTipBase subject);
        void AddToRefreshSchedule(RedTipBase redTip);
    }
}
