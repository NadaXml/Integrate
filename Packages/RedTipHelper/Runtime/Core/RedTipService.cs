
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RedTipHelper.Config;
using System.ComponentModel.Composition.Hosting;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.UI;

namespace RedTipHelper.Core {
    public class RedTipService : IRedTipContext, IRedTipService {

        public static bool IsDevMode = true;
        
        public Dictionary<string, List<string>> RefObservers
        {
            get;
            set;
        }
        
        IRedTipSchedule _redTipSchedule;
        public IRedTipSchedule RefreshSchedule
        {
            get {
                return _redTipSchedule;
            }
        }

        public IList<string> GetRefKeys(string key) {
            if (_redTipRelation.RefRelation.TryGetValue(key, out List<string> keys)) {
                return keys;
            }
            return null;
        }
            
        public RedTipBase GetRedTip(string key) {
            var code = HashCode(key);
            if (_redTipLUT.TryGetValue(code, out RedTipBase redTip)) {
                return redTip;
            }
            return null;
        }
        public void RegisterRefObserver(string subject, string observer) {
            Debug.Log($"reg sub {subject} obs {observer}");
            if (RefObservers.TryGetValue(subject, out List<string> observers)) {
                observers.Add(observer);
            }
            else {
                observers = new List<string>();
                if (-1 == observers.IndexOf(observer)) {
                    observers.Add(observer);
                    RefObservers.Add(subject, observers);
                }
                else {
                    throw new Exception(string.Format("重复插入subject {0} <- observer {1} ", subject, observer));
                }
            }
        }
        public void UnRegisterObserver(string subject, string observer) {
            if (RefObservers.TryGetValue(subject, out List<string> observers)) {
                int index = observers.IndexOf(observer);
                if (index != -1) {
                    observers.RemoveAt(index);
                }
                else {
                    Debug.LogErrorFormat("删除失败 subject {0} <- observer {1}", subject, observer);
                }
            }
            else {
                Debug.LogErrorFormat("删除失败 subject {0} <- observer {1}", subject, observer);
            }
        }
        public void TriggerRef(string subject) {
            if (RefObservers.TryGetValue(subject, out List<string> observers)) {
                foreach (string observer in observers) {
                    var redTip = GetRedTip(observer);
                    if (redTip != null) {
                        redTip.TriggerRef();
                    }
                    else {
                        Debug.Log(string.Format("触发关联刷新的时候找不到 subject {0} -> {1} ", subject, observer));
                    }
                }
            }
        }
        public void AddToRefreshSchedule(RedTipBase redTip) {
            _redTipSchedule.AddRedTip(redTip);
        }

        static Dictionary<string, Func<string, IRedTipContext, RedTipBase>> CreateMapping 
            = new Dictionary<string, Func<string, IRedTipContext, RedTipBase>>();

        RedTipConst _redTipSource;
        RedTipRelation _redTipRelation;
        public void SetSource(RedTipConst source, RedTipRelation relation) {
            _redTipSource = source;
            _redTipRelation = relation;
        }
        public static void BindCreate(string key, Func<string, IRedTipContext, RedTipBase> fun) {
            CreateMapping[key] = fun;
        }
        
        RedTipBase _rootRedTip;
        Dictionary<int, RedTipBase> _redTipLUT;
        
        public void Init() {
            RefObservers = new Dictionary<string, List<string>>();
            _redTipSchedule = new RedTipSchedule();
            _redTipSchedule.Init();
            _redTipLUT = new Dictionary<int, RedTipBase>();
            CheckHashCode();
        }
        
        
        public void UnInit() {
            if (_redTipSchedule != null) {
                _redTipSchedule.UnInit();
                _redTipSchedule = null;
            }
            _rootRedTip = null;
            RefObservers.Clear();
            foreach (KeyValuePair<int,RedTipBase> keyValuePair in _redTipLUT) {
                keyValuePair.Value.Destroy();
            }
            _redTipLUT.Clear();
        }
        public void Start() {
            _redTipSchedule.Start();
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
            stack.Push(_rootRedTip);
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

            foreach (KeyValuePair<string,List<string>> keyValuePair in _redTipRelation.Relation) {
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

            var rootCode = HashCode(_redTipSource.Root);
            _rootRedTip = _redTipLUT[rootCode];

            if (IsDevMode) {
                CheckCyclic();
            }
        }

        // TODO 微调了，不知道对不对
        void CheckCyclic() {
            Stack<string> trackPath = new Stack<string>();
            Dictionary<string, int> set = new Dictionary<string, int>();
            bool isCyclic = CheckCyclicImp(_rootRedTip, set, trackPath);
            if (isCyclic) {
                StringBuilder builder = new StringBuilder();
                builder.AppendJoin("->", trackPath.ToArray().Reverse());
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
                    trackPath.Push(node.Key);
                    Debug.Log(string.Format("节点引用，或者关联错误， 产生环的节点 {0}", node.Key));
                    return true;
                }
                if (color == 1) {
                    return false;
                }
                Debug.Log(string.Format("奇怪的颜色 {0} {1}", node.Key, color));
                return false;
            }
            bool isCyclic = false;
            set[node.Key] = 0;
            trackPath.Push(node.Key);
            if (node.Children != null) {
                foreach (RedTipBase redTipBase in node.Children) {
                    isCyclic = CheckCyclicImp(redTipBase, set, trackPath);
                    if (isCyclic) {
                        break;
                    }
                }
            }
            if (isCyclic) {
                return true;
            }
            
            var refKeys = _redTipRelation.RefRelation;
            if (refKeys.TryGetValue(node.Key, out List<string> refResult)) {
                foreach (string s in refResult) {
                    int code = HashCode(s);
                    if (_redTipLUT.TryGetValue(code, out RedTipBase redTipBase)) {
                        isCyclic = CheckCyclicImp(redTipBase, set, trackPath);
                        if (isCyclic) {
                            break;
                        }
                    }
                }
            }

            if (isCyclic) {
                return true;
            }
            trackPath.Pop();
            set[node.Key] = 1;
            return false;
        }

