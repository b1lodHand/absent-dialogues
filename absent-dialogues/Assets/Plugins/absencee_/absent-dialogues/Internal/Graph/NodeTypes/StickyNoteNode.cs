using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which contains a user defined string.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.StickyNoteNode.html")]
    public sealed class StickyNoteNode : Node
    {
        public static string ParentCreationMenu => "Misc";

        [HideInInspector] public string m_text;

        public override bool DisplayState => false;
        public override bool ShowInMinimap => false;

        public override string GetClassName() => "stickyNoteNode";

        public override string GetTitle() => "Sticky Note";

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void OnPass(DialogueFlowContext context)
        {
            
        }

        protected override void OnReach(DialogueFlowContext context)
        {
            
        }

        protected override void RemoveNextNode_Internal(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => null;
        public override List<string> GetOutputPortNamesForCreation() => new();

        public ExtraDialogueData GetExtraData()
        {
            throw new System.NotImplementedException();
        }
    }

}