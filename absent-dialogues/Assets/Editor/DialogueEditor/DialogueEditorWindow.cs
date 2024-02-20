using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        static DialogueGraphView m_dialogueGraphView;
        static InspectorView m_inspectorView;
        static IMGUIContainer m_blackboardView;
        static Toolbar m_toolbar;
        static ToolbarMenu m_dialogPartFinder;

        static SerializedObject m_dialogueObject;
        static SerializedProperty m_blackboardProperty;

        static Dialogue m_targetDialogue;

        [MenuItem("Game/Dialogue Editor")]
        public static void OpenWindow()
        {
            DialogueEditorWindow wnd = GetWindow<DialogueEditorWindow>();
            wnd.titleContent = new GUIContent()
            {
                image = EditorGUIUtility.IconContent("d_BlendTree Icon").image,
                text = "Dialogue Editor"
            };
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is not Dialogue) return false;
            OpenWindow();

            m_targetDialogue = Selection.activeObject as Dialogue;

            var dialogObjectField = m_toolbar.Q<ObjectField>();
            dialogObjectField.SetValueWithoutNotify(m_targetDialogue);

            return PopulateDialogView(m_targetDialogue);
        }

        private static bool PopulateDialogView(Dialogue dialogue)
        {
            if (dialogue == null) return false;

            if (Application.isPlaying) m_dialogueGraphView.PopulateView(dialogue);
            else if (AssetDatabase.CanOpenAssetInEditor(dialogue.GetInstanceID())) m_dialogueGraphView.PopulateView(dialogue);

            m_dialogueObject = new SerializedObject(dialogue);
            m_blackboardProperty = m_dialogueObject.FindProperty("Blackboard");

            return true;
        }

        public void CreateGUI()
        {
            // Find the root.
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            m_VisualTreeAsset.CloneTree(root);

            AddStyleSheets(root);
            SetupViews(root);
            SetupToolbar(root);

            SetupEvents();
        }
        private void AddStyleSheets(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogueEditor/DialogueEditorWindow.uss");
            root.styleSheets.Add(styleSheet);
        }
        private void SetupViews(VisualElement root)
        {
            /* FIND VIEWS*/
            m_dialogueGraphView = root.Q<DialogueGraphView>();
            m_inspectorView = root.Q<InspectorView>();
            m_blackboardView = root.Q<IMGUIContainer>();
            m_toolbar = root.Q<Toolbar>();

            /* SETUP VIEWS*/
            m_blackboardView.onGUIHandler += () =>
            {
                if (m_dialogueObject == null) return;

                m_dialogueObject.Update();
                EditorGUILayout.PropertyField(m_blackboardProperty);
                m_dialogueObject.ApplyModifiedProperties();
            };

            m_dialogueGraphView.OnNodeSelected -= OnNodeSelectionChanged;
            m_dialogueGraphView.OnNodeSelected += OnNodeSelectionChanged;
            PopulateDialogView(m_targetDialogue);
        }
        private void SetupToolbar(VisualElement root)
        {
            Create_DialogPartFinder();
            Create_FindRootButton();
            Create_DialogObjectField();
            return;

            void Create_DialogPartFinder()
            {
                m_dialogPartFinder = new ToolbarMenu();
                m_dialogPartFinder.text = "Quick Find";

                RefreshDialogPartFinder();

                m_toolbar.Add(m_dialogPartFinder);
            }
            void Create_FindRootButton()
            {
                var findRootButton = new ToolbarButton(() =>
                {
                    if (m_targetDialogue == null) return;

                    FrameToNode(m_targetDialogue.RootNode);
                });

                findRootButton.text = "Quick Find Root";
                m_toolbar.Add(findRootButton);
            }
            void Create_DialogObjectField()
            {
                var dialogObjectField = new ObjectField("");
                dialogObjectField.label = "Current Dialogue: ";

                dialogObjectField.objectType = typeof(Dialogue);
                dialogObjectField.RegisterValueChangedCallback(p =>
                {
                    if (!p.newValue.Equals(p.previousValue))
                    {
                        m_targetDialogue = (Dialogue)p.newValue;
                        PopulateDialogView(m_targetDialogue);
                    }
                });

                m_toolbar.Add(dialogObjectField);
            }
        }
        private void SetupEvents()
        {
            m_dialogueGraphView.OnPopulateView -= RefreshDialogPartFinder;
            m_dialogueGraphView.OnPopulateView += RefreshDialogPartFinder;
        }

        private void RefreshDialogPartFinder()
        {
            m_dialogPartFinder.menu.ClearItems();
            if (m_targetDialogue == null) return;

            m_targetDialogue.GetAllDialogParts().ForEach(dialogPartNode =>
            {
                m_dialogPartFinder.menu.AppendAction(dialogPartNode.DialogPartName, action =>
                {
                    FrameToNode(dialogPartNode);
                });
            });
        }

        private void FrameToNode(Node node)
        {
            ISelectable selectableNode = m_dialogueGraphView.FindNodeView(node).GetFirstOfType<ISelectable>();

            if (selectableNode == null) return;

            m_dialogueGraphView.ClearSelection();
            m_dialogueGraphView.AddToSelection(selectableNode);
            m_dialogueGraphView.FrameSelection();
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            m_inspectorView.UpdateSelection(node);
        }
    }

}