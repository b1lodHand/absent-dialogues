using com.absence.attributes;
using com.absence.personsystem;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Use to manage a single dialogue player.
    /// </summary>
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private bool m_startOnAwake = false;

        [SerializeField] private Animator m_animator;

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

            switch (m_player.State)
            {
                case DialoguePlayer.DialoguePlayerState.Idle:
                    m_player.Continue();
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForSkip:
                    if (CheckSkipInput()) m_player.Continue();
                    break;

                case DialoguePlayer.DialoguePlayerState.WillExit:
                    ExitDialogue();
                    break;
            }
        }

        private bool CheckSkipInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool EnterDialogue()
        {
            if (!DialogueDisplayer.Instance.Occupy(this)) return false;

            if (m_overridePeople.Count > 0) m_player.OverridePeople(m_overridePeople);
            m_inDialogue = true;

            m_player.OnContinue += OnPlayerContinue;

            return true;
        }

        public void ExitDialogue()
        {
            m_inDialogue = false;
            m_player.RevertPeople();

            DialogueDisplayer.Instance.Release();

            m_player.OnContinue -= OnPlayerContinue;
        }

        private void OnPlayerContinue(DialoguePlayer.DialoguePlayerState state)
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