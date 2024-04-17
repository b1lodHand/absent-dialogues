using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.absence.dialoguesystem.internals;

namespace com.absence.dialoguesystem
{
    public class Dialogue : ScriptableObject
    {
        [HideInInspector] public RootNode RootNode;
        [HideInInspector] public Node LastOrCurrentNode;
        [HideInInspector] public Node LastOrCurrentSpeechNode;
        [HideInInspector] public List<Node> AllNodes = new List<Node>();

        public List<Person> People = new List<Person>();
        public Blackboard Blackboard;

        private List<Person> m_tempPeople;

        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;

            node.PersonIndex = 0;

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
            List<Person> people = (List<Person>)data[0];
            if(people != null)
            {
                m_tempPeople = new List<Person>(People);
                People = new List<Person>(people);
            }

            AllNodes.ForEach(n =>
            {
                n.Blackboard = Blackboard;
                n.MasterDialogue = this;
            });

            RootNode.Reach();
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public void Cleanup()
        {
            if(m_tempPeople != null) People = m_tempPeople;
            m_tempPeople = null;
        }

        public void ResetProgress()
        {
            AllNodes.ForEach(n => n.SetState(Node.NodeState.Unreached));
            RootNode.Reach();
        }
    }
}
