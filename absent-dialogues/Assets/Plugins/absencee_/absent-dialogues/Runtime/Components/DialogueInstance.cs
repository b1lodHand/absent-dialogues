using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static com.absence.dialoguesystem.DialogueFlowContext;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Lets you manage a single <see cref="DialoguePlayer"/> in the scene easily.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Instance")]
    [DisallowMultipleComponent]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInstance.html")]
    public class DialogueInstance : MonoBehaviour, IUseDialogueInScene
    {
        [SerializeField, Tooltip("When enabled, this instance will optimize its dialogue player.")]
        private bool m_optimize = true;

#pragma warning disable CS0414
        [SerializeField] private bool m_cloneDialogueBeforeUsing = true;
#pragma warning restore CS0414

        [SerializeField, Tooltip("When enabled, the referenced dialogue will start automatically when the game starts playing.")] 
        private bool m_startOnAwake = false;

        [Space(10)]

        [SerializeField, Required] private Dialogue m_referencedDialogue;

        [SerializeField, Tooltip("A new list of people to override the default one which is in the dialogue itself. Keeping list size the same with the original one is highly recommended. \nLeave empty if you won't use it.")] 
        private List<PersonOverride> m_overridePeople = new();

        [Space(10)]

        [SerializeField, Readonly, Tooltip("A list which contains all of the extension scripts of this dialogue instance.")] 
        private List<DialogueExtensionBase> m_extensionList = new();

        [SerializeField, Readonly, Runtime] private DialoguePlayer m_player;

        public Dialogue ReferencedDialogue => m_referencedDialogue;
        public Dialogue ClonedDialogue => Player.Target;

        /// <summary>
        /// <see cref="DialoguePlayer"/> of this instance.
        /// </summary>
        public DialoguePlayer Player => m_player;

        /// <summary>
        /// The Action which will get invoked when <see cref="InvokeHandleCustomData"/> gets called.
        /// </summary>
        public event Action<NodeCustomDataBase> OnHandleCustomData;

        public event Action<NodeCustomDataBase> OnHandleOptionData;

        /// <summary>
        /// Action which will get invoked right after this instance clons it's <see cref="ReferencedDialogue"/>.
        /// </summary>
        public event Action OnInitialize;

        public event Action OnReachOneShot;
        public event Action OnPassOneShot;
        public event Action OnProgressOneShot;

        /// <summary>
        /// Subscribe to this delegate to override any data will get displayed.
        /// </summary>
        public event Action<Node, DialogueFlowContext> OnProgress;

        /// <summary>
        /// Action which will get invoked when this instance exits dialogue.
        /// </summary>
        public event Action OnExitDialogue;

        /// <summary>
        /// Use to check if this instance is in progress right now.
        /// </summary>
        public bool InDialogue => m_inDialogue;

        public event Action OnValidation = delegate { };

        bool m_inDialogue = false;
        Dictionary<Person, Person> m_overridePairs;

        private void Awake()
        {
            if (m_referencedDialogue == null)
            {
                Debug.LogWarning("DialogueInstance has no dialogue references. Disabling it.");
                enabled = false;
                return;
            }

            //Dialogue dialogue = m_referencedDialogue.Clone();

            Dialogue dialogue = m_referencedDialogue;

#if UNITY_EDITOR
            dialogue = m_referencedDialogue.Clone();
#else
            if (m_cloneDialogueBeforeUsing)       
                dialogue = m_referencedDialogue.Clone();

            m_overridePairs = new();
            for (int i = 0; i < m_overridePeople.Count; i++) 
            {
                PersonOverride ovr = m_overridePeople[i];
                Person resultOfOverride =
                    ovr.Override != null ? ovr.Override : ovr.Target;

                m_overridePairs.Add(ovr.Target, resultOfOverride);
            }
#endif

            m_player = new DialoguePlayer(dialogue);
            if (m_optimize) m_player.Optimize();

            m_extensionList.ForEach(extension => 
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnInitialize();
            });
            
            OnInitialize?.Invoke();
        }

        private void Start()
        {
            if (m_startOnAwake) EnterDialogue();
        }

        private void Update()
        {
            if (!m_inDialogue) return;

            m_extensionList.ForEach(extension =>
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnInstanceUpdate();
            });
        }

        /// <summary>
        /// Use to enter dialogue.
        /// </summary>
        /// <returns><b>False</b> if the <see cref="DialogueDisplayer"/> is already occupied by any other script. Returns <b>true</b> otherwise.</returns>
        public bool EnterDialogue()
        {
            if (m_inDialogue)
                return true;

            m_inDialogue = false;
            if (!DialogueDisplayer.Instance.Occupy()) return false;

            m_inDialogue = true;

            m_player.OnProgress += OnPlayerContinue;

            m_player.TeleportToRoot(false);
            m_player.Continue();

            return true;
        }

        /// <summary>
        /// Use to exit current dialogue.
        /// </summary>
        public void ExitDialogue()
        {
            if (!m_inDialogue) 
                return;

            m_inDialogue = false;
            m_player.ClearContext();

            DialogueDisplayer.Instance.Release();

            m_player.OnProgress -= OnPlayerContinue;

            OnExitDialogue?.Invoke();
        }

        public void ForceContinue()
        {
            m_player.Continue();
        }

        private void OnPlayerContinue(DialoguePlayer player)
        {
            DialogueFlowContext context = player.Context;

            switch (context.State)
            {
                case ContextState.Reach:
                    OnReach(player);
                    break;
                case ContextState.Pass:
                    OnPass(player);
                    break;
                default:
                    return;
            };
        }

        void OnReach(DialoguePlayer player)
        {
            DialogueFlowContext context = player.Context;
            Node frame = player.Frame;

            InvokeOnProgress();
            InvokeHandleCustomData();

            OnReachOneShot?.Invoke();
            OnReachOneShot = null;

            OnProgressOneShot?.Invoke();
            OnProgressOneShot = null;

            Person overridenPerson = null;
            Person person = frame.GetPerson(m_player.Target);

#if !UNITY_EDITOR
            overridenPerson = m_overridePairs[person];
#else
            PersonOverride overrideFound = m_overridePeople.FirstOrDefault(ovr => (ovr.Override != null) && (ovr.Target.Equals(person)));
            overridenPerson = overrideFound != null ? overrideFound.Override : person;
#endif

            if (!context.HasText)
            {
                ForceContinue();
                return;
            }

            OnDisplayPrompt(overridenPerson, context, frame);
        }

        void OnPass(DialoguePlayer player)
        {
            DialogueFlowContext context = player.Context;
            Node frame = player.Frame;

            InvokeOnProgress();
            InvokeHandleOptionData();

            OnPassOneShot?.Invoke();
            OnPassOneShot = null;

            OnProgressOneShot?.Invoke();
            OnProgressOneShot = null;

            if (context.WillExit)
            {
                ExitDialogue();
                return;
            }

            ForceContinue();
        }

        private void InvokeHandleOptionData()
        {
            NodeCustomDataBase optionData = m_player.Context.OptionData;

            if (optionData == null)
                return;

            m_extensionList.ForEach(extension =>
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnHandleOptionData(optionData);
            });

            OnHandleOptionData?.Invoke(optionData);
        }

        private void InvokeHandleCustomData()
        {
            NodeCustomDataBase customData = m_player.Context.CustomData;

            if (customData == null)
                return;

            m_extensionList.ForEach(extension =>
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnHandleCustomData(customData);
            });

            OnHandleCustomData?.Invoke(customData);
        }
        private void InvokeOnProgress()
        {
            Node frame = m_player.Frame;
            DialogueFlowContext context = m_player.Context;
            m_extensionList.ForEach(extension =>
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnProgress(frame, context);
            });

            OnProgress?.Invoke(frame, context);
        }

        protected virtual void OnDisplayPrompt(Person person, DialogueFlowContext context, Node frame)
        {
            if (context.HasOptions)
            {
                DialogueDisplayer.Instance.Display(person, context.Text, context.OptionHandles, handle =>
                {
                    context.SelectedOption = handle.TargetedIndex;
                    context.OptionData = handle.CustomData;
                    ForceContinue();
                });
            }

            else
            {
                DialogueDisplayer.Instance.Display(person, context.Text);
            }
        }

        /// <summary>
        /// Adds a <see cref="DialogueExtensionBase"/> to the target dialogue instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddExtension<T>() where T : DialogueExtensionBase
        {
            T component = gameObject.AddComponent<T>();
            m_extensionList.Add(component);
        }

        [Button("Refresh Dialogue")]
        void Refresh()
        {
            RefreshPeopleOverrides();
        }

        void RefreshPeopleOverrides()
        {
            if (m_referencedDialogue == null)
            {
                m_overridePeople.Clear();
                return;
            }

            List<PersonOverride> newOverrides = new();
            m_referencedDialogue.People.ForEach(person =>
            {
                PersonOverride overrideFound = m_overridePeople.FirstOrDefault(ovr => (ovr.Target == person) && (ovr.Override != null));
                Person result = overrideFound != null ? overrideFound.Override : null;
                newOverrides.Add(new PersonOverride() { Target = person, Override = result });
            });

            m_overridePeople = newOverrides;

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssetIfDirty(gameObject);
#endif
        }

        [Button("Refresh Extension List")]
        void RefreshExtensionList()
        {
            m_extensionList = gameObject.GetComponents<DialogueExtensionBase>().OrderBy(extension => extension.Order).ToList();
            m_extensionList.ForEach(extension => extension.FindInstance());

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssetIfDirty(gameObject);
#endif
        }

        private void OnValidate()
        {
            RefreshPeopleOverrides();

            OnValidation?.Invoke();

            m_extensionList.ForEach(extension =>
            {
                if (extension == null) return;
                if (!extension.enabled) return;

                extension.OnInstanceValidate();
            });
        }

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.OnProgress -= OnPlayerContinue;

            m_player = null;
        }
    }
}