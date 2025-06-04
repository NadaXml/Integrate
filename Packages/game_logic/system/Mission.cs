using adt;
using cfg;
using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using UnityEngine;
using extension;

namespace game_logic.system {
    public class Mission : GameSystem {

        public async UniTask<GameProcedure> CreateMission() {
            string location = @"tbsetting";
            string locationRole = @"tbrole";
            using (var resHandleRole = GameContext.assetService.LoadAssetAsync<FlatBufferBinary>(locationRole)) {
                await resHandleRole.ToUniTask();
                if (resHandleRole.IsSucceed()) {
                    var flatBufferBinaryRole = resHandleRole.GetAsset<FlatBufferBinary>();
                    var tbrole = Tbrole.GetRootAsTbrole(new ByteBuffer(flatBufferBinaryRole.bytes));
                    
                    using (var resHandle = GameContext.assetService.LoadAssetAsync<FlatBufferBinary>(location)) {
                        await resHandle.ToUniTask();
                        if (resHandle.IsSucceed()) {
                            var flatBufferBinarySetting = resHandle.GetAsset<FlatBufferBinary>();
                            var tbsetting = Tbsetting.GetRootAsTbsetting(new ByteBuffer(flatBufferBinarySetting.bytes));
                            for (int i = 0; i < tbsetting.DataListLength; i++) {
                                var setting = tbsetting.DataList(i);
                                Debug.Log($"setting = {setting?.RoleId} {setting?.Index}");

                                for (int j = 0; j < tbrole.DataListLength; j++) {
                                    var role = tbrole.DataList(j);
                                    if (role?.Id == setting?.RoleId) {
                                        Debug.Log(role?.Dump());
                                    }
                                }
                            }
                            return GameProcedure.AssetLoad;
                        } else {
                            return GameProcedure.Success;
                        }
                    }
                }
                else {
                    return GameProcedure.AssetLoad;
                }
            }
        }
    }
}
