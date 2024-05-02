using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    public class FastSpeechNode : Node, ISpeechNode
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private Animation m_animation;

        [HideInInspector] public Node Next;
        [HideInInspector] public string Speech;

        public override bool PersonDependent => true;

        public override string GetClassName() => "fastSpeechNode";
        public override string GetTitle() => "Fast Speech";

        protected override void Pass_Inline(params object[] passData)
        {
            if (Next == null) return;

            Next.Reach();
            SetState(NodeState.Past);
        }
        protected override void Reach_Inline()
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

        public override Node Clone()
        {
            FastSpeechNode node = Instantiate(this);
            node.Next = Next.Clone();
            return node;
        }

        public string GetSpeech() => Speech;
        public string[] GetOptions() => null;
        public AdditionalSpeechData GetAdditionalSpeechData() => new() { AudioClip = m_audioClip, Animation = m_animation };
    }

}