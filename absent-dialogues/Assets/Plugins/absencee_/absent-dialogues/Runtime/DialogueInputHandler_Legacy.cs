using com.absence.dialoguesystem;
using com.absence.dialoguesystem.internals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInputHandler_Legacy : DialogueExtensionBase
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_instance.Player.State == DialoguePlayer.DialoguePlayerState.WaitingForSkip) 
            m_instance.Player.Continue();
    }

    public override void OnHandleAdditionalData(AdditionalSpeechData data)
    {
        return;
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
