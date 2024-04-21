using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Use to manage a single dialogue player.
    /// </summary>
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private bool m_startOnAwake = false;
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private List<Person> m_overridePeople;

        private DialoguePlayer m_player;
        public DialoguePlayer Player => m_player;

        bool m_inDialogue = false;

        private void Start()
        {
            if(m_dialogue != null) m_player = new DialoguePlayer(m_dialogue);
            if (m_startOnAwake) EnterDialogue();
        }

        private void Update()
        {
            if (!m_inDialogue) return;

            switch (m_player.State)
            {
                case DialoguePlayer.DialoguePlayerState.Idle:
                    m_player.Continue();
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForOption:
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForSkip:
                    if (CheckSkipInput()) m_player.Continue();
                    break;

                case DialoguePlayer.DialoguePlayerState.WillExit:
                    ExitDialogue();
                    break;

                default:
                    ExitDialogue();
                    break;
            }
        }

        private bool CheckSkipInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool EnterDialogue()
        {
            if (!DialogueDisplayer.Instance.Occupy(this)) return false;

            if (m_overridePeople.Count > 0) m_player.OverridePeople(m_overridePeople);
            m_inDialogue = true;

            return true;
        }

        public void ExitDialogue()
        {
            m_inDialogue = false;
            m_player.RevertPeople();

            DialogueDisplayer.Instance.Release();
        }

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.RevertPeople();
        }
    }
}