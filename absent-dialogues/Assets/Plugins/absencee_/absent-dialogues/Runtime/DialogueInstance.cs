using com.absence.attributes;
using com.absence.personsystem;
using com.absence.variablesystem;
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
        [SerializeField] private bool m_startOnAwake = false;

        [SerializeField] private AudioSource m_audioSource;
        [SerializeField, HideIf(nameof(m_audioSource), null), Range(0f, 1f)] private float m_audioVolume;

        [Space(10)]

        [SerializeField, Required] private Dialogue m_dialogue;
        [SerializeField] private List<Person> m_overridePeople;

        private DialoguePlayer m_player;
        public DialoguePlayer Player => m_player;

        bool m_inDialogue = false;

        private void Start()
        {
            if(m_dialogue != null) m_player = new DialoguePlayer(m_dialogue);
            if (m_startOnAwake) EnterDialogue();
        }

        private void Update()
        {
            if (!m_inDialogue) return;

            if(Player.State == DialoguePlayer.DialoguePlayerState.WaitingForSkip)
                if (CheckSkipInput()) Player.Continue();
        }

        private bool CheckSkipInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
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

        protected virtual void HandleAdditionalData()
        {
            if (m_audioSource == null) return;

            if (m_audioSource.isPlaying) m_audioSource.Stop();
            if (m_player.AdditionalSpeechData.AudioClip != null) m_audioSource.PlayOneShot(m_player.AdditionalSpeechData.AudioClip, m_audioVolume);
        }

        private void OnApplicationQuit()
        {
            m_inDialogue = false;
            m_player.RevertPeople();

            m_player.OnContinue -= OnPlayerContinue;
        }
    }
}