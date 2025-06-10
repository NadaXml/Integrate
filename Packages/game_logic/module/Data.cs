using adt;
using cfg;
using Cysharp.Threading.Tasks;
using extension;
using game_logic;
using Google.FlatBuffers;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace data_module {
    public class Data : GameModule {

        #region 表格
        public cfg.Tbsetting tbsetting;
        public cfg.Tbrole tbrole;
        public cfg.Tbmission tbmission;
        #endregion

        #region 游戏数据
        public SimulationData simulationData { get; set; }
        #endregion
        
        public async UniTask<GameProcedure> Init(CancellationTokenSource cts) {
            return GameProcedure.Success;
        }

        public async UniTask<FlatBufferBinary> LoadConfigFlatBufferBinary(string location) {
            using (var resHandleRole = gameContext.assetService.LoadAssetAsync<FlatBufferBinary>(location)) {
                await resHandleRole.ToUniTask();
                if (resHandleRole.IsSucceed()) {
                    return resHandleRole.GetAsset<FlatBufferBinary>();
                }
            }
            return null;
        }

        public async UniTask<GameProcedure> LoadMissionCfg() {
            FlatBufferBinary temp;
            // TODO 关联，用于数据加载和卸载均衡
            // TODO 理论上这里是需要进行句柄的生命周期管理，这里保存的tb可能会失效
            // TODO 数据层加载释放管理
            temp = await LoadConfigFlatBufferBinary("tbrole");
            if (temp == null) {
                return GameProcedure.AssetLoad;
            }
            tbrole = Tbrole.GetRootAsTbrole(new ByteBuffer(temp.bytes));

            temp = await LoadConfigFlatBufferBinary("tbsetting");
            if (temp == null) {
                return GameProcedure.AssetLoad;
            }
            tbsetting = Tbsetting.GetRootAsTbsetting(new ByteBuffer(temp.bytes));
            
            temp = await LoadConfigFlatBufferBinary("tbmission");
            if (temp == null) {
                return GameProcedure.AssetLoad;
            }
            tbmission = Tbmission.GetRootAsTbmission(new ByteBuffer(temp.bytes));
            return GameProcedure.Success;
        }
    }
}
