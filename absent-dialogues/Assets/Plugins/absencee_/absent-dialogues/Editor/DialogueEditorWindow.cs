using com.absence.dialoguesystem.editor.backup.internals;
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

        [SerializeField] 
        public static DialogueEditorWindow Current { get; private set; }

        internal DialogueGraphView m_dialogueGraphView;
        internal InspectorView m_inspectorView;
        internal BlackboardView m_blackboardView;
        internal Toolbar m_toolbar;
        internal ToolbarMenu m_dialoguePartFinder;
        internal ToolbarButton m_findRootButton;
        internal Button m_infoButton;
        internal Button m_exportButton;
        internal Button m_importButton;
                 
        internal SerializedObject m_dialogueObject;
        internal Dialogue m_targetDialogue;
        internal ObjectField m_dialogueObjectField;

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
                //image = EditorGUIUtility.IconContent("d_Tile Icon").image,
                image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Plugins/absencee_/absent-dialogues/Editor/Resources/editor-window-icon.png"),
                text = "Dialogue Graph"
            };

            Current = wnd;
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
            return Current.OnOpenAsset();
        }

        bool OnOpenAsset()
        {
            m_targetDialogue = Selection.activeObject as Dialogue;

            var dialogObjectField = m_toolbar.Q<ObjectField>();
            dialogObjectField.SetValueWithoutNotify(m_targetDialogue);

            SaveLastDialogue();
            return PopulateDialogueView(m_targetDialogue);
        }

        /// <summary>
        /// Use to save the dialogue displayed currently in the editor.
        /// </summary>
        public void SaveLastDialogue()
        {
            if (m_targetDialogue == null) return;
            if (!AssetDatabase.Contains(m_targetDialogue)) return;

            EditorPrefs.SetString("LastEditedDialogueBeforePlayMode_AssetPath", AssetDatabase.GetAssetPath(m_targetDialogue));
        }

        /// <summary>
        /// Use to load the last dialogue displayed in the editor.
        /// </summary>
        public void LoadLastDialogue()
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

        public void Refresh()
        {
            m_dialogueGraphView.Refresh();
        }

        /// <summary>
        /// Use to display a dialogue in the graph.
        /// </summary>
        /// <param name="dialogue">Target dialogue.</param>
        /// <returns></returns>
        public bool PopulateDialogueView(Dialogue dialogue)
        {
            if (dialogue == null)
            {
                m_targetDialogue = null;
                m_blackboardView.Clear();
                m_inspectorView.Clear();
                m_dialogueGraphView.ClearViewWithoutNotification();
                m_dialogueGraphView.m_dialogue = null;

                RefreshToolbar();
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
            Current = this;

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
            Create_ShowDialogueButton();
            Create_ExportButton();
            Create_ImportButton();
            return;

            void Create_DialoguePartFinder()
            {
                m_dialoguePartFinder = new ToolbarMenu();
                m_dialoguePartFinder.text = "Find Section";

                RefreshDialoguePartFinder();

                m_toolbar.Add(m_dialoguePartFinder);
            }

            void Create_FindRootButton()
            {
                var findRootButton = new ToolbarButton(() =>
                {
                    if (m_targetDialogue == null) return;

                    FrameToNode(m_targetDialogue.Entry);
                });

                findRootButton.text = "Find Entry";
                findRootButton.style.unityTextAlign = TextAnchor.MiddleLeft;

                m_findRootButton = findRootButton;

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
                        m_targetDialogue = null;
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

                m_dialogueObjectField = dialogObjectField;

                m_toolbar.Add(dialogObjectField);
            }

            void Create_ShowDialogueButton()
            {
                Button pingButton = new Button();
                pingButton.text = "ⓘ";
                pingButton.style.unityFontStyleAndWeight = FontStyle.Bold;
                pingButton.style.width = 22f;
                pingButton.tooltip = "Ping current dialogue.";

                pingButton.clicked += () =>
                {
                    if (m_targetDialogue == null)
                        return;

                    Selection.activeObject = m_targetDialogue;
                    EditorGUIUtility.PingObject(m_targetDialogue);
                };

                m_infoButton = pingButton;

                m_toolbar.Add(pingButton);
            }

            void Create_ImportButton()
            {
                Button importButton = new Button();
                importButton.text = "↧";
                importButton.style.unityFontStyleAndWeight = FontStyle.Bold;
                importButton.style.width = 22f;
                importButton.tooltip = "Import new dialogue.";

                importButton.clicked += () =>
                {
                    EditorJobsHelper.ImportNewDialogue();
                };

                m_importButton = importButton;

                m_toolbar.Add(importButton);
            }

            void Create_ExportButton()
            {
                Button exportButton = new Button();
                exportButton.text = "↥";
                exportButton.style.unityFontStyleAndWeight = FontStyle.Bold;
                exportButton.style.width = 22f;
                exportButton.tooltip = "Export current dialogue.";

                exportButton.clicked += () =>
                {
                    EditorJobsHelper.ExportDialogue(m_targetDialogue);
                };

                m_exportButton = exportButton;

                m_toolbar.Add(exportButton);
            }
        }
        private void SetupEvents()
        {
            m_dialogueGraphView.OnNodeSelected -= OnNodeSelectionChanged;
            m_dialogueGraphView.OnNodeSelected += OnNodeSelectionChanged;

            m_dialogueGraphView.OnPopulateView -= RefreshToolbar;
            m_dialogueGraphView.OnPopulateView += RefreshToolbar;
        }

        internal void RefreshToolbar()
        {
            RefreshDialoguePartFinder();

            bool hasDialogue = m_dialogueGraphView.m_dialogue != null;

            m_dialoguePartFinder.SetEnabled(hasDialogue);
            m_infoButton.SetEnabled(hasDialogue);
            m_exportButton.SetEnabled(hasDialogue);
            m_findRootButton.SetEnabled(hasDialogue);
            m_importButton.SetEnabled(true);
        }

        internal void RefreshDialoguePartFinder()
        {
            m_dialoguePartFinder.menu.ClearItems();
            if (m_dialogueGraphView.m_dialogue == null) return;

            m_dialogueGraphView.m_dialogue.GetAllSections().ForEach(dialogPartNode =>
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
        public void FrameToNode(Node node)
        {
            SelectNode(node);
            m_dialogueGraphView.FrameSelection();
        }

        /// <summary>
        /// Selects the target node.
        /// </summary>
        /// <param name="node">Target node.</param>
        public void SelectNode(Node node)
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