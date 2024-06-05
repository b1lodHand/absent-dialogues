using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace com.absence.variablebanks.editor
{
    /// <summary>
    /// The static class responsible for handling variable bank creation via editor menu.
    /// </summary>
    public static class VariableBankCreationHandler
    {
        [MenuItem("Assets/absencee_/absent-variabes/Create Variable Bank", priority = 0)]
        static void CreateVariableBank()
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (selectedPath == string.Empty) return;

            while((!AssetDatabase.IsValidFolder(selectedPath)))
            {
                TrimLastSlash(ref selectedPath);
            }

            CreateVariableBankEndNameEditAction create = ScriptableObject.CreateInstance<CreateVariableBankEndNameEditAction>();
            var path = Path.Combine(selectedPath, "New VariableBank.asset");
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

        internal class CreateVariableBankEndNameEditAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var itemCreated = ScriptableObject.CreateInstance<VariableBank>();

                AssetDatabase.CreateAsset(itemCreated, pathName);

                itemCreated.ForExternalUse = false;

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

                VariableBankDatabase.Refresh();
            }
        }
    }

}