using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// The scriptable object derived type that holds all of the data which is essential for a dialogue.
    /// </summary>
    public class Dialogue : ScriptableObject
    {
        /// <summary>
        /// The <see cref="RootNode"/> of this dialogue.
        /// </summary>
        [HideInInspector] public RootNode RootNode;

        /// <summary>
        /// The current node reached while progressing in this dialogue. Or the last one reached before exiting the dialogue.
        /// </summary>
        [HideInInspector] public Node LastOrCurrentNode;

        /// <summary>
        /// A list of all of the nodes that are in this dialogue.
        /// </summary>
        [HideInInspector] public List<Node> AllNodes = new List<Node>();

        [SerializeField] private List<Person> m_people = new List<Person>();
        private List<Person> m_tempPeople;

        /// <summary>
        /// People in this dialogue (might be overridden).
        /// </summary>
        public List<Person> People => m_people;

        /// <summary>
        /// The <see cref="Blackboard"/> of this dialogue.
        /// </summary>
        public Blackboard Blackboard;

        /// <summary>
        /// Use to create new nodes. Using runtime is not recommended.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Use to delete existing nodes. Using runtime is not recommended.
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(Node node)
        {
            AllNodes.Remove(node);
            node.OnRemoval();
        }

        /// <summary>
        /// Use to clone the dialogue scriptable object. Useful to progress in a copy
        /// while keeping the original unchanged.
        /// </summary>
        /// <returns></returns>
        public Dialogue Clone()
        {
            Dialogue dialog = Instantiate(this);
            dialog.RootNode = this.RootNode.Clone() as RootNode;
            return dialog;
        }

        /// <summary>
        /// Use to find <see cref="DialoguePartNode"/>s with a specific name.
        /// </summary>
        /// <param name="targetName"></param>
        /// <returns>A list of <see cref="DialoguePartNode"/>s with that specific name. Throws an exception nothing's
        /// found.</returns>
        public List<DialoguePartNode> GetDialogPartNodesWithName(string targetName)
        {
            var check = AllNodes.Where(n =>
            {
                var dialogPartNode = n as DialoguePartNode;
                if (dialogPartNode == null) return false;
                if (dialogPartNode.DialoguePartName != targetName) return false;

                return true;
            }).ToList();

            if (check.Count == 0) throw new Exception($"There is no dialog part named '{targetName}' in dialog '{this.name}'!");
            else if (check.Count > 1) throw new Exception($"There are multiple dialog parts named '{targetName}' in dialog '{this.name}'!");

            return check.ConvertAll(n => (n as DialoguePartNode)).ToList();
        }

        /// <summary>
        /// Use to get a list of all <see cref="DialoguePartNode"/>s in this dialogue.
        /// </summary>
        /// <returns>The entire list of <see cref="DialoguePartNode"/>s in the current dialogue.</returns>
        public List<DialoguePartNode> GetAllDialogParts()
        {
            return AllNodes.Where(n => n is DialoguePartNode).ToList().ConvertAll(n => (n as DialoguePartNode)).ToList();
        }

        /// <summary>
        /// Use to initialize the dialogue before using it.
        /// </summary>
        public void Bind()
        {
            AllNodes.ForEach(n =>
            {
                n.Blackboard = Blackboard;
                n.MasterDialogue = this;
            });

            RootNode.Reach();
        }

        /// <summary>
        /// Use to override the people in this dialogue. Keeping person count the same is highly recommended. The original scriptable
        /// object's people list won't be affected by this.
        /// </summary>
        /// <param name="overridePeople"></param>
        public void OverridePeople(List<Person> overridePeople)
        {
            if (overridePeople != null)
            {
                m_tempPeople = new List<Person>(m_people);
                m_people = new List<Person>(overridePeople);
            }

        }

        /// <summary>
        /// Use to progress to the next node in the dialogue. Using this method directly is not recommended
        /// if you're not adding an extra functionality. You can consider using <see cref="DialoguePlayer"/> instead.
        /// </summary>
        /// <param name="passData"></param>
        public void Pass(params object[] passData)
        {
            if (LastOrCurrentNode != null) LastOrCurrentNode.Pass(passData);
        }

        /// <summary>
        /// Use to reset people list if you've overridden it before.
        /// </summary>
        public void ResetPeopleList()
        {
            if(m_tempPeople != null) m_people = new(m_tempPeople);
            m_tempPeople = null;
        }

        /// <summary>
        /// Use to reset all the progress has gotten in this dialogue.
        /// </summary>
        public void ResetProgress()
        {
            AllNodes.ForEach(n => n.SetState(Node.NodeState.Unreached));

            RootNode.Reach();
        }
    }
}
