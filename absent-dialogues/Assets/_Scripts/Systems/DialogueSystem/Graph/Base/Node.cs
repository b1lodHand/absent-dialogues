using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public abstract class Node : ScriptableObject
    {
        public enum NodeState
        {
            Unreached = 0,
            Current = 1,
            Past = 2,
        }

        [HideInInspector] public string Guid;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public Dialogue MasterDialogue;
        [HideInInspector] public Blackboard Blackboard;
        [HideInInspector] public NodeState State = NodeState.Unreached;
        public bool ExitDialogAfterwards = false;

        public event Action<Node.NodeState> OnSetState;
        public event Action OnRemove;
        public virtual bool DisplayState => true;
        public virtual bool ShowInMinimap => true;
        public abstract string GetClassName();
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
            Pass_Inline(passData);
        }
        public void Reach()
        {
            MasterDialogue.LastOrCurrentNode = this;
            if (this is ISpeechNode) MasterDialogue.LastOrCurrentSpeechNode = this;
            SetState(NodeState.Current);
            Reach_Inline();
        }
        public void OnRemoval()
        {
            OnRemove?.Invoke();
        }

        public void Traverse(Action<Node> visiter)
        {
            visiter?.Invoke(this);
            var nexts = this.GetNextNodes().ConvertAll(n => n.node);
            nexts.ForEach(n => n.Traverse(visiter));
        }

        protected abstract void AddNextNode_Inline(Node nextWillBeAdded, int atPort);
        protected abstract void RemoveNextNode_Inline(int atPort);
        protected abstract void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result);

        protected abstract void Pass_Inline(params object[] passData);
        protected abstract void Reach_Inline();

        public virtual string GetInputPortNameForCreation() => "From";
        public virtual List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>() { "To" };
        }

        public virtual void SetState(Node.NodeState newState)
        {
            if (!DisplayState) return;

            this.State = newState;
            OnSetState?.Invoke(newState);
        }
        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }

    public interface ISpeechNode
    {
        public int PersonIndex { get; set; }
        public Person Person { get; }
        public string GetSpeech();
        public string[] GetOptionSpeeches();
    }
}