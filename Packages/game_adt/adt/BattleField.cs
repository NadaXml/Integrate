using System.Collections.Generic;

namespace adt {
    public class BattleField {
        public MissionComponent mission;
        public int nowTurnIndex = 0;
        public List<TurnComponent> turns = new List<TurnComponent>();

        public bool CheckNeedTurn() {
            return nowTurnIndex >= mission.mission.MaxTurn;
        }

        public bool CheckMaxTurn() {
            return nowTurnIndex >= mission.mission.NeedTurn;
        }
    }
}
