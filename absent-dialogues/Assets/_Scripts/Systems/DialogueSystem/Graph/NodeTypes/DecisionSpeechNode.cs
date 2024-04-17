using com.absence.variablesystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    public class DecisionSpeechNode : Node, ISpeechNode
    {
        [HideInInspector] public List<Option> Options = new List<Option>();
        [HideInInspector] public string Speech;

        public override bool PersonDependent => true;

        public override string GetClassName() => "decisionSpeechNode";
        public override string GetTitle() => "Decisive Speech";

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

        public override Node Clone()
        {
            DecisionSpeechNode node = Instantiate(this);
            //node.Options.ConvertAll(n => n.Clone());
            return node;
        }
        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>();
        }

        public string GetSpeech() => Speech;
        public string[] GetOptionSpeeches() => Options.ToList().ConvertAll(n => n.Speech).ToArray();
    }

    [System.Serializable]
    public class Option
    {
        public string Speech;
        public bool UseShowIf = false;
        public VariableComparer ShowIf;
        public Node LeadsTo;
    }
}