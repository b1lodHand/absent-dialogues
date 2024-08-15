using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which let's you create more and seperate routes.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DialoguePartNode.html")]
    public sealed class DialoguePartNode : Node, IPerformDelayedClone
    {
        [HideInInspector] public Node Next;
        public string DialoguePartName;
        public override bool DisplayState => false;
        public override string GetClassName() => "dialoguePartNode";
        public override string GetTitle() => $"Dialogue Part";

        protected override void Pass_Inline(DialogueFlowContext context)
        {
            if (Next != null) Next.Reach(context);
        }
        protected override void Reach_Inline(DialogueFlowContext context)
        {

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

        public override string GetInputPortNameForCreation()
        {
            return null;
        }

        public void DelayedClone(Dialogue originalDialogue)
        {
            if (Next != null) Next = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            DialoguePartName = dataToRead.DialoguePartName;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.DialoguePartName = DialoguePartName;
        }
    }

}