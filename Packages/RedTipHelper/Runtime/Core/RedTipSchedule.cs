using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
namespace RedTipHelper.Core {
    public class RedTipSchedule : IRedTipSchedule {

        Queue<RedTipBase> _queue;
        
        public void Init() {
            _queue = new Queue<RedTipBase>();
        }
        public void UnInit() {
            if (_queue != null) {
                _queue.Clear();
                _queue = null;
            }
        }
        public void Start() {
            _queue.Clear();
        }
        public void Stop() {
            _queue.Clear();
        }
        public void AddRedTip(RedTipBase redTip) {
            _queue.Enqueue(redTip);
        }
        public void Schedule() {
            Stopwatch start = new Stopwatch();

            var num = 0;

            HashSet<string> changed = new HashSet<string>();

            while (start.ElapsedMilliseconds < 5 && num < 300) {
                if (_queue.Count <= 0) {
                    break;
                }
                var redTip = _queue.Dequeue();
                Debug.Log($"调度刷新 {redTip.Key}");
                if (redTip.Calc()) {
                    changed.Add(redTip.Key);
                    num++;
                }
            }
            
            // TODO 对外事件通知
        }
    }
}
