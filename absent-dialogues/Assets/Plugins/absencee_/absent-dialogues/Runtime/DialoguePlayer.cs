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
        /// Shows what state the dialogue player is in.
        /// </summary>
        public enum PlayerState
        {
            NoSpeech = 0,
            WaitingForOption = 1,
            WaitingForSkip = 2,
            WillExit = 3,
        }

        [SerializeField, Readonly] private PlayerState m_state;
        /// <summary>
        /// Current state of the player.
        /// </summary>
        public PlayerState State => m_state;

        [SerializeField, Readonly] private Dialogue m_dialogue;
        /// <summary>
        /// The dialogue cloned from the original one from constructor.
        /// </summary>
        public Dialogue ClonedDialogue => m_dialogue;

        [SerializeField, Readonly] private VariableBank m_blackboardBank;
        [SerializeField, Readonly] private Blackboard m_blackboard;

        /// <summary>
        /// Action which will get invoked when <see cref="DialoguePlayer.Continue(object[])"/> gets called.
        /// </summary>
        public event Action<PlayerState> OnContinue;

        /// <summary>
        /// Person who speaks.
        /// </summary>
        public Person Speaker => m_dialogue.LastOrCurrentNode.Person;

        /// <summary>
        /// Additional data of the current node.
        /// </summary>
        public AdditionalSpeechData AdditionalSpeechData => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetAdditionalSpeechData();

        /// <summary>
        /// Speech of the current node.
        /// </summary>
        public string Speech => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetSpeech();

        /// <summary>
        /// Options of the current node, if there is any.
        /// </summary>
        public List<Option> Options => (m_dialogue.LastOrCurrentNode as IContainSpeech).GetOptions();

        /// <summary>
        /// Use to check if current node is a <see cref="IContainSpeech"/> or not.
        /// </summary>
        public bool HasSpeech => (m_dialogue.LastOrCurrentNode is IContainSpeech);

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/>.
        /// </summary>
        /// <param name="dialogue">The original dialogue to clone from.</param>
        public DialoguePlayer(Dialogue dialogue)
        {
            m_dialogue = dialogue.Clone();
            //m_dialogue = dialogue;

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            m_dialogue.Initialize();
            m_state = PlayerState.NoSpeech;
        }

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/> with an overridden people list.
        /// </summary>
        /// <param name="dialogue">The original dialogue to clone from.</param>
        /// <param name="overridePeople">The list of new people.</param>
        public DialoguePlayer(Dialogue dialogue, List<Person> overridePeople)
        {
            m_dialogue = dialogue.Clone();
            //m_dialogue = dialogue;

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            m_dialogue.OverridePeople(overridePeople);

            m_dialogue.Initialize();
            m_state = PlayerState.NoSpeech;
        }

        /// <summary>
        /// Use to progress in the target dialogue wih some optional data.
        /// </summary>
        /// <param name="passData">
        /// Anything that you want to pass as data. (e.g. <see cref="DecisionSpeechNode"/> uses the [0] element to get the selected option index.)
        /// </param>
        public void Continue(params object[] passData)
        {
            if (m_dialogue.LastOrCurrentNode.ExitDialogAfterwards)
            {
                m_state = PlayerState.WillExit;
                m_dialogue.Pass(passData);
                return;
            }

            m_dialogue.Pass(passData);

            if (!(m_dialogue.LastOrCurrentNode is IContainSpeech)) m_state = PlayerState.NoSpeech;
            else if (m_dialogue.LastOrCurrentNode is FastSpeechNode) m_state = PlayerState.WaitingForSkip;
            else if (m_dialogue.LastOrCurrentNode is DecisionSpeechNode) m_state = PlayerState.WaitingForOption;

            OnContinue?.Invoke(m_state);
        }
    }
}
