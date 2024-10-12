using System.Collections.Generic;
namespace RedTipHelper.Core {
    public interface IRedTipContext {
        Dictionary<string, List<string>> RefObservers { get; }
        IRedTipSchedule RefreshSchedule { get; }

        IList<string> GetRefKeys(string key);
        RedTipBase GetRedTip(string key);
        void RegisterRefObserver(string subject, string observer);
        void UnRegisterObserver(string subject, string observer);
        void TriggerRef(string subject);
        void AddToRefreshSchedule(RedTipBase redTip);
    }
}
