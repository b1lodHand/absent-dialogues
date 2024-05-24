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

        private DialoguePlayer m_player;
        public DialoguePlayer Player => m_player;

        public event Action<AdditionalSpeechData> OnHandleAdditionalData;

        bool m_inDialogue = false;

        private void Awake()
        {
            if(m_referencedDialogue != null) m_player = new DialoguePlayer(m_referencedDialogue);
        }

        private void Start()
        {
            if (m_startOnAwake) EnterDialogue();
        }

        private void Update()
        {
            if (!m_inDialogue) return;
        }

        public bool EnterDialogue()
        {
            m_inDialogue = false;
            if (!DialogueDisplayer.Instance.Occupy()) return false;

            if (m_overridePeople.Count > 0) m_player.OverridePeople(m_overridePeople);
            m_inDialogue = true;

            m_player.OnContinue += OnPlayerContinue;
            OnPlayerContinue(m_player.State);

            return true;
        }

        public void ExitDialogue()
        {
            if (!m_inDialogue) return;

            m_inDialogue = false;
            m_player.RevertPeople();

            DialogueDisplayer.Instance.Release();

            m_player.OnContinue -= OnPlayerContinue;
        }

        private void OnPlayerContinue(DialoguePlayer.DialoguePlayerState state)
        {
            HandleAdditionalData();

            switch (state)
            {
                case DialoguePlayer.DialoguePlayerState.Idle:
                    Player.Continue();
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForOption:
                    DialogueDisplayer.Instance.Display(Player.Speaker, Player.Speech, Player.Options, i => Player.Continue(i));
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForSkip:
                    DialogueDisplayer.Instance.Display(Player.Speaker, Player.Speech);
                    break;

                case DialoguePlayer.DialoguePlayerState.WillExit:
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

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.RevertPeople();

            m_player.OnContinue -= OnPlayerContinue;
        }

        /// <summary>
        /// Adds a <see cref="DialogueExtensionBase"/> to the target dialogue instance. Does not work runtime.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddExtension<T>() where T : DialogueExtensionBase
        {
            if (Application.isPlaying) return;

            gameObject.AddComponent<T>();
        }
    }
}