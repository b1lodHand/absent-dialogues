using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Person))]
public class PersonCustomInspector : Editor
{
    private static readonly string StyleSheetPath = "Assets/Editor/PersonEditor/PersonCustomInspector.uss";

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetPath);
        root.styleSheets.Add(styleSheet);

        DrawGUI(root);

        return root;
    }

    private void DrawGUI(VisualElement root)
    {
        Image iconPreview = new Image();
        iconPreview.AddToClassList("iconPreview");

        ObjectField iconField = new ObjectField("Icon");
        iconField.allowSceneObjects = false;
        iconField.objectType = typeof(Sprite);
        iconField.bindingPath = "Icon";
        iconField.Bind(serializedObject);
        iconField.AddToClassList("field");
        iconField.AddToClassList("iconField");
        iconField.labelElement.name = "label";

        TextField nameField = new TextField("Name");
        nameField.multiline = false;
        nameField.bindingPath = "Name";
        nameField.Bind(serializedObject);
        nameField.AddToClassList("field");
        nameField.labelElement.name = "label";

        TextField descriptionField = new TextField("Description");
        descriptionField.multiline = true;
        descriptionField.bindingPath = "Description";
        descriptionField.Bind(serializedObject);
        descriptionField.AddToClassList("field");
        descriptionField.AddToClassList("descriptionField");
        descriptionField.labelElement.name = "label";

        iconField.RegisterValueChangedCallback(evt =>
        {
            iconPreview.sprite = (Sprite)evt.newValue;
        });

        root.Add(iconPreview);
        root.Add(iconField);
        root.Add(nameField);
        root.Add(descriptionField);
    }
}
