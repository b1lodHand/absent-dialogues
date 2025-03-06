using com.absence.dialoguesystem.editor.internals;
using com.absence.dialoguesystem.internals;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [InitializeOnLoad]
    public static class NodeViewCreationHandler
    {
        const int k_neededArgumentCountForConstructor = 2;

        static Dictionary<Type, ConstructorInfo> s_database;
        static Dictionary<ConstructorInfo, bool> s_useForChildrenValuePairs;
        public static Dictionary<Type, ConstructorInfo> Database => s_database; 

        static NodeViewCreationHandler()
        {
            Refresh();
        }

        public static void Refresh()
        {
            s_database = new();
            s_useForChildrenValuePairs = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes()) 
                {
                    Type resultNodeType = null;
                    ConstructorInfo resultConstructor = null;

                    if (type == null)
                        continue;

                    CustomNodeViewAttribute attribute = type.GetCustomAttribute<CustomNodeViewAttribute>();

                    if (attribute == null)
                        continue;

                    Type baseType = type;
                    while (baseType != null)
                    {
                        if (baseType.Equals(typeof(NodeView)))
                            break;

                        baseType = baseType.BaseType;
                    }

                    if (baseType == null)
                        continue;

                    ConstructorInfo[] constructors = type.GetConstructors();

                    if (constructors == null || constructors.Length == 0)
                        continue;

                    ConstructorInfo foundConstructor = null;
                    foreach (ConstructorInfo constructor in constructors) 
                    {
                        if (constructor == null)
                            continue;

                        ParameterInfo[] parameters = constructor.GetParameters();

                        if (parameters.Length != k_neededArgumentCountForConstructor)
                            continue;

                        if (!parameters[0].ParameterType.Equals(typeof(Node)))
                            continue;

                        if (!parameters[1].ParameterType.Equals(typeof(DialogueGraphView)))
                            continue;

                        foundConstructor = constructor;
                    }

                    if (foundConstructor == null)
                        continue;

                    resultNodeType = attribute.type;
                    resultConstructor = foundConstructor;

                    if (s_database.ContainsKey(resultNodeType))
                    {
                        Debug.LogWarning("There are multiple NodeView subtypes associated with the same Node subtype. This is not supported, only one will be used.");
                        continue;
                    }

                    bool useForChildren = attribute.useForChildren;
                    s_useForChildrenValuePairs.Add(resultConstructor, useForChildren);
                    s_database.Add(resultNodeType, resultConstructor);
                }
            }
        }

        public static NodeView CreateNodeView<T>(Node node, DialogueGraphView graph) where T : Node
        {
            return DoCreateNodeView(typeof(T), node, graph);
        }

        public static NodeView CreateNodeView(Type type, Node node, DialogueGraphView graph)
        {
            return DoCreateNodeView(type, node, graph);
        }

        static NodeView DoCreateNodeView(Type type, Node node, DialogueGraphView graph)
        {
            ConstructorInfo constructor = null;
            if (!s_database.TryGetValue(type, out constructor))
            {
                Type baseType = type;
                while (baseType != null)
                {
                    if (s_database.TryGetValue(baseType, out ConstructorInfo temp))
                    {
                        if (s_useForChildrenValuePairs[temp])
                        {
                            break;
                        }
                    }

                    baseType = baseType.BaseType;
                }

                if (baseType == null) return new NodeView(node, graph);
                else return DoCreateNodeView(baseType, node, graph);
            }

            try
            {
                NodeView result = (NodeView)(constructor.Invoke(new object[] { node, graph }));
                return result;
            }

            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}
