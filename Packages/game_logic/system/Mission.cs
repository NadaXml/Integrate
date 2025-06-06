using adt;
using cfg;
using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using extension;
using UnityEngine;

namespace game_logic.system {
    public class Mission : GameSystem {

        public async UniTask<GameProcedure> CreateMission() {
            GameProcedure ret = GameProcedure.None;
            // 加载关卡数据
            ret = await gameContext.dataModule.LoadMissionCfg();
            return ret;
        }

        public GameProcedure CreateActor() {
            return GameProcedure.Success;
        }

        public GameProcedure CreateBattleField() {
            BattleField battleField = new BattleField();
            battleField.mission = new MissionComponent() {

            };
            gameContext.dataModule.battleField = battleField;
            return GameProcedure.Success;
        }
    }
}
