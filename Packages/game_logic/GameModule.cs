using game_fund;

namespace game_logic {
    public class GameModule : IModule {

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

        public void BindGameContext(GameContext gameContext) {
            this.gameContext = gameContext;
        }

        protected GameContext gameContext;
    }
}

        
