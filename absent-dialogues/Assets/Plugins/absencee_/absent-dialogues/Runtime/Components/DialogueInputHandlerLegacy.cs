using com.absence.dialoguesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem.builtin
{
    /// <summary>
    /// A small component with the responsibility of using the input comes from player (uses legacy input system of unity) on the dialogue.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Instance Extensions/Dialogue Input Handler (Legacy)")]
    [DisallowMultipleComponent]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInputHandler_Legacy.html")]
    public class DialogueInputHandlerLegacy : DialogueExtensionBase
    {
        bool inputNeeded = false;

        public override void OnInitialize()
        {
            inputNeeded = false;
        }

        public override void OnInstanceUpdate()
        {
            if (inputNeeded && Input.GetKeyDown(KeyCode.Space))
            {
                m_instance.ForceContinue();
            }
        }

        public override void OnProgress(Node frame, DialogueFlowContext context)
        {
            if (context.HasText && (!context.HasOptions)) inputNeeded = true;
            else inputNeeded = false;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Input Handler (Legacy)")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueInputHandlerLegacy>();
        }
#endif
    }

}