using com.absence.dialoguesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component with the responsibility of using the input comes from player (uses legacy input system of unity) on the dialogue.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Instance Extensions/Dialogue Input Handler (Legacy)")]
    [DisallowMultipleComponent]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInputHandler_Legacy.html")]
    public class DialogueInputHandler_Legacy : DialogueExtensionBase
    {
        protected override void OnDialogueUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_instance.Player.State == DialoguePlayer.PlayerState.WaitingForSkip)
                m_instance.Player.Continue();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Input Handler (Legacy)")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueInputHandler_Legacy>();
        }
#endif
    }

}