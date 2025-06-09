using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace adt {
    public class SimulationData {
        public BattleField battleField;
        public Dictionary<int, Actor> actors = new();
        public GameStatus gameStatus;
        int allocateId = 0;

        public Queue<IMove> actionQueue = new Queue<IMove>();
        
        public void AddActor(Actor actor) {
            int id = allocateId++;
            actors.Add(id, actor);
        }

        public void Destroy() {
            battleField = null;
            actors.Clear();
            actionQueue.Clear();
        }

        IMove[] moves = null;
        public IMove[] GetMoves() {
            if (moves == null) {
                moves = actors.Values.ToArray<IMove>();
            }
            return moves;
        }

        public string Dump() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"status: {gameStatus}");
            builder.AppendLine($"now turnIndex: {battleField.nowTurnIndex}");
            return builder.ToString();
        }
    }
}
