using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private bool m_startOnAwake = false;

        Node m_displayedSpeechNode;
        bool m_inDialogue = false;

        private void Start()
        {
            Initialize(null);
            if(m_startOnAwake) EnterDialog();
        }

        private void Update()
        {
            if (!m_inDialogue) return;
            if (m_displayedSpeechNode != null)
            {
                if (m_displayedSpeechNode is FastSpeechNode && Input.GetKeyDown(KeyCode.Space))
                {
                    if (m_dialogue.LastOrCurrentNode.ExitDialogAfterwards) ExitDialog();
                    m_displayedSpeechNode = null;
                    m_dialogue.LastOrCurrentNode.Pass();
                }

                return;
            }

            var currentNode = m_dialogue.LastOrCurrentNode;
            var speechNode = currentNode as ISpeechNode;
            if (speechNode == null)
            {
                if (m_dialogue.LastOrCurrentNode.ExitDialogAfterwards) ExitDialog();
                m_dialogue.LastOrCurrentNode.Pass();
                return;
            }

            if (currentNode is FastSpeechNode f) DialogueDisplayer.Instance.WriteFast(f.Person, speechNode.GetSpeech());
            else if (currentNode is DecisionSpeechNode d) DialogueDisplayer.Instance.WriteDecisive(d.Person, speechNode.GetSpeech(), speechNode.GetOptionSpeeches());

            m_displayedSpeechNode = currentNode;
        }

        public void Initialize(List<Person> people)
        {
            if (m_dialogue == null) return;
            m_dialogue.Bind(people);
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            m_dialogue.ResetProgress();
#endif
        }

        public void EnterDialog()
        {
            m_inDialogue = true;
            DialogueDisplayer.Instance.EnterDialog(this);
        }

        public void ExitDialog()
        {
            m_dialogue.Cleanup();
            m_inDialogue = false;
            DialogueDisplayer.Instance.ExitDialog();
        }

        public void OptionReceived(int index)
        {
            if (m_dialogue.LastOrCurrentNode.ExitDialogAfterwards) ExitDialog();
            m_dialogue.LastOrCurrentNode.Pass(index);
            m_displayedSpeechNode = null;
        }
    }

}