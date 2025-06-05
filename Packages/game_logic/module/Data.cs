using adt;
using cfg;
using Cysharp.Threading.Tasks;
using extension;
using game_logic;
using Google.FlatBuffers;
using System.Threading;
using UnityEngine;

namespace data_module {
    public class Data : GameModule {

        public Tbsetting tbsetting;
        public Tbrole tbrole;
        
        public async UniTask<GameProcedure> Init(CancellationToken cts) {
            return GameProcedure.Success;
        }
        
        public async UniTask<GameProcedure> LoadMissionCfg() {
            // TODO 关联，用于数据加载和卸载均衡
            string location = @"tbsetting";
            string locationRole = @"tbrole";

            GameProcedure ret = GameProcedure.None;
            
            using (var resHandleRole = GameContext.assetService.LoadAssetAsync<FlatBufferBinary>(locationRole)) {
                await resHandleRole.ToUniTask();
                if (resHandleRole.IsSucceed()) {
                    var flatBufferBinaryRole = resHandleRole.GetAsset<FlatBufferBinary>();
                    tbrole = Tbrole.GetRootAsTbrole(new ByteBuffer(flatBufferBinaryRole.bytes));
                }
                else {
                    return GameProcedure.AssetLoad;
                }
            }

            using (var resHandle = GameContext.assetService.LoadAssetAsync<FlatBufferBinary>(location)) {
                await resHandle.ToUniTask();
                if (resHandle.IsSucceed()) {
                    var flatBufferBinarySetting = resHandle.GetAsset<FlatBufferBinary>();
                    tbsetting = Tbsetting.GetRootAsTbsetting(new ByteBuffer(flatBufferBinarySetting.bytes));
                }
                else {
                    return GameProcedure.AssetLoad;
                }
            }
            // 数据层加载释放管理
            return ret;
        }
    }
}
