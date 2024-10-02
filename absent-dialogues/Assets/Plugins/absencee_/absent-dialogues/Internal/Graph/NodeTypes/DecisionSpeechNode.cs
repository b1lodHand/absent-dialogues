using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    /// <summary>
    /// Node which displays a speech with options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html")]
    public sealed class DecisionSpeechNode : Node, IContainData, IPerformDelayedClone, IContainVariableManipulators
    {
        [SerializeField] private ExtraDialogueData m_extraData;

        [Space(10)]
        
        [Tooltip("All of the options of this node.")] public List<Option> Options = new List<Option>();

        [HideInInspector] public string m_text;

        public override bool PersonDependent => true;

        public string Text { get => m_text; set { m_text = value; } }
        List<Option> IContainData.Options { get => Options; set { Options = value; } }
        public ExtraDialogueData ExtraData { get { return m_extraData; } set { m_extraData = value; } }

        public override string GetClassName() => "decisionSpeechNode";
        public override string GetTitle() => "Dialogue";

        protected override void Pass_Inline(DialogueFlowContext context)
        {
            context.ClearSpeech();

            var optionSelected = context.OptionIndex;

            if (Options.Count == 0) return;
            if (Options[optionSelected].LeadsTo == null) return;

            Options[optionSelected].LeadsTo.Reach(context);
            SetState(NodeState.Past);
        }
        protected override void Reach_Inline(DialogueFlowContext context)
        {
            List<OptionHandle> temp = new();
            Options.ForEach(o =>
            {
                if(!o.IsVisible()) return;

                temp.Add(new OptionHandle(Options.IndexOf(o), o.Text));
            });

            context.Text = Text;
            context.OptionIndexPairs = new(temp);

            temp.Clear();
            temp = null;
        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            Options[atPort].LeadsTo = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Inline(int atPort)
        {
            Options[atPort].LeadsTo = null;
        }
        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            foreach (var o in Options.ToArray())
            {
                if (o != null && o.LeadsTo != null) result.Add((Options.IndexOf(o), o.LeadsTo));
            }
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Options.ForEach(option =>
            {
                option.LeadsTo.Traverse(action);
            });
        }

        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>();
        }

        public ExtraDialogueData GetExtraData() => m_extraData;

        public void DelayedClone(Dialogue originalDialogue)
        {
            Options = Options.ConvertAll(opt => opt.Clone(Blackboard.Bank));

            Options.ForEach(opt =>
            {
                if (opt.LeadsTo == null) return;

                opt.LeadsTo = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(opt.LeadsTo)];
            });
        }

        public List<NodeVariableComparer> GetComparers()
        {
            List<NodeVariableComparer> result = new();

            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer =>
                {
                    result.Add(comparer);
                });
            });

            return result;
        }

        public List<NodeVariableSetter> GetSetters() => null;

        public override void OnValidate()
        {
            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));
            });

            base.OnValidate();
        }
    }
}