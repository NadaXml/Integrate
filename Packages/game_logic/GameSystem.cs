using game_fund;
namespace game_logic {
    public class GameSystem : ISystem, IUpdate {
        
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

        public void BindGameContext(GameContext gameContext) {
            this.gameContext = gameContext;
        }
        
        public GameContext gameContext;

    }
}
