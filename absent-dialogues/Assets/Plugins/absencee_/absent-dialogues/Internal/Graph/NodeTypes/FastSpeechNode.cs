using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which displays a speech without options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.FastSpeechNode.html")]
    public sealed class FastSpeechNode : Node, IContainData, IPerformDelayedClone
    {
        [SerializeField] private ExtraDialogueData m_extraData;

        [HideInInspector] public Node Next;
        [HideInInspector] public string m_text;

        public override bool PersonDependent => true;

        public string Text { get => m_text; set { m_text = value; } }
        public List<Option> Options { get => null; set { return; } }
        public ExtraDialogueData ExtraData { get { return m_extraData; } set { m_extraData = value; } }

        public override string GetClassName() => "fastSpeechNode";
        public override string GetTitle() => "Skippable";

        protected override void Pass_Inline(DialogueFlowContext context)
        {
            context.ClearSpeech();

            if (Next == null) return;

            Next.Reach(context);
            SetState(NodeState.Past);
        }
        protected override void Reach_Inline(DialogueFlowContext context)
        {
            context.Text = Text;
        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Inline(int atPort)
        {
            Next = null;
        }
        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            if (Next != null) result.Add((0, Next));
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Next.Traverse(action);
        }

        public ExtraDialogueData GetExtraData() => m_extraData;

        public void DelayedClone(Dialogue originalDialogue)
        {
            if (Next != null) Next = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}