using adt;
using asset_service;
using Cysharp.Threading.Tasks;
using game_logic;
using log_service;
using UnityEngine;

namespace SRSandBox.Scripts {
    public class Launcher : MonoBehaviour {

        public GameContext.AppConfig appConfig;
        
        GameApp app;
        GameAppRunner runner;
        void Start() {
            app = new GameApp();
            runner = new GameAppRunner();
            runner.SetRunApp(app);
            app.Start(appConfig).Forget();
        }

        void OnDestroy() {
            app?.End();
            app = null;
        }

        void Update() {
            if (app.GetGameProcedure() != GameProcedure.Success) {
                return;
            }
            runner.Run(Time.deltaTime);
        }
    }
}
