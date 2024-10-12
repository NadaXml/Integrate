
using RedTipHelper.Core;

namespace RedTipPart.Part {
    public class RedTipIslandHero : RedTipBase {

        public RedTipIslandHero(string key, IRedTipContext context) : base(key, context) {
            ValCalc = new RedTipCalcDefault(context, this, CalcImp);
        }

        protected override void OnAwake() {
            
        }
        
        protected override void OnStart() {
            // RedTipCalcDict<int> calc = ValCalc as RedTipCalcDefault;
            CalcSchedule();
        }

        protected override void OnCalc() {
          
       }

        protected override void OnDestroy() {
  
        }

 
        bool CalcImp() {
            return true;
        }


    }
}
