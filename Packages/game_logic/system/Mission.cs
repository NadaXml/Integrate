using adt;
using cfg;
using Cysharp.Threading.Tasks;
using event_adt;
using System.Collections.Generic;

namespace game_logic.system {
    public class Mission : GameSystem {

        int increaseUUID;
        
        public async UniTask<GameProcedure> Init() {
            increaseUUID = 0;
            return GameProcedure.Success;
        }

        public async UniTask<GameProcedure> CreateMission() {
            GameProcedure ret = GameProcedure.None;
            // 加载关卡数据
            ret = await gameContext.dataModule.LoadMissionCfg();

            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            await UniTask.DelayFrame(5);
            
            gameContext.dataModule.simulationData = new SimulationData();
            gameContext.dataModule.simulationData.gameStatus = GameStatus.Prepare;
            
            ret = CreateBattleField();
            
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            ret = CreateActor();
            
            if (ret != GameProcedure.Success) {
                return ret;
            }

            ret = CreateAbility();
            
            if (ret == GameProcedure.Success) {
                gameContext.dataModule.simulationData.gameStatus = GameStatus.Start;
                gameContext.eventModule.Dispatcher.Send(EventDef.StartMission, null);
                // TODO startMission是异步的情况，事件是不应该同步且关注事件是否执行结束，要用另外的状态控制
                gameContext.dataModule.simulationData.ResetRunningStatus();
            }
            
            return ret;
        }

        public GameProcedure DestroyMission() {
            // 释放表格
            gameContext.dataModule.tbsetting = default;
            gameContext.dataModule.tbrole = default;
            gameContext.dataModule.tbmission = default;
            
            // 释放Data
            if (gameContext.dataModule.simulationData != null) {
                gameContext.dataModule.simulationData.Destroy();
                gameContext.dataModule.simulationData = null;
            }
            return GameProcedure.Success;
        }

        GameProcedure CreateActor() {
            SimulationData data = gameContext.dataModule.simulationData;
            ref Tbsetting tbsetting = ref gameContext.dataModule.tbsetting;
            ref Tbrole tbrole = ref gameContext.dataModule.tbrole;
            for (int i = 0; i < tbsetting.DataListLength; i++) {
                cfg.setting? setting = tbsetting.DataList(i);
                cfg.role? role1 = null;
                for (int j = 0; j < tbrole.DataListLength; j++) {
                    role? role2 = tbrole.DataList(j);
                    if (setting?.RoleId == role2?.Id) {
                        role1 = role2;
                    }
                }
                var actor = new Actor() {
                    action = ActionComponent.FromCfg(role1.GetValueOrDefault(), setting.GetValueOrDefault()),
                    battle = BattleComponent.FromCfg(role1.GetValueOrDefault())
                };
                actor.UUID = increaseUUID;
                data.AddActor(actor);
            }
            return GameProcedure.Success;
        }

        GameProcedure CreateAbility() {
            return GameProcedure.Success;
        }

        GameProcedure CreateBattleField() {
            SimulationData data = gameContext.dataModule.simulationData;
            BattleField battleField = new BattleField();
            cfg.Tbmission tbmission = gameContext.dataModule.tbmission;
            for (int i = 0; i < tbmission.DataListLength; i++) {
                cfg.mission? mission = tbmission.DataList(i);
                if (mission?.Id == 1) {
                    battleField.mission = new MissionComponent(mission.GetValueOrDefault());
                }
            }
            var turns = new List<TurnComponent>(battleField.mission.mission.MaxTurn);
            for (int i = 0; i < battleField.mission.mission.MaxTurn; i++) {
                int turnActionValue = battleField.mission.mission.TurnActionValue;
                if (i == 0) {
                    turnActionValue = battleField.mission.mission.FirstTrunActionValue;
                }
                turns.Add(new TurnComponent() {
                    roundActionValue = RoundActionValue.FromValue(turnActionValue)
                });
            }
            battleField.turns = turns;
            battleField.nowTurnIndex = 0;
            data.battleField = battleField;
            return GameProcedure.Success;
        }
    }
}
