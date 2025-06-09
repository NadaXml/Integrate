using game_fund;
namespace game_logic {
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
    }
}
