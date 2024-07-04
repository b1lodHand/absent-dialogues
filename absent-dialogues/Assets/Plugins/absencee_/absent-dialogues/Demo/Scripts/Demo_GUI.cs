using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.examples
{
    [RequireComponent(typeof(DialogueInstance))]
    public class Demo_GUI : DialogueExtensionBase
    {
        const string K_MISSIONPENDING = "b_missionPending";
        const string K_MISSIONDONE = "b_missionDone";
        const string K_MISSIONCOMMITTED = "b_missionCommitted";

        VariableBank m_blackboardBank;

        bool m_pending = false;
        bool m_done = false;
        bool m_committed = false;

        public override void OnAfterCloning()
        {
            m_blackboardBank = m_instance.Player.ClonedDialogue.Blackboard.Bank;

            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONPENDING, OnPendingChanged);
            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONDONE, OnDoneChanged);
            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONCOMMITTED, OnCommitChanged);

            m_blackboardBank.TryGetBoolean(K_MISSIONPENDING, out m_pending);
            m_blackboardBank.TryGetBoolean(K_MISSIONDONE, out m_done);
            m_blackboardBank.TryGetBoolean(K_MISSIONCOMMITTED, out m_committed);
        }

        private void OnCommitChanged(VariableValueChangedCallbackContext<bool> context)
        {
            m_committed = context.newValue;
        }

        private void OnDoneChanged(VariableValueChangedCallbackContext<bool> context)
        {
            m_done = context.newValue;
        }

        private void OnPendingChanged(VariableValueChangedCallbackContext<bool> context)
        {
            m_pending = context.newValue;
        }

        private void OnGUI()
        {
            if (!m_pending) return;
            if (m_done) return;
            if (m_committed) return;

            if (GUI.Button(new Rect(10f, 10f, 100f, 20f), "Get apples."))
            {
                m_blackboardBank.SetBoolean(K_MISSIONDONE, true);
            }
        }
    }

}