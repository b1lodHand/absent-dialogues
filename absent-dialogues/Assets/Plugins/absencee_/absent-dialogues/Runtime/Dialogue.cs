using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.absence.personsystem;
using com.absence.dialoguesystem.internals;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// The scriptable object derived type that holds all of the data which is essential for a dialogue.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.Dialogue.html")]
    public class Dialogue : ScriptableObject
    {
        /// <summary>
        /// The <see cref="EntryNode"/> of this dialogue.
        /// </summary>
        [HideInInspector] public EntryNode Entry;

        /// <summary>
        /// A list of all of the nodes that are in this dialogue.
        /// </summary>
        [HideInInspector] public List<Node> AllNodes = new List<Node>();

        [SerializeField] private List<Person> m_people = new List<Person>();
        /// <summary>
        /// People in this dialogue (might be overridden on clones).
        /// </summary>
        public List<Person> People => m_people;

        [HideInInspector, SerializeField] private List<GenericOption> m_genericOptions = new List<GenericOption>();

        public List<GenericOption> GenericOptions
        {
            get
            {
                return m_genericOptions;
            }

            internal set
            {
                m_genericOptions = value;
            }
        }

        /// <summary>
        /// The original dialogue which is used to create this cloned one. Returns null if this dialogue is not a clone.
        /// </summary>
        public Dialogue ClonedFrom { get; private set; }

        /// <summary>
        /// Use to check if this dialogue is a clone.
        /// </summary>
        public bool IsClone => ClonedFrom != null;

        /// <summary>
        /// Action which will get invoked if <see cref="OnValidate"/> gets called in the editor.
        /// </summary>
        public event Action OnValidateAction;

        public event Action OnGenericOptionsChange;
        public event Action<int, int> OnGenericOptionsRearranged;
        public event Action<int> OnGenericOptionCreated;
        public event Action<int> OnGenericOptionRemoved;

        /// <summary>
        /// The <see cref="Blackboard"/> of this dialogue.
        /// </summary>
        [HideInInspector] public Blackboard Blackboard;

        /// <summary>
        /// Use to find <see cref="SectionNode"/>s with a specific name.
        /// </summary>
        /// <param name="targetName"></param>
        /// <returns>A list of <see cref="SectionNode"/>s with that specific name. Throws an exception nothing's
        /// found.</returns>
        public List<SectionNode> GetSectionsWithName(string targetName)
        {
            var check = AllNodes.Where(n =>
            {
                var dialogPartNode = n as SectionNode;
                if (dialogPartNode == null) return false;
                if (dialogPartNode.SectionName != targetName) return false;

                return true;
            }).ToList();

            if (check.Count == 0) throw new Exception($"There is no dialog part named '{targetName}' in dialog '{this.name}'!");
            else if (check.Count > 1) throw new Exception($"There are multiple dialog parts named '{targetName}' in dialog '{this.name}'!");

            return check.ConvertAll(n => (n as SectionNode)).ToList();
        }

        /// <summary>
        /// Use to get a list of all <see cref="SectionNode"/>s in this dialogue.
        /// </summary>
        /// <returns>The entire list of <see cref="SectionNode"/>s in the current dialogue.</returns>
        public List<SectionNode> GetAllSections()
        {
            return AllNodes.Where(n => n is SectionNode).ToList().ConvertAll(n => (n as SectionNode)).ToList();
        }

        /// <summary>
        /// Use to clone the dialogue scriptable object. Useful to progress in a copy
        /// while keeping the original unchanged.
        /// </summary>
        /// <returns></returns>
        public Dialogue Clone()
        {
            Dialogue dialogue = Instantiate(this);
            dialogue.Blackboard = Blackboard.Clone();
            dialogue.m_genericOptions = GenericOptions.ConvertAll(opt => opt.Clone<GenericOption>(dialogue.Blackboard.Bank));

            dialogue.AllNodes = AllNodes.ConvertAll(node => node.Clone());

            dialogue.AllNodes.ForEach(node =>
            {
                node.Blackboard = dialogue.Blackboard;
                node.OnCloning(this, dialogue);
                node.UpdateManipulators();
            });

            dialogue.Entry = (EntryNode)dialogue.AllNodes.Where(node => node is EntryNode).FirstOrDefault();
            dialogue.ClonedFrom = this;

            return dialogue;
        }

        /// <summary>
        /// It teleports the flow back to the root node.
        /// </summary>
        public void ResetNodeStates()
        {
            AllNodes.ForEach(node => node.SetState(Node.FlowState.Unreached));
        }

        /// <summary>
        /// It reassigns needed auto-fields to prevent any errors.
        /// </summary>
        public void ValidateNodes()
        {
            AllNodes.ForEach(ValidateNode);
        }

        internal void ValidateNode(Node node)
        {
            node.Blackboard = Blackboard;
            //node.FetchGenericOptions(this);
            node.UpdateManipulators();
        }

        internal void InvokeOnGenericOptionsChange()
        {
            OnGenericOptionsChange?.Invoke();
        }

        internal void InvokeOnGenericOptionCreated(int at)
        {
            OnGenericOptionCreated?.Invoke(at);
        }

        internal void InvokeOnGenericOptionRemoved(int at)
        {
            OnGenericOptionRemoved?.Invoke(at);
        }

        internal void InvokeOnGenericOptionsRearranged(int replacer, int replaced)
        {
            OnGenericOptionsRearranged?.Invoke(replacer, replaced);
        }

        internal void ClearCallbacks()
        {
            OnGenericOptionCreated = null;
            OnGenericOptionRemoved = null;
            OnGenericOptionsRearranged = null;
            OnGenericOptionsChange = null;
            OnValidateAction = null;
        }

        public void OnValidate()
        {
            m_genericOptions.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer =>
                {
                    comparer.SetBlackboardBank(Blackboard.Bank);
                });
            });

            OnValidateAction?.Invoke();
        }
    }
}
