using com.absence.utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.absence.dialoguesystem
{
    public class DialogueDisplayer : Singleton<DialogueDisplayer>
    { 
        [SerializeField] GameObject m_panel;
        [SerializeField] TMP_Text m_speechText;
        [SerializeField] Image m_iconImage;
        [SerializeField] TMP_Text m_personNameText;
        [SerializeField] Transform m_optionBank;
        [SerializeField] OptionText m_optionTextPrefab;

        [SerializeField] private bool m_inDialog = false;

        OptionText[] m_optionTexts;
        DialogueInstance m_currentInstance = null;

        public event Action<int> OnOptionPicked;

        public void WriteDecisive(Person person, string speech, string[] options)
        {
            ClearDisplay();
            m_optionTexts = new OptionText[options.Length];

            if (m_speechText) m_speechText.text = speech;
            if (m_iconImage) m_iconImage.sprite = person.Icon;
            if (m_personNameText) m_personNameText.text = person.Name;

            for (int i = 0; i < options.Length; i++)
            {
                var createdOption = Instantiate(m_optionTextPrefab, m_optionBank);
                m_optionTexts[i] = createdOption;
                createdOption.Initialize(i, options[i]);
                createdOption.OnClickAction = OnOptionPicked;
            }
        }
        public void WriteFast(Person person, string speech)
        {
            ClearDisplay();
            if (m_speechText) m_speechText.text = speech;
            if (m_iconImage) m_iconImage.sprite = person.Icon;
            if (m_personNameText) m_personNameText.text = person.Name;
        }

        public void EnterDialog(DialogueInstance instance)
        {
            if (m_inDialog) return;

            m_inDialog = true;
            m_currentInstance = instance;
            OnOptionPicked += OptionPicked;

            ClearDisplay();
            m_panel.SetActive(true);
        }

        public void ExitDialog()
        {
            if (!m_inDialog) return;

            m_inDialog = false;
            OnOptionPicked -= OptionPicked;
            m_currentInstance = null;

            ClearDisplay();
            m_panel.SetActive(false);
        }

        void ClearDisplay()
        {
            m_speechText.text = string.Empty;
            m_optionBank.DestroyChildren();
        }

        void OptionPicked(int index)
        {
            m_currentInstance.OptionReceived(index);
        }
    }

}