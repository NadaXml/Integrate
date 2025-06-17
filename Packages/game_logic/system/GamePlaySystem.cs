using adt;
using Cysharp.Threading.Tasks;
using game_logic.module;
using System.Threading;
namespace game_logic.system {
    public partial class GamePlaySystem : GameSystem {
        public async UniTask<GameProcedure> Init(CancellationTokenSource cts) {
            OpenFundEvent();
            return GameProcedure.Success;
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            CloseFundEvent();
        }

        void StartMission(EventParam param) {
            OpenGameEvent();
        }

        void EndMission(EventParam param) {
            CloseGameEvent();
        }

        void RoundHandle(EventParam param) {
            
        }
    }
}
