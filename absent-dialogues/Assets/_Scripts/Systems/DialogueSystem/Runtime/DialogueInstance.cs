
using com.absence.dialoguesystem;
using UnityEngine;

namespace com.absence.dialogsystem.runtime
{
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private bool m_startOnAwake = false;

        Node m_displayedSpeechNode;
        bool m_inDialogue = false;

        private void Start()
        {
            Initialize();
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

            if (currentNode is FastSpeechNode) DialogueDisplayer.Instance.WriteFast(speechNode.GetSpeech());
            else if (currentNode is DecisionSpeechNode) DialogueDisplayer.Instance.WriteDecisive(speechNode.GetSpeech(), speechNode.GetOptionSpeeches());

            m_displayedSpeechNode = currentNode;
        }

        public void Initialize(/* will get people info */)
        {
            if (m_dialogue == null) return;
            m_dialogue.Bind();
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