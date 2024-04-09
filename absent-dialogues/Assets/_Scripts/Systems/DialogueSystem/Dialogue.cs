using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class Dialogue : ScriptableObject
    {
        public RootNode RootNode;
        public Node LastOrCurrentNode;
        public Node LastOrCurrentSpeechNode;

        public List<Node> AllNodes = new List<Node>();
        public List<PersonProfile> People = new List<PersonProfile>();
        public Blackboard Blackboard;

        bool m_preAssignedPlayers = true;

        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;

            node.Blackboard = Blackboard;
            node.MasterDialogue = this;

            AllNodes.Add(node);
            return node;
        }

        public void DeleteNode(Node node)
        {
            AllNodes.Remove(node);
            node.OnRemoval();
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
            List<PersonProfile> people = (List<PersonProfile>)data[0];
            if(people != null)
            {
                m_preAssignedPlayers = false;
                People = people;
            }

            AllNodes.ForEach(n =>
            {
                n.Blackboard = Blackboard;
                n.MasterDialogue = this;
            });

            RootNode.Reach();
        }

        public void Cleanup()
        {
            if(!m_preAssignedPlayers) People.Clear();
            m_preAssignedPlayers = true;
        }

        public void ResetProgress()
        {
            AllNodes.ForEach(n => n.SetState(Node.NodeState.Unreached));
            RootNode.Reach();
        }
    }
}
