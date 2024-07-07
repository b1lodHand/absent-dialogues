using com.absence.dialoguesystem.internals;
using com.absence.variablesystem;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// A script responsible for handling the creation of a dialogue.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.DialogueCreationHandler.html")]
    public static class DialogueCreationHandler
    {
        [MenuItem("Assets/Create/absencee_/absent-dialogues/Dialogue", priority = 0)]
        static void CreateDialogue()
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

        internal class CreateDialogueEndNameEditAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var itemCreated = ScriptableObject.CreateInstance<Dialogue>();
                AssetDatabase.CreateAsset(itemCreated, pathName);

                var blackboard = new Blackboard();
                var blackboardBank = ScriptableObject.CreateInstance<VariableBank>();

                blackboardBank.name = $"{itemCreated.name} Blackboard VB";
                blackboardBank.ForExternalUse = true;

                AssetDatabase.AddObjectToAsset(blackboardBank, itemCreated);

                blackboard.Bank = blackboardBank;

                itemCreated.Blackboard = blackboard;

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Selection.activeObject = itemCreated;
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Dialogue item = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
                ScriptableObject.DestroyImmediate(item);
            }
        }
    }

}