using System;
using System.Collections;
using UIDocument.Script.App;
using UnityEngine;
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

            AppGame appGame = App as AppGame;
            appGame.SetPlayMode(PlayMode);

            yield return appGame.Start();
            
            appGame.Play();
        }

        // Update is called once per frame
        void Update()
        {
        
        }


        void OnDestroy() {
            // var package = YooAssets.GetPackage("DefaultPackage");
            // package.ClearAllCacheFilesAsync()
            
            App.Destroy();
        }
    }
}
