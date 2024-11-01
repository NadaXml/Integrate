using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor.UI;
using Debug = UnityEngine.Debug;
namespace Trie {

    public interface IExtraData {
        
    }
    
    public interface ITrieNode<T> where T : class, IExtraData {
        ITrieNode<T> Parent { get; set; }
        int Depth { get; }
        Dictionary<char, ITrieNode<T>> Children { get; }
        T ExtraData { get; set; }
        char Value { get; }
        List<char> ToBufferString(List<char> prefix);
        bool IsEndOfWord { get; set; }
        void DumpNode();
    }
    
    public class TrieNodeBase<T> : ITrieNode<T> where T : class, IExtraData {

        public TrieNodeBase(char value, T extraData, int depth) {
            Value = value;
            ExtraData = extraData;
            Depth = depth;
            Children = new Dictionary<char, ITrieNode<T>>();
            IsEndOfWord = false;
        }
        public ITrieNode<T> Parent
        {
            get;
            set;
        }
        public int Depth
        {
            get;
        }
        public Dictionary<char, ITrieNode<T>> Children
        {
            get;
        }
        public T ExtraData
        {
            get;
            set;
        }
        public char Value
        {
            get;
        }
        public List<char> ToBufferString(List<char> prefix) {
            if (prefix == null) {
                List<char> buffer = new List<char>(256);
                buffer.Add(Value);
                return buffer; 
            }
            else {
                prefix.Add(Value);
                return prefix;
            }
        }
        public bool IsEndOfWord
        {
            get;
            set;
        }
        
        public void DumpNode() {
            StringBuilder builder = new StringBuilder();
            var iterationNode = this;
            
            Stack<(ITrieNode<T>, List<char>)> stack = new Stack<(ITrieNode<T>, List<char>)>();
            stack.Push((iterationNode, iterationNode.ToBufferString(null)));

            while (stack.Count > 0) {
                var node = stack.Pop();
                for (int i = 0; i < node.Item1.Depth * 4; i++) {
                    builder.Append(" ");
                }
                if (node.Item1.Value == char.MinValue) {
                    builder.Append($"|-- root - {node.Item1.Depth}\n");
                }
                else {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(new char[] {node.Item1.Value});
                    var nodeShow = Encoding.UTF8.GetString(bytes);
                    builder.Append($"|-- {nodeShow} - {node.Item1.Depth}\n");
                }
                
                foreach (KeyValuePair<char,ITrieNode<T>> valuePair in node.Item1.Children) {
                    stack.Push((valuePair.Value, node.Item1.ToBufferString(node.Item2)));
                }
            }
            
            Debug.Log(builder.ToString());
        }
    }
}
