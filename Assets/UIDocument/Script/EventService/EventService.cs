using AppFrame;
using System;
using System.Collections;
using System.Collections.Generic;
namespace UIDocument.Script.EventService {
    
    public interface EventServiceProvider {
        EventService GetEventService();
    }
    
    public abstract class GameEventBase : EventArgs {
        public int EventId { get; set; }
    }
    
    public class EventService : IService {

        public Dictionary<string, EventHandler<GameEventBase>> _eventRouter = new Dictionary<string, EventHandler<GameEventBase>>();
        
        public void Awake() {
        }
        public void Destroy() {
            
        }
        public IEnumerator Start() {
            yield return null;
        }
        
        public void RegisterEvent(string eventName, EventHandler<GameEventBase> handler) {
            if (_eventRouter.ContainsKey(eventName)) {
                _eventRouter[eventName] += handler;
            }
            else {
                _eventRouter[eventName] = handler;
            }
        }
        
        public void UnRegisterEvent(string eventName, EventHandler<GameEventBase> handler) {
            if (_eventRouter.ContainsKey(eventName)) {
                _eventRouter[eventName] -= handler;
            }
        }
        
        public void TriggerEvent(ISystem system, string eventName, GameEventBase e) {
            if (_eventRouter.ContainsKey(eventName)) {
                _eventRouter[eventName]?.Invoke(system, e);
            }
        }
    }
}
