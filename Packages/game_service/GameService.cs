using game_fund;
namespace game_service {
    public class GameService : IService, IUpdate {

        public void Awake() {
            OnAwake();
        }
        public void Destroy() {
            OnDestroy();
        }
        
        public void Update() {
            OnUpdate();
        }
        
        protected virtual void OnAwake() {
            
        }

        protected virtual void OnDestroy() {
            
        }

        protected virtual void OnUpdate() {
            
        }

        protected IFunProvider fundProvider;
        public void SetFund(IFunProvider fund) {
            fundProvider = fund;
        }
    }
}
