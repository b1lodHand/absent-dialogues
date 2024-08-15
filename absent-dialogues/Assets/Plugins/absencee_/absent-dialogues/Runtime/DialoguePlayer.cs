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
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialoguePlayer.html")]
    public class DialoguePlayer
    {
        /// <summary>
        /// Shows what state the dialogue player is in.
        /// </summary>
        public enum PlayerState
        {
            /// <summary>
            /// The player is not displaying any dialogue or the current node has no text.
            /// </summary>
            NoText = 0,
            /// <summary>
            /// The player is displaying a speech which has some options and waiting for player to pick an option.
            /// </summary>     
            WaitingForOption = 1,
            /// <summary>
            /// The player is displaying a speech without any options and waiting for the player to skip it.
            /// </summary>
            WaitingForInput = 2,
            /// <summary>
            /// The player's last node was a <see cref="Node.ExitDialogueAfterwards"/>.
            /// </summary>
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

        [SerializeField, Readonly] private Node m_currentNode;
        [SerializeField, Readonly] private VariableBank m_blackboardBank;
        [SerializeField, Readonly] private Blackboard m_blackboard;
        [SerializeField, Readonly] private DialogueFlowContext m_context;

        /// <summary>
        /// Person who speaks.
        /// </summary>
        public Person Speaker => m_currentNode.Person;

        /// <summary>
        /// Additional data of the current node.
        /// </summary>
        public ExtraDialogueData ExtraDialogueData => m_context.ExtraData;

        /// <summary>
        /// Speech of the current node.
        /// </summary>
        public string Text => m_context.Text;

        /// <summary>
        /// Options of the current node, if there is any.
        /// </summary>
        public List<OptionHandle> OptionIndexPairs => m_context.OptionIndexPairs;

        public DialogueFlowContext Context => m_context;

        /// <summary>
        /// Use to check if current node has any text.
        /// </summary>
        public bool HasText => (m_context.HasText);

        /// <summary>
        /// Use to check if current node is a <see cref="FastSpeechNode"/> or not.
        /// </summary>
        public bool HasOptions => (m_context.HasOptions);

        /// <summary>
        /// Use to check if current node <see cref="Node.PersonDependent"/> or not.
        /// </summary>
        public bool HasPerson => (m_currentNode.PersonDependent);


        /// <summary>
        /// Action which will get invoked when <see cref="DialoguePlayer.Continue(object[])"/> gets called.
        /// </summary>
        public event Action<PlayerState> OnContinue;

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/>.
        /// </summary>
        /// <param name="dialogue">The original dialogue to clone from.</param>
        public DialoguePlayer(Dialogue dialogue)
        {
            m_dialogue = dialogue.Clone();

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            TeleportToRoot();
            m_state = PlayerState.NoText;
        }

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/> with an overridden people list.
        /// </summary>
        /// <param name="dialogue">The original dialogue to clone from.</param>
        /// <param name="overridePeople">The list of new people.</param>
        public DialoguePlayer(Dialogue dialogue, List<Person> overridePeople)
        {
            m_dialogue = dialogue.Clone();

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            m_dialogue.OverridePeople(overridePeople);

            TeleportToRoot();
            m_state = PlayerState.NoText;
        }

        /// <summary>
        /// Teleports the flow to the <see cref="RootNode"/> of the dialogue clone.
        /// </summary>
        public void TeleportToRoot()
        {
            m_context = new();

            m_dialogue.Initialize(m_context);
            m_currentNode = m_dialogue.LastOrCurrentNode;
        }

        /// <summary>
        /// Use to progress in the target dialogue wih some optional data.
        /// </summary>
        /// <param name="passData">
        /// Anything that you want to pass as data. (e.g. <see cref="DecisionSpeechNode"/> uses the [0] element to get the selected option index.)
        /// </param>
        public void Continue()
        {
            if (m_context.WillExit)
            {
                m_state = PlayerState.WillExit;
                Pass(m_context);

                OnContinue?.Invoke(m_state);
                return;
            }

            Pass(m_context);

            if (!m_context.HasText)
            {
                m_state = PlayerState.NoText;
                OnContinue?.Invoke(m_state);
                return;
            }

            if (!m_context.HasOptions) m_state = PlayerState.WaitingForInput;
            else m_state = PlayerState.WaitingForOption;

            OnContinue?.Invoke(m_state);
        }

        void Pass(DialogueFlowContext context)
        {
            m_dialogue.Pass(context);
            m_currentNode = m_dialogue.LastOrCurrentNode;
        }
    }
}
