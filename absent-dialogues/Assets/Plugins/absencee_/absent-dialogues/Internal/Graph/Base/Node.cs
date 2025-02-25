using com.absence.attributes.experimental;
using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// This is the base abstract class to derive from for any new node subtypes.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Node.html")]
    public abstract class Node : ScriptableObject
    {
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

        /// <summary>
        /// Use if you have a special USS class for this node. If you don't have any, return null.
        /// </summary>
        /// <returns>Returns the USS class name of this node type as a string.</returns>
        public abstract string GetClassName();

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
        public void AddNextNode(Node nextWillBeAdded, int atPort)
        {
            AddNextNode_Internal(nextWillBeAdded, atPort);
        }

        /// <summary>
        /// Use when you disconnect a node from a riht-side port of this node.
        /// </summary>
        /// <param name="atPort">The port which handled the disconnection event.</param>
        public void RemoveNextNode(int atPort)
        {
            RemoveNextNode_Internal(atPort);
        }

        /// <summary>
        /// Use to get all of the nodes which are <b>directly</b> connected to this node <b>(only the right-side ones)</b>.
        /// </summary>
        /// <returns></returns>
        public List<(int portIndex, Node node)> GetNextNodes()
        {
            var result = new List<(int portIndex, Node node)>();
            GetNextNodes_Internal(ref result);
            return result;
        }

        public void Pass(DialogueFlowContext context)
        {
            SetState(FlowState.Past);

            if (context != null)
            {
                context.State = DialogueFlowContext.ContextState.Pass;
            }

            onPass?.Invoke();
            OnPass(context);
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
        public void OnRemoval()
        {
            onRemove?.Invoke();
        }

        /// <summary>
        /// Use to write the functionality of connecting a node to any port of this node.
        /// </summary>
        /// <param name="nextWillBeAdded"></param>
        /// <param name="atPort"></param>
        protected abstract void AddNextNode_Internal(Node nextWillBeAdded, int atPort);

        /// <summary>
        /// Use to write the functionality of removing the next node of this one.
        /// </summary>
        /// <param name="atPort"></param>
        protected abstract void RemoveNextNode_Internal(int atPort);

        /// <summary>
        /// Use to describe the editor which nodes are the next nodes of this one in the chain by modifying the list.
        /// </summary>
        /// <param name="result"></param>
        protected abstract void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result);

        /// <summary>
        /// Use to write what happenswhen the dialogue passes this node.
        /// </summary>
        /// <param name="passData"></param>
        protected abstract void OnPass(DialogueFlowContext context);

        /// <summary>
        /// Use to write what happens when the dialogue reaches this node.
        /// </summary>
        protected abstract void OnReach(DialogueFlowContext context);

        /// <summary>
        /// Use to describe the name of the input port of this node.
        /// </summary>
        /// <returns>Returns the name as a string. Return null if you don't want any input ports.</returns>
        public virtual string GetInputPortNameForCreation() => "From";

        /// <summary>
        /// Use to describe the dialogue editor how many output ports this node has and what are their names.
        /// </summary>
        /// <returns>Returns the port names as a list of strings. Return an empty list if you want no output ports.</returns>
        public virtual List<string> GetOutputPortNamesForCreation()
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

        /// <summary>
        /// Use to clone this node. 
        /// </summary>
        /// <returns>The clone.</returns>
        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        /// <summary>
        /// Use to traverse any action on a node chain. Nodes not connected directly won't transmit the action to another.
        /// </summary>
        public virtual void Traverse(Action<Node> action)
        {

        }

        public virtual void OnImport(NodeData dataToRead, DialogueImportContext context)
        {

        }

        public virtual void OnExport(NodeData dataToWrite)
        {

        }

        public virtual void OnValidate()
        {
            UpdateManipulators();

            onValidation?.Invoke();

            return;

            void UpdateManipulators() 
            {
                if (this is IContainVariableManipulators manipulator)
                {
                    if (Blackboard == null || Blackboard.Bank == null)
                        return;

                    manipulator.GetComparers()?.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));
                    manipulator.GetSetters()?.ForEach(setter => setter.SetBlackboardBank(Blackboard.Bank));
                }
            }
        }
    }
}