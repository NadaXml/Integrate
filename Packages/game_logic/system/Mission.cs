using adt;
using cfg;
using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using extension;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace game_logic.system {
    public class Mission : GameSystem {

        public async UniTask<GameProcedure> CreateMission() {
            GameProcedure ret = GameProcedure.None;
            // 加载关卡数据
            ret = await gameContext.dataModule.LoadMissionCfg();

            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            await UniTask.DelayFrame(5);

            gameContext.dataModule.simulationData = new SimulationData();
            
            ret = CreateBattleField();
            
            if (ret != GameProcedure.Success) {
                return ret;
            }
            
            ret = CreateActor();
            return ret;
        }

        public GameProcedure DestroyMission() {
            // 释放表格
            gameContext.dataModule.tbsetting = default;
            gameContext.dataModule.tbrole = default;
            gameContext.dataModule.tbmission = default;
            
            // 释放Data
            gameContext.dataModule.simulationData.Destroy();
            gameContext.dataModule.simulationData = null;
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
                    action = new ActionComponent() {
                        speed =  new Attr(role1.GetValueOrDefault().AttrGroup.GetValueOrDefault().Speed),
                        position = setting.GetValueOrDefault().Index
                    },
                    battle = new BattleComponent() {
                        attrGroup = adt.AttrGroup.FromCfg(role1?.AttrGroup)
                    }
                };
                data.AddActor(actor);
            }
            return GameProcedure.Success;
        }

        GameProcedure CreateBattleField() {
            SimulationData data = gameContext.dataModule.simulationData;
            BattleField battleField = new BattleField();
            Tbmission tbmission = gameContext.dataModule.tbmission;
            battleField.mission = new MissionComponent(ref tbmission);
            data.battleField = battleField;
            return GameProcedure.Success;
        }
    }
}
