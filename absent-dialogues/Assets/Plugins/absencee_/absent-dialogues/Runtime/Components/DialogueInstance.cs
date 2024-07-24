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
        public event Action<AdditionalSpeechData> OnHandleAdditionalData;

        /// <summary>
        /// Action which will get invoked right after this instance clons it's <see cref="ReferencedDialogue"/>.
        /// </summary>
        public event Action OnAfterCloning;

        /// <summary>
        /// Subscribe to this delegate to override any data will get displayed.
        /// </summary>
        public event SpeechEventHandler OnBeforeSpeech;

        /// <summary>
        /// Action which will get invoked when this instance exits dialogue.
        /// </summary>
        public event Action OnExitDialogue;

        /// <summary>
        /// The delegate responsible for handling events directly about speech.
        /// </summary>
        /// <param name="speaker">Speaker of this speech.</param>
        /// <param name="speech">Speech in context.</param>
        /// <param name="options">Options of this speech (null if there is no options).</param>
        public delegate void SpeechEventHandler(ref Person speaker, ref string speech, ref List<Option> options);

        bool m_inDialogue = false;

        /// <summary>
        /// Use to check if this instance is in progress right now.
        /// </summary>
        public bool InDialogue => m_inDialogue;

        Person m_speaker;
        string m_speech;
        AdditionalSpeechData m_additionalData;
        List<Option> m_options;

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

        private void OnPlayerContinue(DialoguePlayer.PlayerState state)
        {
            GatherPlayerData();
            HandleAdditionalData();
            InvokeBeforeSpeech();

            switch (state)
            {
                case DialoguePlayer.PlayerState.NoSpeech:
                    Player.Continue();
                    break;

                case DialoguePlayer.PlayerState.WaitingForOption:
                    DialogueDisplayer.Instance.Display(m_speaker, m_speech, m_options, i => Player.Continue(i));
                    break;

                case DialoguePlayer.PlayerState.WaitingForSkip:
                    DialogueDisplayer.Instance.Display(m_speaker, m_speech);
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
            if(!Player.HasSpeech)
            {
                m_speaker = null;
                m_speech = null;
                m_options = null;
                m_additionalData = null;
                return;
            }

            m_speaker = Player.Speaker;
            m_speech = Player.Speech;
            m_additionalData = Player.AdditionalSpeechData;
            if (Player.HasOptions) m_options = new(Player.Options);
        }
        private void HandleAdditionalData()
        {
            if (!Player.HasSpeech) return;

            m_extensionList.ForEach(extension =>
            {
                if (!extension.enabled) return;

                extension.OnHandleAdditionalData(m_additionalData);
            });

            OnHandleAdditionalData?.Invoke(m_additionalData);
        }
        private void InvokeBeforeSpeech()
        {
            if (!Player.HasSpeech) return;

            m_extensionList.ForEach(extension =>
            {
                if (!extension.enabled) return;

                extension.OnBeforeSpeech(ref m_speaker, ref m_speech, ref m_options);
            });

            OnBeforeSpeech?.Invoke(ref m_speaker, ref m_speech, ref m_options);
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