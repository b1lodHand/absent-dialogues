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
            ExportSelectedDialogue();
        }

        [MenuItem("Assets/absencee_/absent-dialogues/Export Selected Dialogue")]
        static void Export_AssetMenuItem()
        {
            ExportSelectedDialogue();
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


        [MenuItem("absencee_/absent-dialogues/Import New Dialogue")]
        static void Import()
        {
            ImportNewDialogue();
        }

        public static void ImportNewDialogue()
        {
            string jsonFilePath = EditorUtility.OpenFilePanel("Select a Valid Json File", "", "json");

            DialogueData data;
            data = JsonHelper.ReadFromJson(FileHelper.ReadFromFile(jsonFilePath));

            string dialogueCreationPath = EditorUtility.OpenFolderPanel("Select a Location for New Dialogue", "", "");

            while (!AssetDatabase.IsValidFolder(dialogueCreationPath))
            {
                dialogueCreationPath = dialogueCreationPath.Remove(0, 1);

                if (dialogueCreationPath.Length == 0) break;
            }

            string fullPath = $"{dialogueCreationPath}/{data.DefaultDialogueName}.asset";

            DialogueImporter.Import(data, fullPath);

            Debug.Log("Imported dialogue successfully!");
        }

        public static void ExportSelectedDialogue()
        {
            if (Selection.activeObject == null)
            {
                Debug.LogWarning("No object selected!");
                return;
            }

            UnityEngine.Object selectedObject = Selection.activeObject;

            if (selectedObject is not Dialogue dialogue)
            {
                Debug.LogWarning("Selected object is not a dialogue!");
                return;
            }

            ExportDialogue(dialogue);
        }

        public static void ExportDialogue(Dialogue target)
        {
            if (target == null)
            {
                Debug.LogWarning("The dialogue you wanted to export is null!");
                return;
            }

            DialogueData data = DialogueExporter.Export(target);
            string path = EditorUtility.SaveFilePanel("Save Generated Dialogue Data", "", "New Dialogue Data.json", "json");

            if (path.Length == 0)
            {
                Debug.LogWarning("Invalid path!");
                return;
            }

            FileHelper.WriteToFile(JsonHelper.GenerateJsonFrom(data), path);
            Debug.Log($"New dialogue data saved to: {path}");
        }
    }
}
