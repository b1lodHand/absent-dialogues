using com.absence.attributes.experimental;
using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which displays a speech without options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.FastSpeechNode.html")]
    public sealed class FastSpeechNode : Node, IDialogueNode, IPerformDelayedClone
    {
        public static string ParentCreationMenu => "Dialogue";

        [HideInInspector] public Node Next;
        [HideInInspector] public string m_text;

        public override bool PersonDependent => true;

        public string Text { get => m_text; set { m_text = value; } }
        public List<Option> Options { get => null; set { return; } }

        public override string GetClassName() => "fastSpeechNode";
        public override string GetTitle() => "Dialogue (Optionless)";

        protected override void OnPass(DialogueFlowContext context)
        {
            context.ClearSpeech();

            if (Next == null) return;

            Next.Reach(context);
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            context.Text = Text;
        }

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Internal(int atPort)
        {
            Next = null;
        }
        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {
            if (Next != null) result.Add((0, Next));
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Next.Traverse(action);
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            m_text = dataToRead.Data;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.Data = m_text;
        }

        public void DelayedClone(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (Next != null) Next = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}