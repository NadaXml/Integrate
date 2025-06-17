using adt;
using Cysharp.Threading.Tasks;
using game_logic;
using System.Threading;
using UnityEditor.Search;
using UnityEngine.UI;
namespace game_logic.module {
    public class EventCenter : GameModule {
        public EventDispatcher Dispatcher;

        public async UniTask<GameProcedure> Init(CancellationTokenSource cts) {
            Dispatcher = new EventDispatcher(gameContext);
            return GameProcedure.Success;
        }

        protected override void OnDestroy() {
            if (Dispatcher != null) {
                Dispatcher.Destroy();
                Dispatcher = null;
            }
        }
    }
}
