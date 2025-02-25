using com.absence.dialoguesystem.internals;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    [InitializeOnLoad]
    public static class NodeViewStyles
    {
        static Dictionary<Type, List<StyleSheet>> s_nodeStyleCache;

        static NodeViewStyles()
        {
            Reset();
        }

        [MenuItem("absencee_/absent-dialogues/Reset Style Cache")]
        static void Refresh()
        {
            Reset();
            DialogueEditorWindow.Refresh();
        }

        static void Reset()
        {
            s_nodeStyleCache = null;
        }

        public static void ApplyStyles(NodeView view)
        {
            Node node = view.Node;
            Type nodeType = node.GetType();

            if (s_nodeStyleCache == null)
                s_nodeStyleCache = new();

            if (!s_nodeStyleCache.ContainsKey(nodeType))
            {
                s_nodeStyleCache.Add(nodeType, new List<StyleSheet>());

                node.AdditionalUSSFileLocations?.ForEach(path =>
                {
                    if (string.IsNullOrWhiteSpace(path)) return;

                    StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

                    s_nodeStyleCache[nodeType].Add(uss);
                });

                view.AdditionalUSSFileLocations?.ForEach(path =>
                {
                    if (string.IsNullOrWhiteSpace(path)) return;

                    StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

                    s_nodeStyleCache[nodeType].Add(uss);
                });
            }

            s_nodeStyleCache[nodeType].ForEach(uss =>
            {
                view.styleSheets.Add(uss);
            });
        }
    }
}
