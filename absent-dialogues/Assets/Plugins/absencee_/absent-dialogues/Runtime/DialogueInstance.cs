using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Lets you manage a single <see cref="DialoguePlayer"/> in the scene easily.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Instance")]
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField, Tooltip("When enabled, the referenced dialogue will start automatically when the game starts playing.")] 
        private bool m_startOnAwake = false;

        [Space(10)]

        [SerializeField, Required] private Dialogue m_referencedDialogue;

        [SerializeField, Tooltip("A new list of people to override the default one which is in the dialogue itself. Keeping list size the same with the original one is highly recommended. Leave empty if you won't use it.")] 
        private List<Person> m_overridePeople;

        [SerializeField, Readonly, Runtime] private DialoguePlayer m_player;

        /// <summary>
        /// <see cref="DialoguePlayer"/> of this instance.
        /// </summary>
        public DialoguePlayer Player => m_player;

        /// <summary>
        /// The Action which will get invoked when <see cref="HandleAdditionalData"/> gets called.
        /// </summary>
        public event Action<AdditionalSpeechData> OnHandleAdditionalData;

        bool m_inDialogue = false;

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
        }
        private void Start()
        {
            if (m_startOnAwake) EnterDialogue();
        }
        private void Update()
        {
            if (!m_inDialogue) return;
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
            OnPlayerContinue(m_player.State);

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
        }

        private void OnPlayerContinue(DialoguePlayer.PlayerState state)
        {
            HandleAdditionalData();

            switch (state)
            {
                case DialoguePlayer.PlayerState.NoSpeech:
                    Player.Continue();
                    break;

                case DialoguePlayer.PlayerState.WaitingForOption:
                    DialogueDisplayer.Instance.Display(Player.Speaker, Player.Speech, Player.Options, i => Player.Continue(i));
                    break;

                case DialoguePlayer.PlayerState.WaitingForSkip:
                    DialogueDisplayer.Instance.Display(Player.Speaker, Player.Speech);
                    break;

                case DialoguePlayer.PlayerState.WillExit:
                    ExitDialogue();
                    break;

                default:
                    ExitDialogue();
                    throw new Exception("An unknown error occurred while displaying the dialogue.");
            }
        }
        private void HandleAdditionalData()
        {
            if (!Player.HasSpeech) return;

            OnHandleAdditionalData?.Invoke(Player.AdditionalSpeechData);
        }

        /// <summary>
        /// Adds a <see cref="DialogueExtensionBase"/> to the target dialogue instance. <b>Does not work runtime.</b>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddExtension<T>() where T : DialogueExtensionBase
        {
            if (Application.isPlaying) return;

            gameObject.AddComponent<T>();
        }

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.OnContinue -= OnPlayerContinue;

            m_player = null;
        }

    }
}