using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Lets you progress in a dialogue easily.
    /// </summary>
    [System.Serializable]
    public class DialoguePlayer
    {
        /// <summary>
        /// Shows what state the dialogue is in.
        /// </summary>
        public enum DialoguePlayerState
        {
            NoSpeech = 0,
            WaitingForOption = 1,
            WaitingForSkip = 2,
            WillExit = 3,
        }

        [SerializeField, Readonly] private DialoguePlayerState m_state;
        public DialoguePlayerState State => m_state;

        [SerializeField, Readonly] private Dialogue m_dialogue;
        public Dialogue Dialogue => m_dialogue;

        [SerializeField, Readonly] private VariableBank m_blackboardBank;

        [SerializeField, Readonly] private Blackboard m_blackboard;

        public event Action<DialoguePlayerState> OnContinue;

        public Person Speaker => m_dialogue.LastOrCurrentNode.Person;
        public AdditionalSpeechData AdditionalSpeechData => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetAdditionalSpeechData();
        public string Speech => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetSpeech();
        public List<Option> Options => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetOptions();
        public bool HasSpeech => (m_dialogue.LastOrCurrentNode is IContainSpeech);

        bool m_overriden = false;

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/>.
        /// </summary>
        /// <param name="dialogue"></param>
        public DialoguePlayer(Dialogue dialogue)
        {
            m_dialogue = dialogue.Clone();
            //m_dialogue = dialogue;

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            m_dialogue.Initialize();
            m_state = DialoguePlayerState.NoSpeech;
        }

        /// <summary>
        /// Use to progress in the target dialogue wih some optional data.
        /// </summary>
        /// <param name="passData"></param>
        public void Continue(params object[] passData)
        {
            if (m_dialogue.LastOrCurrentNode.ExitDialogAfterwards)
            {
                m_state = DialoguePlayerState.WillExit;
                m_dialogue.Pass(passData);
                return;
            }

            m_dialogue.Pass(passData);

            if (!(m_dialogue.LastOrCurrentNode is IContainSpeech)) m_state = DialoguePlayerState.NoSpeech;
            else if (m_dialogue.LastOrCurrentNode is FastSpeechNode) m_state = DialoguePlayerState.WaitingForSkip;
            else if (m_dialogue.LastOrCurrentNode is DecisionSpeechNode) m_state = DialoguePlayerState.WaitingForOption;

            OnContinue?.Invoke(m_state);
        }

        /// <summary>
        /// Overrides the people in the target dialogue. Won't work if it is already overriden.
        /// </summary>
        /// <param name="overridePeople"></param>
        public void OverridePeople(List<Person> overridePeople)
        {
            if (m_overriden) return;
            m_dialogue.OverridePeople(overridePeople);
        }


        /// <summary>
        /// Reverts any overriding process.
        /// </summary>
        public void RevertPeople()
        {
            if (!m_overriden) return;

            m_dialogue.ResetPeopleList();
            m_overriden = false;
        }
    }
}
