using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
namespace game_logic.module {
    public class EventDispatcher {

        GameContext gameContext;

        public EventDispatcher(GameContext gameContext) {
            this.gameContext = gameContext;
            gameContext.poolService.poolEventData.RegCreator(CrateDelegateData);
        }

        public EventData CrateDelegateData() {
            return new EventData();
        }
        
        readonly Dictionary<int, EventData> eventTable = new Dictionary<int, EventData>();

        public bool AddEventListener(int eventType, Action<EventParam> handler) {
            if (!eventTable.TryGetValue(eventType, out EventData data)) {
                data = gameContext.poolService.poolEventData.Acquire() as EventData;
                eventTable.Add(eventType, data);
            }
            else {
                data.AddHandler(handler);
            }
            return true;
        }

        public void RemoveEventListener(int eventType, Action<EventParam> handler) {
            if(eventTable.TryGetValue(eventType, out EventData data)) {
                data.RmvHandler(handler);
            }
        }

        public void Send(int eventType, EventParam param) {
            if (eventTable.TryGetValue(eventType, out EventData data)) {
                data.Invoke(param);
            }
        }

        public void Destroy() {
            foreach (KeyValuePair<int,EventData> pair in eventTable) {
                var value = pair.Value;
                gameContext.poolService.poolEventData.Release(value);
            }
            eventTable.Clear();
        }
    }
}
