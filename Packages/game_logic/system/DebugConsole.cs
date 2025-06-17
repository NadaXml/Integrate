using adt;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UnityEngine;
namespace game_logic.system {
    public class DebugConsole : GameSystem {
        protected override void OnDestroy() {
   
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
