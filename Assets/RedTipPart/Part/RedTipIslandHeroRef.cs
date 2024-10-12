
using RedTipHelper.Core;

namespace RedTipPart.Part {
    public class RedTipIslandHeroRef : RedTipBase {

        public RedTipIslandHeroRef(string key, IRedTipContext context) : base(key, context) {
            var calc = new RedTipCalcRefOther(context, this);
            var keys = GetRefKeys();
            calc.SetRef(keys);
            ValCalc = calc;   
        }

        protected override void OnAwake() {
            
        }
        
        protected override void OnStart() {
            // RedTipCalcDict<int> calc = ValCalc as RedTipCalcDict<int>;

        }

        protected override void OnCalc() {
          
       }

        protected override void OnDestroy() {
  
        }



    }
}
