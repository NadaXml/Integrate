
using RedTipHelper.Core;

namespace RedTipPart.Part {
    public class RedTipIslandHeroDict : RedTipBase {

        public RedTipIslandHeroDict(string key, IRedTipContext context) : base(key, context) {
            ValCalc = new RedTipCalcDict<int>(context, this, CalcImp);
        }

        protected override void OnAwake() {
            
        }
        
        protected override void OnStart() {
            RedTipCalcDict<int> calc = ValCalc as RedTipCalcDict<int>;
            calc.SetKeys(new []{1,2});
            CalcSchedule();
        }

        protected override void OnCalc() {
          
       }

        protected override void OnDestroy() {
  
        }
 
        bool CalcImp(int key) {
            if (key == 1) {
                return true;
            } else if (key == 2) {
                return false;
            }
            return false;
        }

    }
}
