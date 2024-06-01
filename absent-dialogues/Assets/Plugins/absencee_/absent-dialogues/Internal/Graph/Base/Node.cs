using com.absence.attributes;
using com.absence.personsystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// This is the base abstract class to derive from for any new node subtypes.
    /// </summary>
    public abstract class Node : ScriptableObject
    {
        /// <summary>
        /// Describes the node's state on the flow. While progressing in the dialogue.
        /// </summary>
        public enum NodeState
        {
            Unreached = 0,
            Current = 1,
            Past = 2,
        }

        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position = new();

        [Readonly] public Dialogue MasterDialogue;
        [HideInInspector] public Blackboard Blackboard;

        [HideInInspector] public NodeState State = NodeState.Unreached;
        [Tooltip("Toggling this on will make the dialogue exit right after this node getting passed.")] public bool ExitDialogAfterwards = false;

        /// <summary>
        /// Action which will get invoked when the state of this node gets changed.
        /// </summary>
        public event Action<NodeState> OnSetState;

        /// <summary>
        /// Action which will get invoked when this node gets removed from the dialogue.
        /// </summary>
        public event Action OnRemove;

        /// <summary>
        /// Action which will get invoked when <see cref="OnValidate"/> function gets called.
        /// </summary>
        public event Action OnValidation;

        /// <summary>
        /// Action which will get invoked when this node gets reached on the flow.
        /// </summary>
        public event Action OnReach;

        /// <summary>
        /// Action which will get invoked when this node get passed on the flow.
        /// </summary>
        public event Action OnPass;

        /// <summary>
        /// Index of the person this node depends on (if it is <see cref="PersonDependent"/>) on the person list of the
        /// <see cref="MasterDialogue"/>.
        /// </summary>
        [HideInInspector] public int PersonIndex;

        /// <summary>
        /// Property which returns the person with the index of <see cref="PersonIndex"/> from the person list.
        /// </summary>
        [HideInInspector] public Person Person { get => MasterDialogue.People[PersonIndex]; }

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

        /// <summary>
        /// Use to  set the title of this node type in the graph view.
        /// </summary>
        /// <returns>The title as a string.</returns>
        public abstract string GetTitle();

        /// <summary>
        /// Use when you connect a new node to a right-side port of this node.
        /// </summary>
        /// <param name="nextWillBeAdded">The reference value of the node connected.</param>
        /// <param name="atPort">The port which hold the connection.</param>
        public void AddNextNode(Node nextWillBeAdded, int atPort)
        {
            AddNextNode_Inline(nextWillBeAdded, atPort);
        }

        /// <summary>
        /// Use when you disconnect a node from a riht-side port of this node.
        /// </summary>
        /// <param name="atPort">The port which handled the disconnection event.</param>
        public void RemoveNextNode(int atPort)
        {
            RemoveNextNode_Inline(atPort);
        }

        /// <summary>
        /// Use to get all of the nodes which are <b>directly</b> connected to this node <b>(only the right-side ones)</b>.
        /// </summary>
        /// <returns></returns>
        public List<(int portIndex, Node node)> GetNextNodes()
        {
            var result = new List<(int portIndex, Node node)>();
            GetNextNodes_Inline(ref result);
            return result;
        }

        public void Pass(params object[] passData)
        {
            SetState(NodeState.Past);

            OnPass?.Invoke();
            Pass_Inline(passData);
        }
        public void Reach()
        {
            MasterDialogue.LastOrCurrentNode = this;
            SetState(NodeState.Current);

            OnReach?.Invoke();
            Reach_Inline();
        }
        public void OnRemoval()
        {
            OnRemove?.Invoke();
        }

        /// <summary>
        /// Use to write the functionality of connecting a node to any port of this node.
        /// </summary>
        /// <param name="nextWillBeAdded"></param>
        /// <param name="atPort"></param>
        protected abstract void AddNextNode_Inline(Node nextWillBeAdded, int atPort);

        /// <summary>
        /// Use to write the functionality of removing the next node of this one.
        /// </summary>
        /// <param name="atPort"></param>
        protected abstract void RemoveNextNode_Inline(int atPort);

        /// <summary>
        /// Use to describe the editor which nodes are the next nodes of this one in the chain by modifying the list.
        /// </summary>
        /// <param name="result"></param>
        protected abstract void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result);

        /// <summary>
        /// Use to write what happenswhen the dialogue passes this node.
        /// </summary>
        /// <param name="passData"></param>
        protected abstract void Pass_Inline(params object[] passData);

        /// <summary>
        /// Use to write what happens when the dialogue reaches this node.
        /// </summary>
        protected abstract void Reach_Inline();

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
        public virtual void SetState(Node.NodeState newState)
        {
            if (!DisplayState) return;

            this.State = newState;
            OnSetState?.Invoke(newState);
        }

        /// <summary>
        /// Use to clone this node. 
        /// <br></br>
        /// <br></br>
        /// <b>CAUTION!</b> It works as a traverse function. If you clone any node,
        /// it will automatically clone any node connected to it (forward-only). But the <see cref="GotoNode"/> won't clone
        /// the <see cref="DialoguePartNode"/> referenced to it. Simply because they are not connected directly.
        /// </summary>
        /// <returns></returns>
        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        /// <summary>
        /// Use to traverse any action on a node chain. Nodes not connected directly won't transmitthe action to another.
        /// </summary>
        public virtual void Traverse(Action<Node> action)
        {

        }

        protected virtual void OnValidate()
        {
            OnValidation?.Invoke();
        }
    }
}