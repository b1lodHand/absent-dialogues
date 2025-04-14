using com.absence.dialoguesystem.internals;
using System;
using System.Text;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public static class DialogueSystem
    {
        public const HideFlags DIALOGUE_SUB_ASSET_FLAGS = HideFlags.HideInInspector;
        public static bool BypassUndo = false;

        public static Node CreateNode(Node source, Dialogue target)
        {
            Node node = Node.Instantiate(source);
            CreateNode_Internal(node, target);
            target.AllNodes.Add(node);
            return node;
        }

        public static Node CreateNode(Type type, Dialogue target)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            CreateNode_Internal(node, target);
            target.AllNodes.Add(node);
            return node;
        }

        static Node CreateNode_Internal(Node instance, Dialogue target)
        {
            instance.Guid = GenerateGuid(instance);
            instance.name = GenerateName(instance);
            instance.hideFlags = DialogueSystem.DIALOGUE_SUB_ASSET_FLAGS;

            instance.FetchGenericOptions(target);
            target.ValidateNode(instance);
            instance.PersonIndex = 0;

            return instance;
        }

        public static string GenerateGuid(Node node)
        {
            if (node.GetType().Equals(typeof(EntryNode)))
                return nameof(EntryNode);

            return System.Guid.NewGuid().ToString();
        }

        public static string GenerateName(Node node)
        {
            return node.Guid;
        }

        public static string GenerateCustomDataName(Node node)
        {
            StringBuilder sb = new(node.Guid);
            sb.Append("_");
            sb.Append("CustomData");

            return sb.ToString();
        }

        public static string GenerateOptionDataName(Node node)
        {
            StringBuilder sb = new(node.Guid);
            sb.Append("_");
            sb.Append("OptionData");

            return sb.ToString();
        }

        public static string GenerateGenericOptionDataName(Dialogue dialogue)
        {
            return "GenericOptionData";
        }
    }
}
