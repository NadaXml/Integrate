using System.Security.Cryptography;
namespace RedTipHelper.Core {
    public interface IRedTipCalc : ILifecycle {
        
        RedTipBase Caller { get; }
        bool IsActive { get; }

        void Calc();
    }
    
    public abstract class RedTipCalc : IRedTipCalc {

        public enum CalcType {
            None = 0,
            Default = 1,
            Dict = 2,
            RefOther = 3
        }
        
        public enum CalcState {
            NoActive = 0,
            Active = 1,
        }

        public enum RelationType {
            OR = 0,
            AND = 1
        }

        protected IRedTipContext _context;
        protected CalcType _calcType;
        public RedTipBase Caller
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            protected set;
        }
        public abstract void Calc();

        public RedTipCalc(IRedTipContext context, RedTipBase caller) {
            _context = context;
            Caller = caller;
            _calcType = CalcType.None;
        }

        public void Awake() {
        }
        public void Start() {
        }
        public void Destroy() {
            OnDestroy();
        }

        public virtual void OnDestroy(){}

    }
}
