using System;
using System.Collections.Generic;
using UnityEngine;
namespace RedTipHelper.Core {
    public class RedTipCalcDict<T> : RedTipCalc 
        where T : struct {

        Func<T, bool> _fun;
        Dictionary<T, CalcState> _state;
        RelationType _relation;
        
        public RedTipCalcDict(IRedTipContext context, RedTipBase caller, Func<T, bool> fun) : base(context, caller) {
            _fun = fun;
            _state = new Dictionary<T, CalcState>();
            _relation = RelationType.OR;
            _calcType = CalcType.Dict;
        }

        
        public void SetKeys(IEnumerable<T> keys) {
            _state.Clear();
            foreach (T key in keys) {
                _state[key] = CalcState.NoActive;
            }
        }

        public void ResetKeys() {
            _state.Clear();
        }

        public bool AddKey(T key) {
            if (_state.ContainsKey(key)) {
                Debug.LogError("重复添加 " + key);
                return false;
            }
            _state.Add(key, CalcState.NoActive);
            return true;
        }

        public bool RemoveKey(T key) {
            if (_state.ContainsKey(key)) {
                return _state.Remove(key);
            }
            return false;
        }

        public bool GetActiveDict(T key) {
            if (_state.TryGetValue(key, out CalcState state)) {
                return state == CalcState.Active;
            }
            return false;
        }
        
        public override void Calc() {
            bool final = _relation == RelationType.AND;
            foreach (KeyValuePair<T,CalcState> keyValuePair in _state) {
                var state = _fun.Invoke(keyValuePair.Key);
                if (_relation == RelationType.OR) {
                    final = final || state;
                } else if (_relation == RelationType.AND) {
                    final = final && state;
                }
                if (state) {
                    _state[keyValuePair.Key] = CalcState.Active;
                }
                else {
                    _state[keyValuePair.Key] = CalcState.NoActive;
                }
            }
            IsActive = final;
        }

        public override void OnDestroy() {
            ResetKeys();
        }
    }
}
