using com.absence.dialoguesystem.editor.backup.utilities;
using com.absence.dialoguesystem.runtime.backup.data;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class EditorJobsHelper
    {
        [MenuItem("absencee_/absent-dialogues/Export Selected Dialogue")]
        static void Export_DirectMenuItem()
        {
            BackupSystem.ExportSelectedDialogue();
        }

        [MenuItem("Assets/absencee_/absent-dialogues/Export Selected Dialogue")]
        static void Export_AssetMenuItem()
        {
            BackupSystem.ExportSelectedDialogue();
        }

        [MenuItem("absencee_/absent-dialogues/Import New Dialogue")]
        static void Import()
        {
            BackupSystem.ImportNewDialogue();
        }

        [MenuItem("absencee_/absent-dialogues/Export Selected Dialogue", validate = true)]
        static bool Export_DirectMenuItemValidation()
        {
            if(Selection.activeObject == null) return false;
            return Selection.activeObject is Dialogue;
        }

        [MenuItem("Assets/absencee_/absent-dialogues/Export Selected Dialogue", validate = true)]
        static bool Export_AssetMenuItemValidation()
        {
            if (Selection.activeObject == null) return false;
            return Selection.activeObject is Dialogue;
        }
    }
}
