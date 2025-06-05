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
            ret = await GameContext.dataModule.LoadMissionCfg();
            return ret;
        }
    }
}
