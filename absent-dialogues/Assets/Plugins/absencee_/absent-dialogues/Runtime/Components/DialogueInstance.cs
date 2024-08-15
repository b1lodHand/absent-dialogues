using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [SerializeField, Tooltip("When enabled, the referenced dialogue will start automatically when the game starts playing.")] 
        private bool m_startOnAwake = false;

        [SerializeField, Tooltip("When enabled, last node stepped on the flow will get saved. For the next time you enter a dialogue with this instance.")]
        private bool m_saveProgressOnExit = false;

        [Space(10)]

        [SerializeField, Required] private Dialogue m_referencedDialogue;

        [SerializeField, Tooltip("A new list of people to override the default one which is in the dialogue itself. Keeping list size the same with the original one is highly recommended. \nLeave empty if you won't use it.")] 
        private List<Person> m_overridePeople;

        [Space(10)]

        [SerializeField, Readonly, Tooltip("A list which contains all of the extension scripts of this dialogue instance.")] 
        private List<DialogueExtensionBase> m_extensionList = new();

        [SerializeField, Readonly, Runtime] private DialoguePlayer m_player;

        public Dialogue ReferencedDialogue => m_referencedDialogue;
        public Dialogue ClonedDialogue
        {
            get
            {
                if (Player == null) throw new Exception("You cannot use 'DialogueInstance.ClonedDialogue' before that instance clones it's dialogue!");

                return Player.ClonedDialogue;
            }

            private set
            {
                ClonedDialogue = value;
            }
        }

        /// <summary>
        /// <see cref="DialoguePlayer"/> of this instance.
        /// </summary>
        public DialoguePlayer Player => m_player;

        /// <summary>
        /// The Action which will get invoked when <see cref="HandleAdditionalData"/> gets called.
        /// </summary>
        public event Action<ExtraDialogueData> OnHandleExtraData;

        /// <summary>
        /// Action which will get invoked right after this instance clons it's <see cref="ReferencedDialogue"/>.
        /// </summary>
        public event Action OnAfterCloning;

        /// <summary>
        /// Subscribe to this delegate to override any data will get displayed.
        /// </summary>
        public event Action<DialogueFlowContext> OnBeforeProgress;

        /// <summary>
        /// Action which will get invoked when this instance exits dialogue.
        /// </summary>
        public event Action OnExitDialogue;

        bool m_inDialogue = false;

        /// <summary>
        /// Use to check if this instance is in progress right now.
        /// </summary>
        public bool InDialogue => m_inDialogue;

        Person m_speaker;
        string m_text;
        ExtraDialogueData m_extraData;
        List<OptionHandle> m_options;

        [Button("Refresh Extension List")]
        void RefreshExtensionList()
        {
            m_extensionList = gameObject.GetComponents<DialogueExtensionBase>().OrderBy(extension => extension.Order).ToList();
            m_extensionList.ForEach(extension => extension.FindInstance());
        }

        private void Awake()
        {
            if (m_referencedDialogue == null)
            {
                Debug.LogWarning("DialogueInstance has no dialogue references. Disabling it.");
                enabled = false;
                return;
            }

            if(m_overridePeople.Count > 0) m_player = new DialoguePlayer(m_referencedDialogue, m_overridePeople);
            else m_player = new DialoguePlayer(m_referencedDialogue);

            m_extensionList.ForEach(extension => 
            { 
                if (!extension.enabled) return;

                extension.OnAfterCloning();
            });
            
            OnAfterCloning?.Invoke();
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
                if (!extension.enabled) return;

                extension.OnDialogueUpdate();
            });
        }

        /// <summary>
        /// Use to enter dialogue.
        /// </summary>
        /// <returns><b>False</b> if the <see cref="DialogueDisplayer"/> is already occupied by any other script. Returns <b>true</b> otherwise.</returns>
        public bool EnterDialogue()
        {
            m_inDialogue = false;
            if (!DialogueDisplayer.Instance.Occupy()) return false;

            m_inDialogue = true;

            m_player.OnContinue += OnPlayerContinue;

            if(!m_saveProgressOnExit)
            {
                m_player.TeleportToRoot();
                m_player.Continue();
            }

            else
            {
                OnPlayerContinue(m_player.State);
            }

            return true;
        }

        /// <summary>
        /// Use to exit current dialogue.
        /// </summary>
        public void ExitDialogue()
        {
            if (!m_inDialogue) return;

            m_inDialogue = false;

            DialogueDisplayer.Instance.Release();

            m_player.OnContinue -= OnPlayerContinue;

            OnExitDialogue?.Invoke();
        }

        public void ForceContinue()
        {
            m_player.Continue();
        }

        private void OnPlayerContinue(DialoguePlayer.PlayerState state)
        {
            GatherPlayerData();
            HandleAdditionalData();
            InvokeBeforeSpeech();

            switch (state)
            {
                case DialoguePlayer.PlayerState.NoText:
                    Player.Continue();
                    break;

                case DialoguePlayer.PlayerState.WaitingForOption:
                    DialogueDisplayer.Instance.Display(m_speaker, m_text, m_options, i =>
                    {
                        Player.Context.OptionIndex = i;
                        Player.Continue();
                    });
                    break;

                case DialoguePlayer.PlayerState.WaitingForInput:
                    DialogueDisplayer.Instance.Display(m_speaker, m_text);
                    break;

                case DialoguePlayer.PlayerState.WillExit:
                    ExitDialogue();
                    break;

                default:
                    ExitDialogue();
                    throw new Exception("An unknown error occurred while displaying the dialogue.");
            }
        }

        private void GatherPlayerData()
        {
            if(!Player.HasText)
            {
                m_speaker = null;
                m_text = null;
                m_options = null;
                m_extraData = null;
                return;
            }

            m_speaker = Player.Speaker;
            m_text = Player.Text;
            m_extraData = Player.ExtraDialogueData;
            if (Player.HasOptions) m_options = new(Player.OptionIndexPairs);
        }
        private void HandleAdditionalData()
        {
            if (!Player.HasText) return;

            m_extensionList.ForEach(extension =>
            {
                if (!extension.enabled) return;

                extension.OnHandleExtraData(m_extraData);
            });

            OnHandleExtraData?.Invoke(m_extraData);
        }
        private void InvokeBeforeSpeech()
        {
            if (!Player.HasText) return;

            m_extensionList.ForEach(extension =>
            {
                if (!extension.enabled) return;

                extension.OnBeforeSpeech(m_player.Context);
            });

            OnBeforeProgress?.Invoke(m_player.Context);
        }

        /// <summary>
        /// Adds a <see cref="DialogueExtensionBase"/> to the target dialogue instance. <b>Does not work runtime.</b>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddExtension<T>() where T : DialogueExtensionBase
        {
            if (Application.isPlaying) return;

            T component = gameObject.AddComponent<T>();
            m_extensionList.Add(component);
        }

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.OnContinue -= OnPlayerContinue;

            m_player = null;
        }

    }
}