using UnityEditor.UI;
namespace RedTipHelper.Core {
    public class RedTipCalcRefOther : RedTipCalc {

        string[] _refKeys;
        RelationType _relation;
        
        public RedTipCalcRefOther(IRedTipContext context, RedTipBase caller) : base(context, caller) {
            _relation = RelationType.OR;
            _calcType = CalcType.RefOther;
        }

        public void SetRef(string[] keys) {
            if (keys == null) {
                return;
            }
            _refKeys = keys;
            foreach (var key in keys) {
                _context.RegisterRefObserver(key, Caller.Key);
            }
        }

        public void ResetRef() {
            if (_refKeys == null) {
                return;
            }
            foreach (string refKey in _refKeys) {
                _context.UnRegisterObserver(refKey, Caller.Key);
            }
        }
        
        public override void Calc() {
            var final = _relation == RelationType.AND;
            foreach (var refKey in _refKeys) {
                var redTip = _context.GetRedTip(refKey);
                if (redTip != null) {
                    if (_relation == RelationType.OR) {
                        final = final || redTip.GetActive();
                    }
                    else {
                        final = final && redTip.GetActive();
                    }
                }
                else {
                    // TODO 异常日志
                }
            }
        }

        public override void OnDestroy() {
            ResetRef();
            _refKeys = null;
        }
    }
}
