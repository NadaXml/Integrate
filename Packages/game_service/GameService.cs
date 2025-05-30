using game_fund;
namespace game_logic {
    public class GameService : IService {

        public void Awake() {
            OnAwake();
        }
        public void Destroy() {
            OnDestroy();
        }
        protected virtual void OnAwake() {
            
        }

        protected virtual void OnDestroy() {
            
        }
    }
}
