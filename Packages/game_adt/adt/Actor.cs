namespace adt {
    public class Actor : IMove {
        public BattleComponent battle;
        public ActionComponent action
        {
            get;
            set;
        }
    }
}
