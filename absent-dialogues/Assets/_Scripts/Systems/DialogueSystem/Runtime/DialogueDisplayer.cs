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
        [SerializeField] private Transform m_optionContainer;
        [SerializeField] private OptionText m_optionPrefab;
        DialoguePlayer m_player;

        public bool Occupy(DialoguePlayer player)
        {
            if (m_player != null) return false;

            m_player = player;
            return true;
        }

        public void Release()
        {
            m_player = null;
        }

        public void Display(Person speaker, string speech)
        {
            if (m_speakerIcon != null) m_speakerIcon.sprite = speaker.Icon;
            if (m_speakerNameText != null) m_speakerNameText.text = speaker.Name;
            m_speechText.text = speech;
        }

        public void Display(Person speaker, string speech, string[] options)
        {
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
    }
}