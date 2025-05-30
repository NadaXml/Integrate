using Cysharp.Threading.Tasks;
using UnityEngine;
namespace game_logic.system {
    public class Mission : GameSystem {

        public async UniTaskVoid CreateMission() {
            using (var resHandle = GameContext.assetService.LoadAssetAsync<TextAsset>("tbsetting.bin")) {
                await resHandle.ToUniTask();
                var textAsset = resHandle.GetAsset<TextAsset>();
                Debug.Log(textAsset.text);
            }
        }
    }
}
