using adt;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UnityEngine;
namespace game_logic.system {
    public class DebugConsole : GameSystem {
        GameObject _inspectorGo;

        protected override void OnDestroy() {
            if (_inspectorGo != null) {
                Object.Destroy(_inspectorGo);
                _inspectorGo = null;
            }
        }

        public async UniTask<GameProcedure> Init() {
            
            // 事件参数无返回值
            DebugLogConsole.AddCommand("aa", "dump round", () => {
            });
            
            DebugLogConsole.AddCommand("bb", "dump_round_inspector", () => {
            });
            
            DebugLogConsole.AddCommand("cc", "retry", () => {
                
            });
            
            return GameProcedure.Success;
        }
    }
}
