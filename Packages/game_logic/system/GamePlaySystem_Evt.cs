using event_adt;
using game_logic.module;

namespace game_logic.system {
    public partial class GamePlaySystem {
        public void OpenFundEvent() {
            gameContext.eventModule.Dispatcher.AddEventListener(EventDef.StartMission, StartMission);
            gameContext.eventModule.Dispatcher.AddEventListener(EventDef.EndMission, EndMission);
        }

        public void CloseFundEvent() {
            gameContext.eventModule.Dispatcher.RemoveEventListener(EventDef.StartMission, StartMission);
            gameContext.eventModule.Dispatcher.RemoveEventListener(EventDef.EndMission, EndMission);
        }

        public void OpenGameEvent() {
            gameContext.eventModule.Dispatcher.AddEventListener(EventDef.RoundHandle, RoundHandle);
        }

        public void CloseGameEvent() {
            gameContext.eventModule.Dispatcher.RemoveEventListener(EventDef.RoundHandle, RoundHandle);
        }
    }
}
