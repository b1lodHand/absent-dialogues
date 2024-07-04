using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = com.absence.dialoguesystem.internals.Node;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// The dialogue editor window responsible for letting you open, edit and save a dialogue.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.DialogueEditorWindow.html")]
    public sealed class DialogueEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        static DialogueGraphView m_dialogueGraphView;
        internal static InspectorView m_inspectorView;
        static BlackboardView m_blackboardView;
        static Toolbar m_toolbar;
        static ToolbarMenu m_dialoguePartFinder;

        static SerializedObject m_dialogueObject;
        static Dialogue m_targetDialogue;

        /// <summary>
        /// Gets invoked when <see cref="CreateGUI"/> gets called. <b>Clears itself everytime it gets invoked.</b>
        /// </summary>
        public static event Action OnGUIDelayCall;

        /// <summary>
        /// Use to open the dialogue editor window.
        /// </summary>
        [MenuItem("absencee_/absent-dialogues/Open Dialogue Graph Window")]
        public static void OpenWindow()
        {
            DialogueEditorWindow wnd = GetWindow<DialogueEditorWindow>();
            wnd.titleContent = new GUIContent()
            {
                image = EditorGUIUtility.IconContent("d_Tile Icon").image,
                text = "Dialogue Graph"
            };
        }

        /// <summary>
        /// Use to save the dialogue displayed currently in the editor.
        /// </summary>
        public static void SaveLastDialogue()
        {
            if (m_targetDialogue == null) return;
            if (!AssetDatabase.Contains(m_targetDialogue)) return;

            EditorPrefs.SetString("LastEditedDialogueBeforePlayMode_AssetPath", AssetDatabase.GetAssetPath(m_targetDialogue));
        }

        /// <summary>
        /// Use to load the last dialogue displayed in the editor.
        /// </summary>
        public static void LoadLastDialogue()
        {
            string lastDialoguePath = EditorPrefs.GetString("LastEditedDialogueBeforePlayMode_AssetPath", "");
            if (string.IsNullOrWhiteSpace(lastDialoguePath))
            {
                PopulateDialogueView(null);
                return;
            }

            Dialogue lastDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(lastDialoguePath);
            if (lastDialogue == null) 
            {
                PopulateDialogueView(null);
                EditorPrefs.SetString("LastEditedDialogueBeforePlayMode_AssetPath", " ");
                return;
            }

            PopulateDialogueView(lastDialogue);
        }

        /// <summary>
        /// The method that handles the asset selection events.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is not Dialogue) return false;

            OpenWindow();

            m_targetDialogue = Selection.activeObject as Dialogue;

            var dialogObjectField = m_toolbar.Q<ObjectField>();
            dialogObjectField.SetValueWithoutNotify(m_targetDialogue);

            SaveLastDialogue();
            return PopulateDialogueView(m_targetDialogue);
        }

        /// <summary>
        /// Use to display a dialogue in the graph.
        /// </summary>
        /// <param name="dialogue">Target dialogue.</param>
        /// <returns></returns>
        public static bool PopulateDialogueView(Dialogue dialogue)
        {
            if (dialogue == null)
            {
                m_targetDialogue = null;
                m_blackboardView.Clear();
                m_inspectorView.Clear();
                m_dialogueGraphView.ClearViewWithoutNotification();
                m_dialogueGraphView.m_dialogue = null;
                return false;
            }

            m_targetDialogue = dialogue;
            m_dialogueObject = new SerializedObject(m_targetDialogue);

            if (m_dialogueGraphView == null) return false;

            if (Application.isPlaying) m_dialogueGraphView.PopulateView(dialogue);
            else if (AssetDatabase.CanOpenAssetInEditor(dialogue.GetInstanceID())) m_dialogueGraphView.PopulateView(dialogue);

            m_blackboardView.Initialize(m_dialogueObject);

            ObjectField dialogueObjectField = m_toolbar.Q<ObjectField>("dialogue-object-field");
            if (dialogueObjectField == null) return true;

            dialogueObjectField.SetValueWithoutNotify(m_targetDialogue);

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

            PopulateDialogueView(m_targetDialogue);

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            LoadLastDialogue();

            OnGUIDelayCall?.Invoke();
            OnGUIDelayCall = null;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    LoadLastDialogue();
                    break;
            }
        }

        private void AddStyleSheets(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/absencee_/absent-dialogues/Editor/DialogueEditorWindow.uss");
            root.styleSheets.Add(styleSheet);
        }

        private void SetupViews(VisualElement root)
        {
            /* FIND VIEWS*/
            m_dialogueGraphView = root.Q<DialogueGraphView>();
            m_inspectorView = root.Q<InspectorView>();
            m_blackboardView = root.Q<BlackboardView>();
            m_toolbar = root.Q<Toolbar>();
        }

        private void SetupToolbar(VisualElement root)
        {
            Create_DialoguePartFinder();
            Create_FindRootButton();
            Create_DialogueObjectField();
            return;

            void Create_DialoguePartFinder()
            {
                m_dialoguePartFinder = new ToolbarMenu();
                m_dialoguePartFinder.text = "Quick Find";

                RefreshDialoguePartFinder();

                m_toolbar.Add(m_dialoguePartFinder);
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

            void Create_DialogueObjectField()
            {
                ObjectField dialogObjectField = new ObjectField("");
                dialogObjectField.name = "dialogue-object-field";
                dialogObjectField.label = "Current Dialogue: ";

                dialogObjectField.objectType = typeof(Dialogue);
                dialogObjectField.RegisterValueChangedCallback(p =>
                {
                    if (p.newValue == null)
                    {
                        PopulateDialogueView(null);
                        EditorPrefs.SetString("LastEditedDialogueBeforePlayMode_AssetPath", " ");
                        return;
                    }

                    if (!p.newValue.Equals(p.previousValue))
                    {
                        m_targetDialogue = (Dialogue)p.newValue;
                        PopulateDialogueView(m_targetDialogue);
                        SaveLastDialogue();
                    }
                });

                m_toolbar.Add(dialogObjectField);
            }
        }
        private void SetupEvents()
        {
            m_dialogueGraphView.OnNodeSelected -= OnNodeSelectionChanged;
            m_dialogueGraphView.OnNodeSelected += OnNodeSelectionChanged;

            m_dialogueGraphView.OnPopulateView -= RefreshDialoguePartFinder;
            m_dialogueGraphView.OnPopulateView += RefreshDialoguePartFinder;
        }

        internal static void RefreshDialoguePartFinder()
        {
            m_dialoguePartFinder.menu.ClearItems();
            if (m_targetDialogue == null) return;

            m_targetDialogue.GetAllDialogueParts().ForEach(dialogPartNode =>
            {
                m_dialoguePartFinder.menu.AppendAction(dialogPartNode.DialoguePartName, action =>
                {
                    FrameToNode(dialogPartNode);
                });
            });
        }

        /// <summary>
        /// Teleports the view to the target node and selects it.
        /// </summary>
        /// <param name="node">Target node.</param>
        public static void FrameToNode(Node node)
        {
            SelectNode(node);
            m_dialogueGraphView.FrameSelection();
        }

        /// <summary>
        /// Selects the target node.
        /// </summary>
        /// <param name="node">Target node.</param>
        public static void SelectNode(Node node)
        {
            m_dialogueGraphView.SelectNode(node);
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            m_inspectorView.UpdateSelection(node);

            NodeView currentNode = m_inspectorView.m_currentNode;

            if (currentNode != null) EditorPrefs.SetString("last-node-guid", currentNode.Node.Guid);
            else EditorPrefs.SetString("last-node-guid", string.Empty);
        }
    }

}