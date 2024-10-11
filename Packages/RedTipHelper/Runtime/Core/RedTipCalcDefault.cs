using System;
namespace RedTipHelper.Core {
    public class RedTipCalcDefault : RedTipCalc {

        Func<bool> _func;
        public RedTipCalcDefault(IRedTipContext context, RedTipBase caller, Func<bool> func) : base(context, caller) {
            _func = func;
            _calcType = CalcType.Default;
        }
        
        public override void Calc() {
            IsActive = false;
            if (_func != null) {
                IsActive = _func.Invoke();
            }
        }
    }
}
