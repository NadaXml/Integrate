using asset_service;
using Cysharp.Threading.Tasks;
using game_logic;
using System;
using UnityEngine;
namespace SRSandBox.Scripts {
    public class Launcher : MonoBehaviour {

        public Asset.AssetParam AssetParam;
        
        GameApp app;
        void Start() {
            app = new GameApp();
            app.Start(AssetParam).Forget();
        }

        void OnDestroy() {
            app?.End();
            app = null;
        }
    }
}
