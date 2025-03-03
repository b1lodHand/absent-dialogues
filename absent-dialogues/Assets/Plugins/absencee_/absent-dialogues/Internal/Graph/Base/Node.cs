using com.absence.attributes.experimental;
using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using com.absence.personsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// This is the base abstract class to derive from for any new node subtypes.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Node.html")]
    public abstract class Node : ScriptableObject
    {
        public const string NaN = "NaN";

        /// <summary>
        /// Describes the node's state on the flow. While progressing in the dialogue.
        /// </summary>
        public enum FlowState
        {
            Unreached = 0,
            Current = 1,
            Past = 2,
        }

        [HideInInspector] public string Guid;

#if UNITY_EDITOR
        [HideInInspector] public Vector2 Position = new();
#endif

        [InlineEditor(newButtonId = 1801, delButtonId = 1800)]
        public NodeCustomDataBase CustomData = null;

        [HideInInspector] public Blackboard Blackboard;
        [HideInInspector] public FlowState State = FlowState.Unreached;

        /// <summary>
        /// Action which will get invoked when the state of this node gets changed.
        /// </summary>
        public event Action<FlowState> onSetState;

        /// <summary>
        /// Action which will get invoked when this node gets removed from the dialogue.
        /// </summary>
        public event Action onRemove;

        /// <summary>
        /// Action which will get invoked when <see cref="OnValidate"/> function gets called.
        /// </summary>
        public event Action onValidation;

        /// <summary>
        /// Action which will get invoked when this node gets reached on the flow.
        /// </summary>
        public event Action onReach;

        /// <summary>
        /// Action which will get invoked when this node get passed on the flow.
        /// </summary>
        public event Action onPass;

        /// <summary>
        /// Index of the person this node depends on (if it is <see cref="PersonDependent"/>).
        /// </summary>
        [HideInInspector] public int PersonIndex;

        /// <summary>
        /// Will this node display it's state in editor on the flow.
        /// </summary>
        public virtual bool DisplayState => true;

        /// <summary>
        /// Will this node be visible on the minimap.
        /// </summary>
        public virtual bool ShowInMinimap => true;

        /// <summary>
        /// Is this node person dependent.
        /// </summary>
        public virtual bool PersonDependent => false;

        public virtual string Text
        {
            get
            {
                return null;
            }

            set
            {

            }
        }
        public virtual List<Option> Options
        {
            get
            {
                return null;
            }

            set
            {

            }
        }

        [HideInInspector, SerializeField] private List<GenericOptionReference> m_genericOptions;
        public List<GenericOptionReference> GenericOptions { get { return m_genericOptions; } set { m_genericOptions = value; } }

        public virtual List<NodeVariableComparer> Comparers
        {
            get
            {
                return null;
            }

            set
            {

            }
        }
        public virtual List<NodeVariableSetter> Setters
        {
            get
            {
                return null;
            }

            set
            {

            }
        }

        public virtual bool HasText => Text != null;
        public virtual bool HasOptions => Options != null;
        public virtual bool UseGenericOptions => false;
        public bool NoOptionsOverall =>
            ((!HasOptions) || Options.Count == 0) && ((!UseGenericOptions) || GenericOptions.Count == 0);

        public bool NoCertainOptions
        {
            get
            {
                bool noCertainNormalOptions =
                    HasOptions ? Options.Where(opt => !opt.UseShowIf).Count() == 0 : true;

                bool noCertainGenericOptions =
                    UseGenericOptions ? GenericOptions.Where(opt => (!opt.Bypass) && (!opt.Target.UseShowIf)).Count() == 0 : true;

                return noCertainNormalOptions && noCertainGenericOptions;
            }
        }

        public virtual List<string> AdditionalUSSFileLocations => null;

        /// <summary>
        /// Use to  set the title of this node type in the graph view.
        /// </summary>
        /// <returns>The title as a string.</returns>
        public virtual string Title => null;

        /// <summary>
        /// Use when you connect a new node to a right-side port of this node.
        /// </summary>
        /// <param name="nextWillBeAdded">The reference value of the node connected.</param>
        /// <param name="atPort">The port which hold the connection.</param>
        public void AddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            OnAddOutputConnection(nextWillBeAdded, atPort);
        }

        /// <summary>
        /// Use when you disconnect a node from a riht-side port of this node.
        /// </summary>
        /// <param name="atPort">The port which handled the disconnection event.</param>
        public void RemoveOutputConnection(int atPort)
        {
            OnRemoveOutputConnection(atPort);
        }

        /// <summary>
        /// Use to get all of the nodes which are <b>directly</b> connected to this node <b>(only the right-side ones)</b>.
        /// </summary>
        /// <returns></returns>
        public List<Node> GetOutputConnections()
        {
            var result = new List<Node>();
            WriteOutputConnections(ref result);
            return result;
        }

        public Node Pass(DialogueFlowContext context)
        {
            SetState(FlowState.Past);

            if (context != null)
            {
                context.State = DialogueFlowContext.ContextState.Pass;
            }

            onPass?.Invoke();
            return OnPass(context);
        }
        public void Reach(DialogueFlowContext context)
        {
            SetState(FlowState.Current);

            if (context != null)
            {
                context.State = DialogueFlowContext.ContextState.Reach;
                context.CustomData = CustomData;
            }

            onReach?.Invoke();
            OnReach(context);
        }
        /// <summary>
        /// Use to clone this node. 
        /// </summary>
        /// <returns>The clone.</returns>
        internal Node Clone()
        {
            Node result = Instantiate(this);
            if (CustomData != null) result.CustomData = NodeCustomDataBase.Instantiate(CustomData);
            return result;
        }

        internal void FetchGenericOptions(Dialogue dialogue)
        {
#if UNITY_EDITOR
            if (GenericOptions != null)
                return;

            List<GenericOptionReference> references = new();

            for (int i = 0; i < dialogue.GenericOptions.Count; i++)
            {
                references.Add(new GenericOptionReference(dialogue.GenericOptions[i]));
            }

            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "Node (Fetch Generic Options)");
            GenericOptions = references;

            Undo.CollapseUndoOperations(group);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public Node Clone(Dialogue originalDialogue, Dialogue cloneDialogue)
        {
            Node result = this.Clone();
            result.OnCloning(originalDialogue, cloneDialogue);
            return result;
        }

        /// <summary>
        /// Use to traverse any action on a node chain. Nodes not connected directly won't transmit the action to another.
        /// </summary>
        public void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            GetOutputConnections().ForEach(node =>
            {
                if (node != null)
                    node.Traverse(action);
            });
        }

        public void OnRemoveFromDialogue()
        {
            onRemove?.Invoke();
        }

        /// <summary>
        /// Use to write the functionality of connecting a node to any port of this node.
        /// </summary>
        /// <param name="nextWillBeAdded"></param>
        /// <param name="atPort"></param>
        protected abstract void OnAddOutputConnection(Node nextWillBeAdded, int atPort);

        /// <summary>
        /// Use to write the functionality of removing the next node of this one.
        /// </summary>
        /// <param name="atPort"></param>
        protected abstract void OnRemoveOutputConnection(int atPort);

        /// <summary>
        /// Use to describe the editor which nodes are the next nodes of this one in the chain by modifying the list.
        /// </summary>
        /// <param name="result"></param>
        protected abstract void WriteOutputConnections(ref List<Node> result);

        /// <summary>
        /// Use to write what happenswhen the dialogue passes this node.
        /// </summary>
        /// <param name="passData"></param>
        protected abstract Node OnPass(DialogueFlowContext context);

        /// <summary>
        /// Use to write what happens when the dialogue reaches this node.
        /// </summary>
        protected abstract void OnReach(DialogueFlowContext context);

        /// <summary>
        /// Use to describe the name of the input port of this node.
        /// </summary>
        /// <returns>Returns the name as a string. Return null if you don't want any input ports.</returns>
        public virtual string GetDefaultInputPortName() => "From";

        /// <summary>
        /// Use to describe the dialogue editor how many output ports this node has and what are their names.
        /// </summary>
        /// <returns>Returns the port names as a list of strings. Return an empty list if you want no output ports.</returns>
        public virtual List<string> GetDefaultOutputPortNames()
        {
            return new List<string>() { "To" };
        }

        /// <summary>
        /// Use to set the flow state of this node.
        /// </summary>
        /// <param name="newState"></param>
        public virtual void SetState(Node.FlowState newState)
        {
            if (!DisplayState) return;

            this.State = newState;
            onSetState?.Invoke(newState);
        }

        public virtual Person GetPerson(Dialogue context) => 
            PersonDependent ? (context.People.Count > 0 ? (context.People[PersonIndex]) : (null)) : (null);

        public virtual void OnImport(NodeData dataToRead, DialogueImportContext context)
        {

        }

        public virtual void OnExport(NodeData dataToWrite)
        {

        }

        public virtual void OnCloning(Dialogue originalDialogue, Dialogue cloneDialogue)
        {
            if (Options != null)
            {
                Options = Options.ConvertAll(opt => 
                {
                    Option result = opt.Clone<Option>(cloneDialogue.Blackboard.Bank);
                    //result.LeadsTo = cloneDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(result.LeadsTo)];
                    return result;
                });
            }

            if (Comparers != null)
            {
                Comparers = Comparers.ConvertAll(cmp => cmp.Clone(cloneDialogue.Blackboard.Bank));
            }

            if (Setters != null)
            {
                for (int i = 0; i < Setters.Count; i++)
                {
                    Setters = Setters.ConvertAll(set => set.Clone(cloneDialogue.Blackboard.Bank));
                }
            }

            List<Node> originalConnections = GetOutputConnections();
            for (int i = 0; i < originalConnections.Count; i++) 
            {
                Node originalConnection = originalConnections[i];

                if (originalConnection == null)
                    continue;

                AddOutputConnection(cloneDialogue.AllNodes.First(nd => nd.Guid.Equals(originalConnection.Guid)), i);
            }
        }

        public virtual void UpdateManipulators()
        {
            Comparers?.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));
            Setters?.ForEach(setter => setter.SetBlackboardBank(Blackboard.Bank));
        }

        public virtual void OnValidate()
        {
            UpdateManipulators();
            onValidation?.Invoke();

            return;
        }
    }
}