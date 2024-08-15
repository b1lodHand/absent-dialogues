using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.examples
{
    /// <summary>
    /// A simple component to show how to use the system in the recommended way.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    public class Demo_GUI : DialogueExtensionBase
    {
        const string K_MISSIONPENDING = "b_missionPending";     // using constants
        const string K_MISSIONDONE = "b_missionDone";           // to avoid any
        const string K_MISSIONCOMMITTED = "b_missionCommitted"; // string-prone issues.

        VariableBank m_blackboardBank; // holding a reference to our blackboard's bank.

        bool m_pending = false;   // using variables
        bool m_done = false;      // to cache
        bool m_committed = false; // needed values.

        public override void OnAfterCloning()
        {
            m_blackboardBank = m_instance.Player.ClonedDialogue.Blackboard.Bank; // we're doing this to easily reach the bank in the future.

            // adding listeners to needed variables. This way, we will be notified when the get changed.
            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONPENDING, OnPendingChanged);
            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONDONE, OnDoneChanged);
            m_blackboardBank.AddValueChangeListenerToBoolean(K_MISSIONCOMMITTED, OnCommitChanged);

            // we're setting the initial values of the our variables.
            m_blackboardBank.TryGetBoolean(K_MISSIONPENDING, out m_pending);
            m_blackboardBank.TryGetBoolean(K_MISSIONDONE, out m_done);
            m_blackboardBank.TryGetBoolean(K_MISSIONCOMMITTED, out m_committed);
        }

        /* methods for handling the value change events */
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
            // For this example, I've used IMGUI system. Because it is the fastest option to setup. I wouldn't recommend using it
            // in your games, tho.

            // drawing a simple button to let any tester re-enter the dialogue.
            if ((!m_instance.InDialogue) && GUILayout.Button("Talk"))
            {
                m_instance.EnterDialogue();
            }

            if (!m_pending) return; // checking if we've gotten the mission.
            if (m_done) return; // checking if we've already done the mission.
            if (m_committed) return; // checking if we've told the NPC that we've done the mission already.

            // drawing the 'Get apples' button to let users complete the mission easily.
            if (GUILayout.Button("Get apples."))
            {
                m_blackboardBank.SetBoolean(K_MISSIONDONE, true);
            }

            /* WE'RE DONE! */
        }
    }

}