using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;

namespace Trie {
    
    public class Trie<T> where T : class, IExtraData {
        public ITrieNode<T> Root
        {
            get;
            protected set;
        }

        public Trie() {
            Root = CreateNode<T>(Char.MinValue, null);
        }

        public ITrieNode<T> FindWithInsert(string str, T extraData = null) {
            var iterationNode = Root;
            foreach (char c in str) {
                if (iterationNode.Children.TryGetValue(c, out ITrieNode<T> child)) {
                    iterationNode = child;
                }
                else {
                    iterationNode = CreateNode<T>(c, iterationNode);
                 
                }
            }
            if (iterationNode != Root) {
                iterationNode.IsEndOfWord = true;
                iterationNode.ExtraData = extraData;
            }
            return iterationNode;
        }

        public ITrieNode<T> Search(string str) {
            ITrieNode<T> resultNode = null;
            var iterationNode = Root;
            foreach (char c in str) {
                if (iterationNode.Children.TryGetValue(c, out ITrieNode<T> node)) {
                    resultNode = node;
                    iterationNode = node;
                }
                else {
                    resultNode = null;
                    break;
                }
            }
            return resultNode;
        }

        public bool IsSearch(string str) {
            var node = Search(str);
            return node != null && node.IsEndOfWord;
        }

        public void Insert(string str, T extraData = null) {
            var iterationNode = Root;
            foreach (char c in str) {
                if (!iterationNode.Children.TryGetValue(c, out var child)) {
                    child = CreateNode<T>(c, iterationNode);
                    
                }
                iterationNode = child;
            }
            if (iterationNode != Root) {
                iterationNode.IsEndOfWord = true;
                iterationNode.ExtraData = extraData;
            }
        }

        public IList<T> GetPrefix(string str) {
            // 找到节点
            ITrieNode<T> resultNode = Search(str);
            if (resultNode == null) {
                return null;
            }
            // resultNode.DumpNode();
            return GetAllChildExtraData(resultNode);
        }

        public IList<char[]> GetPrefixNames(string str) {
            ITrieNode<T> resultNode = Search(str);
            if (resultNode == null) {
                return null;
            }
            // resultNode.DumpNode();
            return GetAllChildrenName(resultNode);
        }

        public IList<T> GetAllChildExtraData(ITrieNode<T> node) {
            IList<T> result = new List<T>();
            Stack<ITrieNode<T>> stack = new Stack<ITrieNode<T>>();
            stack.Push(node);
            while ( stack.Count > 0) {
                var iterationNode = stack.Pop();

                if (iterationNode.ExtraData != null) {
                    result.Add(iterationNode.ExtraData);
                }

                foreach (KeyValuePair<char,ITrieNode<T>> valuePair in iterationNode.Children) {
                    stack.Push(valuePair.Value);
                }
            }
            return result;
        }

        public IList<char[]> GetAllChildrenName(ITrieNode<T> node) {
            List<char[]> result = new List<char[]>();
            Stack<(ITrieNode<T>, List<char>)> stack = new Stack<(ITrieNode<T>, List<char>)>();
            stack.Push((node, new List<char>()));

            while (stack.Count > 0) {
                var valTurple = stack.Pop();
                var t = valTurple.Item1;
                var l = valTurple.Item2;
                l.Add(t.Value);

                if (t.IsEndOfWord) {
                    result.Add(l.ToArray());
                }

                foreach (KeyValuePair<char,ITrieNode<T>> valuePair in t.Children) {
                    var childL = new List<char>();
                    childL.AddRange(l);
                    stack.Push((valuePair.Value, childL));
                }
            }
            return result;
        }

        [ItemCanBeNull]
        public IList<ITrieNode<T>> SearchIncludeTreeNode(string str) {
            // 创建搜索的子树
            var findRoot = CreateNode<T>(Char.MinValue, null);
            var iterationNode = findRoot;
            foreach (char c in str) {
                if (!iterationNode.Children.TryGetValue(c, out var child)) {
                    child = CreateNode<T>(c, iterationNode);
                }
                iterationNode = child;
            }

            var fr = new List<ITrieNode<T>>();
            IsIncludeStr(Root, str, fr);
            return fr;
        }

        public IList<T> SearchIncludeTree(string str) {
            IList<ITrieNode<T>> fr = SearchIncludeTreeNode(str);

            IList<T> ls = new List<T>();
            foreach (ITrieNode<T> n in fr) {
                var d = GetAllChildExtraData(n);
                ls.AddRange(d);
            }
            return ls;
        }

        public IList<string> SearchIncludeTreeNodeNames(string str) {
            IList<ITrieNode<T>> fr = SearchIncludeTreeNode(str);
            
            IList<string> ls = new List<string>();
            foreach (ITrieNode<T> n in fr) {
                var d = GetAllChildrenName(n);
                char[] prefix = new char[n.Depth];
                var prefixNode = n.Parent;
                int index = 0;
                while (prefixNode != null && prefixNode.Depth >= 0) {
                    prefix[prefix.Length - index - 1] = prefixNode.Value;
                    index++;
                    prefixNode = prefixNode.Parent;
                }
                foreach (char[] chars in d) {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(prefix);
                    builder.Append(chars);
                    ls.Add(builder.ToString());
                }
            }
            return ls;
        }

        void IsIncludeStr(ITrieNode<T> node, string str, List<ITrieNode<T>> fr) {
            if (node == null || str == string.Empty) {
                return;
            }
            if (IsNodeIncludeStr(node, str, 0)) {
                fr.Add(node);
            }

            foreach (KeyValuePair<char,ITrieNode<T>> keyValuePair in node.Children) {
                IsIncludeStr(keyValuePair.Value, str, fr);
            }
        }

        bool IsNodeIncludeStr(ITrieNode<T> node, string str, int index) {
            if (index < str.Length && node.Value != str[index]) {
                return false;
            }

            foreach (KeyValuePair<char,ITrieNode<T>> keyValuePair in node.Children) {
                if (!IsNodeIncludeStr(keyValuePair.Value, str, index + 1)) {
                    return false;
                }
            }
            return true;
        }

        static ITrieNode<T> CreateNode<T>(char value, ITrieNode<T> parent) where T : class, IExtraData {
            int depth = -1;
            if (parent != null) {
                depth = parent.Depth + 1;
            }
            var node = new TrieNodeBase<T>(value, null, depth);
            if (parent != null) {
                parent.Children.Add(value, node);
                node.Parent = parent;
            }
            return node;
        }
    }
}
