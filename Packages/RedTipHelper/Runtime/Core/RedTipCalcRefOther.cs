using System.Collections.Generic;
using UnityEngine;

namespace RedTipHelper.Core {
    public class RedTipCalcRefOther : RedTipCalc {

        List<string> _refKeys;
        RelationType _relation;
        
        public RedTipCalcRefOther(IRedTipContext context, RedTipBase caller) : base(context, caller) {
            _refKeys = new List<string>();
            _relation = RelationType.OR;
            _calcType = CalcType.RefOther;
        }

        public void SetRef(string[] keys) {
            if (keys == null) {
                return;
            }
            _refKeys.AddRange(keys);
            foreach (var key in keys) {
                _context.RegisterRefObserver(key, Caller.Key);
            }
        }

        public void ResetRef() {
            if (_refKeys == null) {
                return;
            }
            foreach (string refKey in _refKeys) {
                _context.UnRegisterObserver(refKey, Caller.Key);
            }
        }

        public void RemoveRef(string key) {
            if (_refKeys.Remove(key)) {
                _context.UnRegisterObserver(key, Caller.Key);
            }
        }
        
        public override void Calc() {
            var final = _relation == RelationType.AND;
            foreach (var refKey in _refKeys) {
                var redTip = _context.GetRedTip(refKey);
                if (redTip != null) {
                    if (_relation == RelationType.OR) {
                        final = final || redTip.GetActive();
                    }
                    else {
                        final = final && redTip.GetActive();
                    }
                }
                else {
                    // 正常情况，目前节点删除，不会删除关联
                    Debug.Log($"节点引用，找不到关联{Caller.Key} ref {refKey}");
                }
            }
            IsActive = final;
        }

        public override void OnDestroy() {
            ResetRef();
            _refKeys = null;
        }
    }
}
