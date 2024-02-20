using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    [CreateAssetMenu()]
    public class Dialogue : ScriptableObject
    {
        public RootNode RootNode;
        public Node LastOrCurrentNode;
        public Node LastOrCurrentSpeechNode;

        public List<Node> AllNodes = new List<Node>();
        [HideInInspector] public Blackboard Blackboard = new Blackboard();

        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();

            node.Blackboard = Blackboard;
            node.MasterDialogue = this;

            Undo.RecordObject(this, "Dialog (Create Node)");
            AllNodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Dialog (Create Node)");

            AssetDatabase.SaveAssets();
            return node;
        }
        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Dialog (Delete Node)");
            AllNodes.Remove(node);
            node.OnRemoval();
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public Dialogue Clone()
        {
            Dialogue dialog = Instantiate(this);
            dialog.RootNode = this.RootNode.Clone() as RootNode;
            return dialog;
        }

        public List<DialoguePartNode> GetDialogPartNodesWithName(string targetName)
        {
            var check = AllNodes.Where(n =>
            {
                var dialogPartNode = n as DialoguePartNode;
                if (dialogPartNode == null) return false;
                if (dialogPartNode.DialogPartName != targetName) return false;

                return true;
            }).ToList();

            if (check.Count == 0) throw new Exception($"There is no dialog part named '{targetName}' in dialog '{this.name}'!");
            else if (check.Count > 1) throw new Exception($"There are multiple dialog parts named '{targetName}' in dialog '{this.name}'!");

            return check.ConvertAll(n => (n as DialoguePartNode)).ToList();
        }
        public List<DialoguePartNode> GetAllDialogParts()
        {
            return AllNodes.Where(n => n is DialoguePartNode).ToList().ConvertAll(n => (n as DialoguePartNode)).ToList();
        }

        public void Bind(params object[] data)
        {
            AllNodes.ForEach(n =>
            {
                n.Blackboard = Blackboard;
                n.MasterDialogue = this;
            });

            RootNode.Reach();
        }
        public void ResetProgress()
        {
            AllNodes.ForEach(n => n.SetState(Node.NodeState.Unreached));
            RootNode.Reach();
        }
    }
}
