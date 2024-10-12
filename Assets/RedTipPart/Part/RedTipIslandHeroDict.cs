
using RedTipHelper.Core;

namespace RedTipPart.Part {
    public class RedTipIslandHeroDict : RedTipBase {

        public RedTipIslandHeroDict(string key, IRedTipContext context) : base(key, context) {
            ValCalc = new RedTipCalcDict<int>(context, this, CalcImp);
        }

        protected override void OnAwake() {
            
        }
        
        protected override void OnStart() {
            // RedTipCalcDict<int> calc = ValCalc as RedTipCalcDefault;
        }

        protected override void OnCalc() {
          
       }

        protected override void OnDestroy() {
  
        }


 
        bool CalcImp(int key) {
            return false;
        }

    }
}
