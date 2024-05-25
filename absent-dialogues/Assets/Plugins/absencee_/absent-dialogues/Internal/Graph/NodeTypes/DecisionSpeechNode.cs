using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    public sealed class DecisionSpeechNode : Node, IContainSpeech, IPerformDelayedClone
    {
        [SerializeField] private AdditionalSpeechData m_additionalData;

        [Space(10)]

        public List<Option> Options = new List<Option>();
        [HideInInspector] public string Speech;

        public override bool PersonDependent => true;

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

        public string GetSpeech() => Speech;
        public List<Option> GetOptions() => Options;
        public AdditionalSpeechData GetAdditionalSpeechData() => m_additionalData;

        public void DelayedClone(Dialogue originalDialogue)
        {
            Options = Options.ConvertAll(opt => opt.Clone(Blackboard.Bank));

            Options.ForEach(opt =>
            {
                opt.LeadsTo = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(opt.LeadsTo)];
            });
        }
    }

    [System.Serializable]
    public class Option
    {
        [HideInInspector] public string Speech;
        [HideInInspector] public bool UseShowIf = false;
        [HideInInspector] public VariableComparer ShowIf;
        [HideInInspector] public Node LeadsTo;
        public AdditionalSpeechData AdditionalData;

        public Option Clone(VariableBank overrideBank)
        {
            Option clone = new Option();
            clone.Speech = Speech;
            clone.UseShowIf = UseShowIf;
            clone.ShowIf = ShowIf.Clone(overrideBank);
            clone.LeadsTo = LeadsTo;
            clone.AdditionalData = AdditionalData;

            return clone;
        }
    }
}