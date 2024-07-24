using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which contains a user defined string.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.StickyNoteNode.html")]
    public sealed class StickyNoteNode : Node, IContainSpeech
    {
        [HideInInspector] public string Speech;

        public override bool DisplayState => false;
        public override bool ShowInMinimap => false;

        string IContainSpeech.Speech { get => Speech; set { Speech = value; } }
        public List<Option> Options { get => null; set { return; } }

        public override string GetClassName() => "stickyNoteNode";

        public override string GetTitle() => "Sticky Note";

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void Pass_Inline(params object[] passData)
        {
            
        }

        protected override void Reach_Inline()
        {
            
        }

        protected override void RemoveNextNode_Inline(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => null;
        public override List<string> GetOutputPortNamesForCreation() => new();

        public AdditionalSpeechData GetAdditionalSpeechData()
        {
            throw new System.NotImplementedException();
        }
    }

}