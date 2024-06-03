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
        private static readonly string k_createPath = "Assets/Scriptables/Dialogue Graphs";

        [MenuItem("absencee_/absent-dialogues/Create Dialogue Graph", priority = 0)]
        static void CreateDialogue()
        {
            CreateDialogueEndNameEditAction create = ScriptableObject.CreateInstance<CreateDialogueEndNameEditAction>();
            var path = Path.Combine(k_createPath, "New Dialogue.asset");
            var icon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image as Texture2D;

            if (!AssetDatabase.IsValidFolder("Assets/Scriptables")) AssetDatabase.CreateFolder("Assets", "Scriptables");
            if (!AssetDatabase.IsValidFolder(k_createPath)) AssetDatabase.CreateFolder("Assets/Scriptables", "Dialogue Graphs");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, path, icon, null);
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
                blackboardBank.AvoidCloning = true;
                blackboardBank.ShowOnList = false;

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