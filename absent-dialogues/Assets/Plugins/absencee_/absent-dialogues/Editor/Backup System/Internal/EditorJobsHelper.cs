using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.editor.backup.utilities;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class EditorJobsHelper
    {
        [MenuItem("absencee_/absent-dialogues/Export Selected Dialogue")]
        static void Export_DirectMenuItem()
        {
            Export();
        }

        [MenuItem("Assets/absencee_/absent-dialogues/Export Selected Dialogue")]
        static void Export_AssetMenuItem()
        {
            Export();
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

        static void Export()
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

            DialogueData data = DialogueExporter.Export(dialogue);
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
