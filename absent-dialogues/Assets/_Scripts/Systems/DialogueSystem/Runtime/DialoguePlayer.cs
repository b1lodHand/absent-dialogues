using com.absence.dialoguesystem.internals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class DialoguePlayer
    {
        public enum DialoguePlayerState
        {
            Idle = 0,
            WaitingForOption = 1,
            WaitingForSkip = 2,
            WillExit = 3,
        }

        Dialogue m_dialogue;
        bool m_overriden = false;

        DialoguePlayerState m_state;
        public DialoguePlayerState State => m_state;

        public event Action<DialoguePlayerState> OnContinue;

        public Person Speaker => m_dialogue.LastOrCurrentNode.Person;
        public string Speech => (m_dialogue.LastOrCurrentNode as ISpeechNode).GetSpeech();
        public string[] Options => (m_dialogue.LastOrCurrentNode as ISpeechNode).GetOptions();

        public DialoguePlayer(Dialogue dialogue)
        {
            m_dialogue = dialogue;
            m_dialogue.Bind();

            m_state = DialoguePlayerState.Idle;
        }

        public void Continue(params object[] passData)
        {
            if (m_dialogue.WillExit)
            {
                m_state = DialoguePlayerState.WillExit;
                m_dialogue.Continue(passData);
                return;
            }

            m_dialogue.Continue(passData);

            if (!m_dialogue.HasSpeech) m_state = DialoguePlayerState.Idle;
            else if (m_dialogue.LastOrCurrentNode is FastSpeechNode) m_state = DialoguePlayerState.WaitingForSkip;
            else if (m_dialogue.LastOrCurrentNode is DecisionSpeechNode) m_state = DialoguePlayerState.WaitingForOption;

            OnContinue?.Invoke(m_state);
        }

        public void OverridePeople(List<Person> overridePeople)
        {
            if (m_overriden) return;
            m_dialogue.OverridePeople(overridePeople);
        }

        public void RevertPeople()
        {
            if (!m_overriden) return;

            m_dialogue.ResetPeopleList();
            m_overriden = false;
        }
    }
}
