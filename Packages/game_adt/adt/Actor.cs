namespace adt {
    public class Actor 
        : IMove
        , IBattle {
        
        public ActionComponent action
        {
            get;
            set;
        }
        public BattleComponent battle
        {
            get;
            set;
        }
        
        public int UUID
        {
            get;
            set;
        }
    }
}
