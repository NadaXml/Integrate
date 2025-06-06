using System.Collections.Generic;
namespace adt {
    public class SimulationData {
        public BattleField battleField;
        public Dictionary<int, Actor> actors;
        public GameStatus gameStatus;

        int allocateId;
        public SimulationData() {
            allocateId = 0;
            actors = new Dictionary<int, Actor>();
        }
        
        public void AddActor(Actor actor) {
            int id = allocateId++;
            actors.Add(id, actor);
        }

        public void Destroy() {
            battleField = null;
            // 纯数据没有资源，不用释放
            actors.Clear();
        }
    }
}
