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

        [HideInInspector] public Dialogue MasterDialogue;
        [HideInInspector] public Blackboard Blackboard;

        [HideInInspector] public NodeState State = NodeState.Unreached;
        [Tooltip("Toggling this on will make the dialogue exit right after this node getting passed.")] public bool ExitDialogAfterwards = false;

        public event Action<Node.NodeState> OnSetState;
        public event Action OnRemove;
        public event Action OnValidation;

        public event Action OnReach;
        public event Action OnPass;

        [HideInInspector] public int PersonIndex;
        [HideInInspector] public Person Person { get => MasterDialogue.People[PersonIndex]; }

        public virtual bool DisplayState => true;
        public virtual bool ShowInMinimap => true;
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

        public void AddNextNode(Node nextWillBeAdded, int atPort)
        {
            AddNextNode_Inline(nextWillBeAdded, atPort);
        }
        public void RemoveNextNode(int atPort)
        {
            RemoveNextNode_Inline(atPort);
        }
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
        /// </summary>
        /// <returns></returns>
        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        private void OnValidate()
        {
            OnValidation?.Invoke();
        }
    }

    public interface IContainSpeech
    {
        public string GetSpeech();
        public string[] GetOptions();
        public AdditionalSpeechData GetAdditionalSpeechData();
    }

    [System.Serializable]
    public class AdditionalSpeechData
    {
        public AudioClip AudioClip;
        public Animation Animation;
        public Sprite Sprite;
        public string Keyword;
    }
}