using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using UnityEditor;
using UnityEditor.UI;
namespace Core {
    public class RedTipBase : IRedTipLifecycle {

        string _location;
        public string Location {
            get {
                if (_location == null) {
                    StringBuilder builder = new StringBuilder();
                    List<string> arr = new List<string>();

                    var iterParent = Parent;
                    while (iterParent != null) {
                        arr.Add(iterParent.Key);
                        iterParent = iterParent.Parent;
                    }
                    arr.Reverse();
                    builder.AppendJoin(".", arr);
                    _location = builder.ToString();
                }
                return _location;
            }
        }
        public string Key;
        public IRedTipCalc ValCalc;
        public RedTipBase Parent;
        public List<RedTipBase> Children;
        public IRedTipContext Context { get; private set; }
        
        bool _childrenDirty;
        int _childrenActiveNum;
        string[] _refKeys;
        bool _isActive;

        RedTipBase(string key, IRedTipContext context) {
            Key = key;
            Context = context;
            _childrenActiveNum = 0;
            _childrenDirty = false;
            _isActive = false;
        }
        public void Awake() {
            OnAwake();
        }
        public void Start() {
            OnStart();
        }
        public void Destroy() {
            if (ValCalc != null) {
                ValCalc.Destroy();
                ValCalc = null;
            }
            OnDestroy();
        }
        public void Calc() {
            OnCalc();
        }
        public void CalcSchedule() {
            Context.AddToRefreshSchedule(this);
        }

        void TriggerRef() {
            CalcSchedule();
        }

        bool GetActive(bool isOld = false) {
            var ownActive = false;
            if (ValCalc != null) {
                ownActive = ValCalc.GetActive();
            }
            if ( !isOld ) {
                TryChildrenDirty();
            }
            return ownActive || _childrenActiveNum > 0;
        }

        public string DumpStr() {
            return null;
        }

        void MarkChildrenDirty() {
            _childrenDirty = true;
        }

        void TryChildrenDirty() {
            if (!_childrenDirty) {
                return;
            }
            if (Children != null) {
                _childrenActiveNum = 0;
                foreach (RedTipBase child in Children) {
                    if (child.GetActive()) {
                        _childrenActiveNum++;
                    }
                }
            }
            _childrenDirty = false;
        }

        string[] GetRefKeys() {
            // TODO 从配置读取
            return null;
        }
        
        public virtual void OnAwake() {}
        public virtual void OnStart() {}
        public virtual void OnCalc() {}
        public virtual void OnDestroy() {}
    }
}
