using com.absence.dialoguesystem.editor.internals.backup;
using System.IO;
using UnityEditor;
using static com.absence.dialoguesystem.editor.DialogueCreationHandler;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    public static class EditorJobsHelper
    {
        [MenuItem("Assets/Create/absencee_/absent-dialogues/Dialogue", priority = 0)]
        static void CreateDialogue_MenuItem()
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (selectedPath == string.Empty) return;

            while ((!AssetDatabase.IsValidFolder(selectedPath)))
            {
                TrimLastSlash(ref selectedPath);
            }

            CreateDialogueEndNameEditAction create = ScriptableObject.CreateInstance<CreateDialogueEndNameEditAction>();
            var path = Path.Combine(selectedPath, "New Dialogue.asset");
            var icon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image as Texture2D;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, path, icon, null);
        }

        private static void TrimLastSlash(ref string path)
        {
            int lastSlashIndex;
            for (lastSlashIndex = path.Length - 1; lastSlashIndex > 0; lastSlashIndex--)
            {
                if (path[lastSlashIndex] == '/') break;
            }

            path = path.Remove(lastSlashIndex, (path.Length - lastSlashIndex));
        }

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
