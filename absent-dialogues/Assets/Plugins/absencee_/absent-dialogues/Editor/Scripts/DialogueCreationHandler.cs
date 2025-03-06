using com.absence.dialoguesystem.internals;
using com.absence.variablesystem.banksystembase;
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
        public static Dialogue CreateDialogue(string pathName)
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
            //itemCreated.RootNode = itemCreated.CreateNode(typeof(RootNode)) as RootNode;
            //AssetDatabase.AddObjectToAsset(itemCreated.RootNode, itemCreated);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = itemCreated;

            return itemCreated;
        }

        internal class CreateDialogueEndNameEditAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                DialogueCreationHandler.CreateDialogue(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Dialogue item = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
                ScriptableObject.DestroyImmediate(item);
            }
        }
    }

}