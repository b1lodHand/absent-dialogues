using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    /// <summary>
    /// Node which displays a speech with options.
    /// </summary>
    public sealed class DecisionSpeechNode : Node, IContainSpeech, IPerformDelayedClone, IContainVariableManipulators, IPerformEditorRefresh
    {
        [SerializeField] private AdditionalSpeechData m_additionalData;

        [Space(10)]

        
        [Tooltip("All of the options of this node.")] public List<Option> Options = new List<Option>();

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

        public List<FixedVariableComparer> GetComparers()
        {
            List<FixedVariableComparer> result = new();

            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer =>
                {
                    if (comparer != null) result.Add(comparer);
                });
            });

            return result;
        }

        public List<FixedVariableSetter> GetSetters() => null;

        public void PerformEditorRefresh()
        {
            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer => comparer.SetFixedBank(Blackboard.Bank));
            });
        }
    }
}