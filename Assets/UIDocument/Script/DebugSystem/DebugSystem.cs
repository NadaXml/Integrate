using AppFrame;
using System.Collections;
using IngameDebugConsole;
using Newtonsoft.Json;
using UIDocument.Script.Core.ADT;
using UIDocument.Script.EventService;
using UnityEngine;

namespace UIDocument.Script.DebugSystem {
    
    public class DebugSystem : ISystem {

        GameObject _inspectorGo;
        
        public void Update(float deltaTime) {
            // throw new System.NotImplementedException();
        }
        public void Awake() {
            // throw new System.NotImplementedException();
        }
        public void Destroy() {
            // throw new System.NotImplementedException();
            if (_inspectorGo != null) {
                Object.Destroy(_inspectorGo);
                _inspectorGo = null;
            }
        }
        public IEnumerator Start() {
           
            // 事件参数无返回值
            DebugLogConsole.AddCommand(EventNameDef.DumpRound, "dump round", () => {
                _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.DumpRound, new DumpRoundSystem());
            });
            yield return null;
            
            // 事件参数有返回值，通过事件返回，如果异步执行， 到时候再商量
            DebugLogConsole.AddCommand(EventNameDef.DumpRoundInspector, "dump_round_inspector", () => {
                var evt = new FetchRoundInspector();
                _context.EventServiceProvider.GetEventService().TriggerEvent(this, EventNameDef.DumpRoundInspector, evt);
                ShowRoundInspector(evt.Result);
            });
        }

        public void ShowRoundInspector(MoveComponentStream result) {
            if (_inspectorGo == null) {
                _inspectorGo = new GameObject();
                Object.DontDestroyOnLoad(_inspectorGo);
                _inspectorGo.AddComponent<DebugRoundMonobehaviour>();
            }
            
            var mono = _inspectorGo.GetComponent<DebugRoundMonobehaviour>();
            mono.components = result;
        }
        
        public struct CreateParam {
            public DebugSystemContext Context;
        }

        DebugSystemContext _context;

        public struct DebugSystemContext : ISystemContext {
            public EventService.EventServiceProvider EventServiceProvider;
        }

        public DebugSystem(in CreateParam param) {
            _context = param.Context;   
        }
    }
}
