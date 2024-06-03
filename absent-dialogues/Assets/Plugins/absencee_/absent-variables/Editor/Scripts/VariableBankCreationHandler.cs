using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace com.absence.variablesystem.editor
{
    public class VariableBankCreationHandler
    {
        private static readonly string k_createPath = "Assets/Scriptables/VariableBanks";

        [MenuItem("absencee_/absent-variabes/Create Variable Bank", priority = 0)]
        static void CreateVariableBank()
        {
            CreateVariableBankEndNameEditAction create = ScriptableObject.CreateInstance<CreateVariableBankEndNameEditAction>();
            var path = Path.Combine(k_createPath, "New VariableBank.asset");
            var icon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image as Texture2D;

            if (!AssetDatabase.IsValidFolder("Assets/Scriptables")) AssetDatabase.CreateFolder("Assets", "Scriptables");
            if (!AssetDatabase.IsValidFolder("Assets/Scriptables/VariableBanks")) AssetDatabase.CreateFolder("Assets/Scriptables", "VariableBanks");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, path, icon, null);
        }

        internal class CreateVariableBankEndNameEditAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var itemCreated = ScriptableObject.CreateInstance<VariableBank>();

                AssetDatabase.CreateAsset(itemCreated, pathName);

                itemCreated.AvoidCloning = false;
                itemCreated.ShowOnList = true;

                itemCreated.OnDestroyAction += () =>
                {
                    VariableBankDatabase.Refresh();
                };

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                VariableBankDatabase.Refresh();

                Selection.activeObject = itemCreated;
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                VariableBank item = EditorUtility.InstanceIDToObject(instanceId) as VariableBank;
                ScriptableObject.DestroyImmediate(item);
            }
        }
    }

}