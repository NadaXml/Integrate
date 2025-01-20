using System;
using System.Collections;
using UIDocument.Script.App;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset;

namespace UIDocument.Script.Launcher {
    public class Launcher : MonoBehaviour {
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        public string PackageName = "DefaultPackage";
    
        public IApp App = null;

        void Awake() {
            App = new AppGame();
            App.Awake();
        }

        IEnumerator Start() {
            DontDestroyOnLoad(gameObject);

            GraphicsSettings.useScriptableRenderPipelineBatching = true;
            
            AppGame appGame = App as AppGame;
            appGame.SetPlayMode(PlayMode);

            yield return appGame.Start();
            
            appGame.Play();
        }

        // Update is called once per frame
        void Update() {
            App.Update(Time.deltaTime);
        }


        void OnDestroy() {
            App.Destroy();
        }
    }
}
