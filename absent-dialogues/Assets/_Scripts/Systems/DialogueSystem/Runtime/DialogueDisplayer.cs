using com.absence.utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.absence.dialoguesystem
{
    public class DialogueDisplayer : Singleton<DialogueDisplayer>
    {
        [SerializeField] private Image m_speakerIcon;
        [SerializeField] private TMP_Text m_speakerNameText;
        [SerializeField] private TMP_Text m_speechText;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private Transform m_optionContainer;
        [SerializeField] private OptionText m_optionPrefab;
        DialoguePlayer m_player;

        public bool Occupy(DialoguePlayer player)
        {
            if (m_player != null) return false;

            m_player = player;
            m_player.OnContinue += OnPlayerContinue;

            m_panel.SetActive(true);
            return true;
        }

        public void Release()
        {
            if (m_player == null) return;

            m_player.OnContinue -= OnPlayerContinue;
            m_player = null;

            Clear();
        }

        public void Display(Person speaker, string speech)
        {
            ClearOptionContainer();

            if (m_speakerIcon != null) m_speakerIcon.sprite = speaker.Icon;
            if (m_speakerNameText != null) m_speakerNameText.text = speaker.Name;
            m_speechText.text = speech;
        }

        public void Display(Person speaker, string speech, string[] options)
        {
            ClearOptionContainer();

            Display(speaker, speech);
            for (int i = 0; i < options.Length; i++)
            {
                var o = Instantiate(m_optionPrefab, m_optionContainer);
                o.OnClickAction += i =>
                {
                    m_player.Continue(i);
                };
            }
        }

        public void Clear()
        {
            m_panel.SetActive(false);
        }

        public void ClearOptionContainer()
        {
            m_optionContainer.DestroyChildren();
        }

        private void OnPlayerContinue(DialoguePlayer.DialoguePlayerState state)
        {
            switch (state)
            {
                case DialoguePlayer.DialoguePlayerState.Idle:
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForOption:
                    Display(m_player.Speaker, m_player.Speech, m_player.Options);
                    break;

                case DialoguePlayer.DialoguePlayerState.WaitingForSkip:
                    Display(m_player.Speaker, m_player.Speech);
                    break;

                default:
                    Release();
                    break;
            }
        }
    }
}