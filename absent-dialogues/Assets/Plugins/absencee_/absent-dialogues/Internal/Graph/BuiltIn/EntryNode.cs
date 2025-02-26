using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which is essential if you want to have a dialogue graph.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.RootNode.html")]
    [MovedFrom("RootNode")]
    public sealed class EntryNode : Node
    {
        public static string CreationMenuName => NaN;

        [HideInInspector] public Node Next;

        public override string Title => "Entry";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/EntryNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            return Next;
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            Next = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            result.Add(Next);
        }

        public override string GetDefaultInputPortName()
        {
            return null;
        }
        public override List<string> GetDefaultOutputPortNames()
        {
            return new List<string>() { "Start" };
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            base.OnCloning(originalDialogue, clonedDialogue);

            if (Next != null) Next = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}