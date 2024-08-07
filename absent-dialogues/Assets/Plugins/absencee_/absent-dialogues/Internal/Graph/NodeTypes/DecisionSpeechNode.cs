using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    /// <summary>
    /// Node which displays a speech with options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html")]
    public sealed class DecisionSpeechNode : Node, IContainSpeech, IPerformDelayedClone, IContainVariableManipulators
    {
        [SerializeField] private AdditionalSpeechData m_additionalData;

        [Space(10)]
        
        [Tooltip("All of the options of this node.")] public List<Option> Options = new List<Option>();

        [HideInInspector] public string Speech;

        public override bool PersonDependent => true;

        string IContainSpeech.Speech { get => Speech; set { Speech = value; } }
        List<Option> IContainSpeech.Options { get => Options; set { Options = value; } }

        public override string GetClassName() => "decisionSpeechNode";
        public override string GetTitle() => "Decision Speech";

        protected override void Pass_Inline(params object[] passData)
        {
            var optionSelected = (int)passData[0];

            if (Options.Count == 0) return;
            if (Options[optionSelected].LeadsTo == null) return;

            Options[optionSelected].LeadsTo.Reach();
            SetState(NodeState.Past);
        }
        protected override void Reach_Inline()
        {

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

        public AdditionalSpeechData GetAdditionalSpeechData() => m_additionalData;

        public void DelayedClone(Dialogue originalDialogue)
        {
            Options = Options.ConvertAll(opt => opt.Clone(Blackboard.Bank));

            Options.ForEach(opt =>
            {
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

        protected override void OnValidate()
        {
            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));
            });

            base.OnValidate();
        }
    }
}