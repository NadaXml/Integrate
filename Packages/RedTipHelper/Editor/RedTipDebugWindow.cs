using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace RedTipDebug.Editor {
    public class RedTipDebugWindow : EditorWindow {
        RedTipTreeView _treeView;
        [SerializeField] TreeViewState _treeViewState;
        [SerializeField] MultiColumnHeaderState _multiColumnHeaderState;
        SearchField m_SearchField;

        Rect toolbarRect {
            get { return new Rect (20f, 10f, position.width-40f, 20f); }
        }

        Rect multiColumnTreeViewRect
        {
            get { return new Rect(20, 30, position.width-40, position.height-80); }
        }

        Rect bottomToolbarRect
        {
            get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); }
        }

        void OnEnable() {
            if (_treeViewState == null) {
                _treeViewState = new TreeViewState();
            }

            var headerState = RedTipTreeView.CreateMultiColumnHeaderState(multiColumnTreeViewRect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(_multiColumnHeaderState, headerState)) {
                MultiColumnHeaderState.OverwriteSerializedFields(_multiColumnHeaderState, headerState);
            }
            _multiColumnHeaderState = headerState;
            var multiColumnHeader = new RedTipTreeViewColumnHeader(headerState);
            multiColumnHeader.ResizeToFit();

            var treeModel = new TreeModel<RedTipTreeElement>(GetDataFromLua());
            _treeView = new RedTipTreeView(_treeViewState, multiColumnHeader, treeModel);

            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        const string ChildPrefix = "$";
        const string ChildSuffix = "#";
        IList<RedTipTreeElement> GetDataFromLua() {

            List<RedTipTreeElement> treeElement = new List<RedTipTreeElement>();
            int iterDepth = -1;
            var root = new RedTipTreeElement("Root", false, iterDepth, "Root".GetHashCode());
            treeElement.Add(root);
            ++iterDepth;


            if (GetShowGraphStr == null) {
                return treeElement;
            }

            string content = GetShowGraphStr.Invoke();
            if (string.IsNullOrEmpty(content)) {
                return treeElement;
            }

            string[] symbols = content.Split(';');
            foreach (var symbol in symbols) {
                string a = symbol;
                switch (a) {
                    case ChildPrefix: {
                        ++iterDepth;
                    }
                        break;
                    case ChildSuffix: {
                        --iterDepth;
                    }
                        break;
                    default: {
                        string[] pair = a.Split(':');
                        int val = Convert.ToInt32(pair[1]);
                        string name = pair[0];
                        var child = new RedTipTreeElement(name, val == 1 ? true : false, iterDepth, name.GetHashCode());
                        treeElement.Add(child);
                    }
                        break;
                }
            }
            return treeElement;
        }

        void OnGUI() {
            SearchBar (toolbarRect);
            _treeView.OnGUI(multiColumnTreeViewRect);
            BottomToolbar(bottomToolbarRect);
        }
        void SearchBar(Rect rect) {
            _treeView.searchString = m_SearchField.OnGUI(rect, _treeView.searchString);
        }

        void BottomToolbar(Rect rect) {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope()) {

                var style = "miniButton";
                if (GUILayout.Button("ShowTree", style)) {
                    ShowTree();
                }
            }

            GUILayout.EndArea();
        }

        void ShowTree() {
            _treeView.treeModel.SetData(GetDataFromLua());
            _treeView.Reload();
        }

        public static event Func<string> GetShowGraphStr;

        [MenuItem ("RedTipRRTemplate/DebugView")]
        static void ShowWindow ()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<RedTipDebugWindow> ();
            window.titleContent = new GUIContent ("DebugView");
            window.Show ();
        }
    }
}