        bool CheckHashCode() {
            Dictionary<int, string> set = new Dictionary<int, string>();
            foreach (string key in _redTipSource.GetKeys()) {
                int code = HashCode(key);
                if (set.TryGetValue(code, out string value)) {
                    throw new Exception($" hash code 冲突 {value}, {key}");
                }
            }
            return true;
        }

        
        RedTipBase CreateRedTip(string key) {
            if (CreateMapping.TryGetValue(key, out Func<string, IRedTipContext, RedTipBase> fun)) {
                return fun.Invoke(key, this);
            }
            return null;
        }

        void ConnectRedTip(RedTipBase parent, RedTipBase child) {
            if (child.Parent != null) {
                throw new Exception(string.Format("有节点存在多个父亲，不是合法的树 \n old parent: {0} \n new parent: {1} \n child: {2}",
                    child.Parent.Key, parent.Key, child.Key));
            }
            child.Parent = parent;
            if (parent.Children == null) {
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
            bool isFirst = false;
            while (stack.Count > 0) {

                var tu = stack.Pop();
                if (tu.Item1 != null) {
                    if (!isFirst) {
                        isFirst = true;
                    }
                    else {
                        builder.Append(';');
                    }
                    builder.Append(tu.Item1.DumpStr());
                    
                    stack.Push((null, "#"));
                    
                    if (tu.Item1.Children != null) {
                        foreach (RedTipBase redTipBase in tu.Item1.Children) {
                            stack.Push((redTipBase, null));
                        }
                    }
                    stack.Push((null, "$"));
                }
                else {
                    if (!isFirst) {
                        isFirst = true;
                    }
                    else {
                        builder.Append(';');
                    }
                    builder.Append(tu.Item2);
                }
            }
        }

        public void AddRedTip(string parentKey, string key) {
            if (GetRedTip(key) == null) {
                Debug.LogError($"重复插入 {key}");
                return;
            }
            var redTip = CreateRedTip(key);
            redTip.Awake();
            var parentRedTip = GetRedTip(parentKey);
            if (parentRedTip == null) {
                Debug.LogError($"父节点找不到 插入失败 {key}");
                return;
            }
            ConnectRedTip(parentRedTip, redTip);
            var code = HashCode(key);
            _redTipLUT[code] = redTip;
        }

        public void RemoveRedTip(string key) {
            var redTip = GetRedTip(key);
            if (redTip == null) {
                Debug.LogError($"删除不存在的节点{key}");
                return;
            }
            var parentRedTip = redTip.Parent;
            if (parentRedTip == null) {
                Debug.LogError($"根节点只能通过service清理删除{key}");
                return;
            }

            if (parentRedTip.Children != null) {
                var num = parentRedTip.Children.RemoveAll((t) => t.Key == key);
                if (num > 0) {
                    redTip.Destroy();
                    var code = HashCode(redTip.Key);
                    _redTipLUT.Remove(code);
                    parentRedTip.MarkChildrenDirty();
                    parentRedTip.CalcSchedule();
                    
                    // 删除节点引用关系,如果量很大，再增加双向映射
                    var refRelation = _redTipRelation.RefRelation;
                    foreach (KeyValuePair<string,List<string>> keyValuePair in refRelation) {
                        bool isHit = false;
                        foreach (var k in keyValuePair.Value) {
                            if (k == redTip.Key) {
                                isHit = true;
                                break;
                            }
                        }
                        if (isHit) {
                            var redTipRefed = GetRedTip(keyValuePair.Key);
                            if (redTipRefed != null) {
                                var calc = redTipRefed.ValCalc as RedTipCalcRefOther;
                                if (calc != null) {
                                    calc.RemoveRef(keyValuePair.Key);
                                }
                                else {
                                    Debug.LogError("ValCalc 类型错误");
                                }
                            }
                            else {
                                Debug.LogError("找不到关联节点");
                            }
           
                            break;
                        }
                    }
                }
            }
        }

        public void AddRedTipDict<T>(string key, T id) where T : struct {
            var redTip = GetRedTip(key);
            if (redTip == null || redTip.ValCalc == null) {
                return;
            }
            RedTipCalcDict<T> calc = redTip.ValCalc as RedTipCalcDict<T>;
            if (calc != null) {
                if (calc.AddKey(id)) {
                    redTip.CalcSchedule();
                }
            }
            else {
                Debug.LogError($"{key} calc RedTipCalcDict类型调用错误");
            }
        }

        public void RemoveRedTipDict<T>(string key, T id) where T : struct {
            var redTip = GetRedTip(key);
            if (redTip == null || redTip.ValCalc == null) {
                return;
            }

            RedTipCalcDict<T> calc = redTip.ValCalc as RedTipCalcDict<T>;
            if (calc == null) {
                if (calc.RemoveKey(id)) {
                    redTip.CalcSchedule();
                }
            }
            else {
                Debug.LogError($"{key} calc RedTipCalcDict类型调用错误");
            }
        }
    }
}
