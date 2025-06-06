namespace game_logic {
    public class GameAppRunner {
        struct RunVar {
            public float accumulate;
        }
        
        GameApp app;
        RunVar runVar;
        
        public void Awake() {
            runVar.accumulate = 0;
        }

        public void Destroy() {
            
        }
        
        public void SetRunApp(GameApp app) {
            this.app = app;
        }
        
        public void Run(float delta) {
            float t = delta + runVar.accumulate;
            GameContext.RunParam runParam = app.GetRunParam();
            float dt = 1.0f / runParam.frameRate;
            while (t > dt) {
                t -= dt;
                app.Tick();
                app.SetFrameCount(runParam.frameCount + 1);
                // need break ?
            }
            if (t > 0) {
                runVar.accumulate = t;
            }
        }
    }
}
