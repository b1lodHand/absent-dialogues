using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using com.absence.variablesystem.banksystembase;
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
        [SerializeField, Readonly] private Dialogue m_dialogue;
        /// <summary>
        /// The dialogue cloned from the original one from constructor.
        /// </summary>
        public Dialogue ClonedDialogue => m_dialogue;

        [SerializeField, Readonly] private Node m_frame;
        [SerializeField, Readonly] private VariableBank m_blackboardBank;
        [SerializeField, Readonly] private Blackboard m_blackboard;
        [SerializeField, Readonly] private DialogueFlowContext m_context;

        public DialogueFlowContext Context => m_context;
        public Node Frame => m_frame;

        /// <summary>
        /// Action which will get invoked when <see cref="DialoguePlayer.Continue(object[])"/> gets called.
        /// </summary>
        public event Action<DialoguePlayer> OnContinue;

        /// <summary>
        /// Use to create a new <see cref="DialoguePlayer"/> with an overridden people list.
        /// </summary>
        /// <param name="dialogue">The original dialogue to clone from.</param>
        /// <param name="overridePeople">The list of new people.</param>
        public DialoguePlayer(Dialogue dialogue, List<Person> overridePeople = null)
        {
            m_dialogue = dialogue;

            m_blackboard = m_dialogue.Blackboard;
            m_blackboardBank = m_dialogue.Blackboard.Bank;

            Initialize();
        }

        /// <summary>
        /// Teleports the flow to the <see cref="EntryNode"/> of the dialogue clone.
        /// </summary>
        public void TeleportToRoot(bool invokeProgressEvent = true)
        {
            DoTeleportToRoot();
            if (invokeProgressEvent) OnContinue?.Invoke(this);
        }

        private void DoTeleportToRoot()
        {
            m_frame = m_dialogue.Entry;
        }

        public void Initialize()
        {
            ClearContext();

            m_dialogue.ValidateNodes();
            m_dialogue.ResetNodeStates();
            DoTeleportToRoot();
        }

        public void ClearContext()
        {
            m_context = new();
        }

        /// <summary>
        /// Use to progress in the target dialogue wih some optional data.
        /// </summary>
        /// <param name="passData">
        /// Anything that you want to pass as data. (e.g. <see cref="PromptNode"/> uses the [0] element to get the selected option index.)
        /// </param>
        public void Continue()
        {
            Progress();
        }

        void Progress()
        {
            switch (m_context.State)
            {
                case DialogueFlowContext.ContextState.Pass:
                    m_frame.Reach(m_context);
                    break;
                case DialogueFlowContext.ContextState.Reach:
                    Node next = m_frame.Pass(m_context);
                    if (next == null && (!m_context.WillExit))
                        throw new Exception("There is an empty output port!");
                    m_frame = next;
                    break;
                default:
                    return;
            }

            OnContinue?.Invoke(this);
        }
    }
}
