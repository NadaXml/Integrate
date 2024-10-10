using Config;
using dnlib.DotNet.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Animations;
namespace Core {
    public class RedTipService : IRedTipContext, IRedTipService {

        public Dictionary<string, string> RefObservers
        {
            get;
        }
        public IRedTipSchedule RefreshSchedule
        {
            get;
        }

        public static Dictionary<string, Func<RedTipBase>> CreateMapping = new Dictionary<string, Func<RedTipBase>>();

        public static void BindCreate(string key, Func<RedTipBase> fun) {
            CreateMapping[key] = fun;
        }
        
        IRedTipSchedule _redTipSchedule;
        RedTipBase _rootRedTip;
        Dictionary<int, RedTipBase> _redTipLUT;
        
        
        public RedTipBase GetRedTip() {
            throw new System.NotImplementedException();
        }
        public void RegisterRefObserver(RedTipBase subject, RedTipBase observer) {
            throw new System.NotImplementedException();
        }
        public void UnRegisterObserver(RedTipBase subject, RedTipBase observer) {
            throw new System.NotImplementedException();
        }
        public void TriggerRef(RedTipBase subject) {
            throw new System.NotImplementedException();
        }
        public void AddToRefreshSchedule(RedTipBase redTip) {
            throw new System.NotImplementedException();
        }
        public void Init() {
            _redTipLUT = new Dictionary<int, RedTipBase>();
            CheckHashCode();
        }
        
        
        public void UnInit() {
            if (_redTipSchedule != null) {
                _redTipSchedule.UnInit();
                _redTipSchedule = null;
            }
        }
        public void Start() {
            GenerateConfig();
            PreOrderStart();
        }
        public void Stop() {
            _redTipLUT.Clear();
            _redTipSchedule.Stop();
            _rootRedTip = null;
        }

        void PreOrderStart() {
            if (_rootRedTip == null) {
                return;
            }

            Stack<RedTipBase> stack = new Stack<RedTipBase>();
            stack.Append(_rootRedTip);

            while (stack.Count > 0) {

                RedTipBase redTip = stack.Pop();
                redTip.Start();

                if (redTip.Children != null) {
                    foreach (RedTipBase r in redTip.Children) {
                        stack.Push(r);
                    }
                }
            }
        }

        int HashCode(string key) {
            // 如果真冲突来，再换质数
            int seed = 131;  // 质数 -- 31 131 1313 13131 131313 etc..
            int hash = 0;
            for (int i = 1; i < key.Length; i++) {
                hash = hash * seed + key[i];
            }
            // 再换取模运算
            return hash & 0x7FFFFFFF;
        }

        void GenerateConfig() {
            _redTipLUT.Clear();

            foreach (KeyValuePair<string,List<string>> keyValuePair in RedTipRelation.Relation) {
                var code = HashCode(keyValuePair.Key);
                if (!_redTipLUT.TryGetValue(code, out RedTipBase parentRedTipBase)) {
                    parentRedTipBase = CreateRedTip(keyValuePair.Key);
                    parentRedTipBase.Awake();
                    
                    _redTipLUT[code] = parentRedTipBase;
                }

                foreach (string childKey in keyValuePair.Value) {
                    var childCode = HashCode(childKey);
                    if (!_redTipLUT.TryGetValue(childCode, out RedTipBase childRedTipBase)) {
                        childRedTipBase = CreateRedTip(childKey);
                        childRedTipBase.Awake();
                        _redTipLUT[childCode] = childRedTipBase;
                    }
                    ConnectRedTip(parentRedTipBase, childRedTipBase);
                }
            }

            var rootCode = HashCode(RedTipConst.Main);
            _rootRedTip = _redTipLUT[rootCode];

            CheckCyclic();
        }

        // TODO 微调了，不知道对不对
        void CheckCyclic() {
            Stack<string> trackPath = new Stack<string>();
            Dictionary<string, int> set = new Dictionary<string, int>();
            bool isCyclic = CheckCyclicImp(_rootRedTip, set, trackPath);
            if (isCyclic) {
                StringBuilder builder = new StringBuilder();
                builder.AppendJoin("->", trackPath.GetEnumerator());
                throw new Exception(" 存在环 环路 \n" + builder);
            }
        }

        // TODO 微调了，不知道对不对
        bool CheckCyclicImp(RedTipBase node, Dictionary<string,int> set, Stack<string> trackPath) {
            if (node == null) {
                return false;
            }
            
            if (set.TryGetValue(node.Key, out int color)) {
                if (color == 0) {
                    Debug.Log(string.Format("节点引用，或者关联错误， 产生环的节点 %s", node.Key));
                    return true;
                }
                if (color == 1) {
                    return false;
                }
                Debug.Log(string.Format("奇怪的颜色 %s %d", node.Key, color));
                return false;
            }
            bool isCyclic = false;
            set[node.Key] = 0;
            trackPath.Push(node.Key);
            if (node.Children != null) {
                foreach (RedTipBase redTipBase in node.Children) {
                    isCyclic = CheckCyclicImp(node, set, trackPath);
                    if (isCyclic) {
                        break;
                    }
                }
            }
            if (isCyclic) {
                return isCyclic;
            }
            var refKeys = RedTipRelation.RefRelation;
            foreach (KeyValuePair<string,List<string>> keyValuePair in refKeys) {
                int code = HashCode(keyValuePair.Key);
                if (_redTipLUT.TryGetValue(code, out RedTipBase redTipBase)) {
                    isCyclic = CheckCyclicImp(redTipBase, set, trackPath);
                    if (isCyclic) {
                        break;
                    }
                }
            }
            if (isCyclic) {
                return isCyclic;
            }
            trackPath.Pop();
            set[node.Key] = 1;
            return false;
        }

        bool CheckHashCode() {
            Dictionary<int, string> set = new Dictionary<int, string>();
            foreach (string key in RedTipConst.Keys) {
                int code = HashCode(key);
                if (set.ContainsKey(code)) {
                    throw new Exception($" hash code 冲突 {set[code]}, {key}");
                }
            }
            return true;
        }

        
        RedTipBase CreateRedTip(string key) {
            if (CreateMapping.TryGetValue(key, out Func<RedTipBase> fun)) {
                return fun.Invoke();
            }
            return null;
        }

        void ConnectRedTip(RedTipBase parent, RedTipBase child) {
            if (child.Parent != null) {
                
            }
            child.Parent = parent;
            if (parent.Children != null) {
                parent.Children = new List<RedTipBase>();
            }
            parent.Children.Add(child);
        }

        public string DumpActive() {
            StringBuilder builder = new StringBuilder();
            TreeToList(_rootRedTip, builder);
            string s = builder.ToString();
            Debug.Log(s);
            return s;
        }

        public void TreeToList(RedTipBase root, StringBuilder builder) {
            if (root == null) {
                return;
            }
            Stack<(RedTipBase, string)> stack = new Stack<(RedTipBase, string)>();
            stack.Push((root, null));
            while (stack.Count > 0) {

                var tu = stack.Pop();
                if (tu.Item1 != null) {
                    builder.Append(tu.Item1.Key);
                }
                else {
                    builder.Append(tu.Item2);
                }
                
                stack.Push((null, "#"));

                if (tu.Item1.Children != null) {
                    foreach (RedTipBase redTipBase in tu.Item1.Children) {
                        stack.Push((redTipBase, null));
                    }
                }
                
                stack.Push((null, "$"));
            }
        }
    }
}
