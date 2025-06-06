using adt;
using asset_service;
using Cysharp.Threading.Tasks;
using game_logic;
using System;
using UnityEngine;
namespace SRSandBox.Scripts {
    public class Launcher : MonoBehaviour {

        public Asset.AssetParam AssetParam;
        
        GameApp app;
        GameAppRunner runner;
        void Start() {
            app = new GameApp();
            runner = new GameAppRunner();
            runner.SetRunApp(app);
            app.Start(AssetParam).Forget();
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
