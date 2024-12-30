using System.Collections.Generic;
using UnityEngine;
using File = System.IO.File;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Profiling;

namespace Trie {
    public class TrieTest : MonoBehaviour {
        void Start() {
            Application.targetFrameRate = 60;
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.K)) {
                // Debug.Log("Test1");
                // Test1();
                // Debug.Log("Test2");
                // Test2();
                // Debug.Log("Test3");
                // Test3();
                // Debug.Log("Test4");
                // Test4();
                // Debug.Log("Test5");
                // Test5();
                // Debug.Log("Test6");
                // Test6();
                // Debug.Log("Test7");
                // Test7();
                // Debug.Log("Test8");
                // Test8();
                Debug.Log("Test9");
                Test9();
                Debug.Log("Test10");
                Test10();
            }
        }

        Trie<Ids> TestInitA() {
            Trie<Ids> trie = new Trie<Ids>();
            string path = "Assets/Trie/testData.txt";
            string jsonStr = File.ReadAllText(path);
            var source = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonStr);
            foreach (KeyValuePair<string, List<int>> valuePair in source) {
                trie.Insert(valuePair.Key, new Ids() {
                    ids = valuePair.Value
                });
            }
            return trie;
        }

        Dictionary<string, List<int>> TestInitB() {
            string path = "Assets/Trie/testData.txt";
            string jsonStr = File.ReadAllText(path);
            var source = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jsonStr);
            return source;
        }

        void DumpIds(IList<Ids> results) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < results.Count; i++) {
                builder.AppendJoin<int>(",", results[i].ids);
                builder.AppendLine();
            }
            Debug.Log(builder);
        }
        
        void Test1() {
            Trie<Ids> trie = TestInitA();
            Profiler.BeginSample("Test1");
            // trie.Root.DumpNode();
            trie.IsSearch("六合");
            Profiler.EndSample();
        }

        void Test2() {
            Trie<Ids> trie = new Trie<Ids>();
            trie.Insert("六合");
            Profiler.BeginSample("Test2");
            // trie.Root.DumpNode();
            trie.IsSearch("六合");
            Profiler.EndSample();
        }

        void Test3() {
            Trie<Ids> trie = TestInitA();
            Profiler.BeginSample("Test3");
            // trie.Root.DumpNode();
            var ls = trie.GetPrefix("火");
            // DumpIds(ls);
            Profiler.EndSample();
        }

        void Test4() {
            Trie<Ids> trie = TestInitA();
            Profiler.BeginSample("Test4");
            // trie.Root.DumpNode();
            var names = trie.GetPrefixNames("火");
            // DumpNames(names);
            Profiler.EndSample();
        }
        void DumpNames(IList<char[]> names) {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (char[] charName in names) {
                builder.Append(num++);
                builder.Append(charName);
                builder.Append('\n');
            }
            Debug.Log(builder);
        }

        void DumpNames(IList<string> names) {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (string charName in names) {
                builder.Append(num++);
                builder.Append(':');
                builder.Append(charName);
                builder.Append('\n');
            }
            Debug.Log(builder);
        }

        void Test5() {
            var dict = TestInitB();
            StringBuilder nameBuilder = new StringBuilder();
            List<List<int>> results = new List<List<int>>();
            int num = 0;
            Profiler.BeginSample("Test5");
            foreach (KeyValuePair<string,List<int>> valuePair in dict) {
                if (valuePair.Key.StartsWith("火")) {
                    results.Add(valuePair.Value);
                    // nameBuilder.Append(num++);
                    // nameBuilder.Append(valuePair.Key);
                    // nameBuilder.Append('\n');
                }
            }
            Profiler.EndSample();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < results.Count; i++) {
                builder.AppendJoin<int>(",", results[i]);
                builder.AppendLine();
            }
            // Debug.Log(nameBuilder);
            // Debug.Log(builder);
        }

        void Test6() {
            Trie<Ids> trie = TestInitA();
            // Profiler.BeginSample("Test3");
            // trie.Root.DumpNode();
            var ls = trie.SearchIncludeTree("之");
            DumpIds(ls);
            // Profiler.EndSample();
        }

        void Test7() {
            Trie<Ids> trie = TestInitA();
            var names = trie.SearchIncludeTreeNodeNames("之");
            DumpNames(names);
        }

        void Test8() {
            var dict = TestInitB();
            StringBuilder nameBuilder = new StringBuilder();
            int num = 0;
            Profiler.BeginSample("Test8");
            foreach (KeyValuePair<string,List<int>> valuePair in dict) {
                if (valuePair.Key.Contains("之")) {
                    nameBuilder.Append(num++);
                    nameBuilder.Append(':');
                    nameBuilder.Append(valuePair.Key);
                    nameBuilder.Append('\n');
                }
            }
            Debug.Log(nameBuilder);
        }

        void Test9() {
            Trie<Ids> trie = TestInitA();
            Profiler.BeginSample("Test9");
            var results = trie.SearchIncludeTree("之");
            Profiler.EndSample();
        }

        void Test10() {
            var dict = TestInitB();
            List<List<int>> results = new List<List<int>>();
            Profiler.BeginSample("Test10");
            foreach (KeyValuePair<string,List<int>> valuePair in dict) {
                if (valuePair.Key.Contains("之")) {
                    results.Add(valuePair.Value);
                }
            }
            Profiler.EndSample();
        }
    }
}
