using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace RedTipDebug.Editor {

    public class RedTipTreeViewColumnHeader : MultiColumnHeader
    {
        Mode m_Mode;

        public enum Mode
        {
            LargeHeader,
            DefaultHeader,
            MinimumHeaderWithoutSorting
        }

        public RedTipTreeViewColumnHeader(MultiColumnHeaderState state)
            : base(state)
        {
            mode = Mode.DefaultHeader;
        }

        public Mode mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.LargeHeader:
                        canSort = true;
                        height = 37f;
                        break;
                    case Mode.DefaultHeader:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                    case Mode.MinimumHeaderWithoutSorting:
                        canSort = false;
                        height = DefaultGUI.minimumHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderGUI (MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            // Default column header gui
            base.ColumnHeaderGUI(column, headerRect, columnIndex);

            // Add additional info for large header
            if (mode == Mode.LargeHeader)
            {
                // Show example overlay stuff on some of the columns
                if (columnIndex > 2)
                {
                    headerRect.xMax -= 3f;
                    var oldAlignment = EditorStyles.largeLabel.alignment;
                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                    GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
                    EditorStyles.largeLabel.alignment = oldAlignment;
                }
            }
        }
    }

    public class RedTipTreeView : TreeViewWithTreeModel<RedTipTreeElement> {

        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;
        public bool showControls = true;

        public RedTipTreeView(TreeViewState state, TreeModel<RedTipTreeElement> model) : base(state, model) {
        }
        public RedTipTreeView(TreeViewState state, RedTipTreeViewColumnHeader multiColumnHeader, TreeModel<RedTipTreeElement> model) : base(state, multiColumnHeader, model) {
            Assert.AreEqual(m_SortOptions.Length , Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 1;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kToggleWidth;
            multiColumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        protected override bool CanStartDrag(CanStartDragArgs args) {
            return false;
        }

        public static void TreeToList (TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
                throw new NullReferenceException("root");
            if (result == null)
                throw new NullReferenceException("result");

            result.Clear();

            if (root.children == null)
                return;

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (int i = root.children.Count - 1; i >= 0; i--)
                stack.Push(root.children[i]);

            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows) {
            if (rows.Count <= 1) {
                return;
            }
            if (multiColumnHeader.sortedColumnIndex == -1) {
                return;
            }

            SortByMultipleColumns();
            TreeToList(root, rows);
            Repaint();
        }
        void SortByMultipleColumns() {
            var sortedColumns = multiColumnHeader.state.sortedColumns;
            if (sortedColumns.Length == 0) {
                return;
            }

            var myTypes = rootItem.children.Cast<TreeViewItem<RedTipTreeElement>>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; ++i) {
                SortOption sortOption = m_SortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);
                switch (sortOption)
                {
                    case SortOption.Name:
                        if (ascending) {
                            orderedQuery = orderedQuery.ThenBy(l => l.data.name);
                        } else {
                            orderedQuery = orderedQuery.ThenByDescending(l => l.data.name);
                        }
                        break;
                    case SortOption.Value1:
                        if (ascending) {
                            orderedQuery = orderedQuery.ThenBy(l => l.data.TestFloat1);
                        } else {
                            orderedQuery = orderedQuery.ThenByDescending(l => l.data.TestFloat1);
                        }
                        break;
                    case SortOption.Active:
                        if (ascending) {
                            orderedQuery = orderedQuery.ThenBy(l => l.data.RedTipActive);
                        } else {
                            orderedQuery = orderedQuery.ThenByDescending(l => l.data.RedTipActive);
                        }
                        break;
                }
            }

            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<TreeViewItem<RedTipTreeElement>> InitialOrder(IEnumerable<TreeViewItem<RedTipTreeElement>> myTypes, int[] history) {
            SortOption sortOption = m_SortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption) {
                case SortOption.Name: {
                    if (ascending) {
                        return myTypes.OrderBy(l => l.data.name);
                    }
                    return myTypes.OrderByDescending(l => l.data.name);
                }
                    break;
                case SortOption.Value1: {
                    if (ascending) {
                        return myTypes.OrderBy(l => l.data.TestFloat1);
                    }
                    return myTypes.OrderByDescending(l => l.data.TestFloat1);
                }
                    break;
                case SortOption.Active: {
                    if (ascending) {
                        return myTypes.OrderBy(l => l.data.RedTipActive);
                    }
                    return myTypes.OrderByDescending(l => l.data.RedTipActive);
                }
                    break;
                default:
                    Assert.IsTrue(false, "unhandled enum " + sortOption.ToString());
                    break;
            }
            return myTypes.OrderBy(l => l.data.name);
        }

        void OnSortingChanged (MultiColumnHeader multiColumnHeader)
        {
            SortIfNeeded (rootItem, GetRows());
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root) {
            var rows = base.BuildRows(root);
            SortIfNeeded(root, rows);
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = (TreeViewItem<RedTipTreeElement>)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i) {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        int GetIcon1Index(TreeViewItem<RedTipTreeElement> item) {
            return 0;
        }

        void CellGUI (Rect cellRect, TreeViewItem<RedTipTreeElement> item, MyColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case MyColumns.Icon1:
					{
						GUI.DrawTexture(cellRect, s_Icons[GetIcon1Index(item)], ScaleMode.ScaleToFit);
					}
					break;

				case MyColumns.Name:
					{
						// Do toggle
						Rect toggleRect = cellRect;
						toggleRect.x += GetContentIndent(item);
						toggleRect.width = kToggleWidth;
						if (toggleRect.xMax < cellRect.xMax)
							item.data.enabled = EditorGUI.Toggle(toggleRect, item.data.enabled); // hide when outside cell rect

						// Default icon and label
						args.rowRect = cellRect;
						base.RowGUI(args);
					}
					break;

				case MyColumns.Value1:
                case MyColumns.Active:
					{
						if (showControls)
						{
							cellRect.xMin += 5f; // When showing controls make some extra spacing

							if (column == MyColumns.Value1)
								item.data.TestFloat1 = EditorGUI.Slider(cellRect, GUIContent.none, item.data.TestFloat1, 0f, 1f);
                            if (column == MyColumns.Active)
                                item.data.RedTipActive = EditorGUI.Toggle(cellRect, GUIContent.none, item.data.RedTipActive);
                        }
						else
						{
							string value = "Missing";
							if (column == MyColumns.Value1)
								value = item.data.TestFloat1.ToString("f5");
                            if (column == MyColumns.Active)
                                value = item.data.RedTipActive.ToString();
							DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
						}
					}
					break;
			}
		}

        static Texture2D[] s_Icons = {
            EditorGUIUtility.FindTexture("Folder Icon")
        };

        // All columns
        enum MyColumns
        {
            Icon1,
            Name,
            Value1,
            Active,
        }

        public enum SortOption
        {
            Name,
            Value1,
            Active,
        }

        SortOption[] m_SortOptions =
        {
            SortOption.Value1,
            SortOption.Name,
            SortOption.Value1,
            SortOption.Active,
        };


        public static MultiColumnHeaderState CreateMultiColumnHeaderState(float treeViewWidth) {
            var columns = new[] {
                new MultiColumnHeaderState.Column() {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "tooltip"),
                    contextMenuText = "Asset",
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("滑动", "tooltip"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column() {
                    headerContent = new GUIContent("是否激活", "tooltip"),
                    headerTextAlignment = TextAlignment.Right,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}
