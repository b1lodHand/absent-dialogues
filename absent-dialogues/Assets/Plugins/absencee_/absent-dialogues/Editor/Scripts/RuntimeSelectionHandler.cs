using System;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// It handles the selection events of <see cref="IUseDialogueInScene"/> game objects.
    /// </summary>
    [InitializeOnLoad]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.RuntimeSelectionHandler.html")]
    public static class RuntimeSelectionHandler
    {
        static IUseDialogueInScene s_lastSelectedUser;

        static RuntimeSelectionHandler()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    DialogueEditorWindow.OnGUIDelayCall += OnSelectionChanged;
                    break;
            }
        }

        private static void OnSelectionChanged()
        {
            if (s_lastSelectedUser != null)
            {
                s_lastSelectedUser.OnValidation -= OnUserValidation;
                s_lastSelectedUser = null;
            }

            GameObject gameObject = Selection.activeGameObject;

            if (!Selection.activeGameObject) return;
            if (!gameObject.TryGetComponent(out IUseDialogueInScene displayer)) return;
            if (displayer.ReferencedDialogue == null) return;

            s_lastSelectedUser = displayer;
            s_lastSelectedUser.OnValidation += OnUserValidation;

            if (Application.isPlaying)
            {
                DialogueEditorWindow.PopulateDialogueView(displayer.ClonedDialogue);
            }

            else
            {
                DialogueEditorWindow.PopulateDialogueView(displayer.ReferencedDialogue);
                DialogueEditorWindow.SaveLastDialogue();
            }
        }

        private static void OnUserValidation()
        {
            if (Application.isPlaying)
                return;

            DialogueEditorWindow.PopulateDialogueView(s_lastSelectedUser.ReferencedDialogue);
            DialogueEditorWindow.SaveLastDialogue();
        }
    }
}
