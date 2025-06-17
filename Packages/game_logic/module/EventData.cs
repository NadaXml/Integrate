using game_service.pool_service;
using System;
using System.Collections.Generic;
namespace game_logic.module {
    public class EventData : IReference {
        
        public int EventType;
        List<Action<EventParam>> waitAddHandlers;
        List<Action<EventParam>> existHandlers;
        List<Action<EventParam>> waitRmvHandlers;
        bool IsInvoking;
        bool IsDirty;

        public void AddHandler(Action<EventParam> handler) {
            if (IsInvoking) {
                IsDirty = true;
                waitAddHandlers.Add(handler);
            }
            else {
                existHandlers.Add(handler);
            }
        }

        public void RmvHandler(Action<EventParam> handler) {
            if (IsInvoking) {
                IsDirty = true;
                waitRmvHandlers.Add(handler);
            }
            else {
                existHandlers.Remove(handler);
            }
        }
        
        public void Invoke(EventParam eventParam) {
            IsInvoking = true;
            for (int i = 0; i < existHandlers.Count; i++) {
                var handler = existHandlers[i];
                handler.Invoke(eventParam);
            }
            CheckModify();
        }

        public void CheckModify() {
            IsInvoking = false;
            if (IsDirty) {
                // 删除
                if (waitRmvHandlers.Count > 0) {
                    foreach (Action<EventParam> handler in waitRmvHandlers) {
                        existHandlers.Remove(handler);
                    }
                    waitRmvHandlers.Clear();
                }
            
                // 添加
                if (waitAddHandlers.Count > 0) {
                    foreach (Action<EventParam> handler in waitAddHandlers) {
                        existHandlers.Add(handler);
                    }
                    waitAddHandlers.Clear();
                }
                IsDirty = false;
            }
        }
        public void ClearForPool() {
            EventType = 0;
            existHandlers.Clear();
            waitAddHandlers.Clear();
            waitRmvHandlers.Clear();
            IsDirty = false;
            IsInvoking = false;
        }
    }
}
