using com.absence.utilities;
using System;
using TMPro;
using UnityEngine;

namespace com.absence.dialogsystem.runtime
{
    public class DialogueDisplayer : Singleton<DialogueDisplayer>
    {
        [SerializeField] TMP_Text m_speechText;
        [SerializeField] OptionText m_optionTextPrefab;
        [SerializeField] Transform m_optionBank;
        [SerializeField] GameObject m_panel;
        [SerializeField] private bool m_inDialog = false;

        OptionText[] m_optionTexts;
        DialogueInstance m_currentInstance = null;

        public event Action<int> OnOptionPicked;

        public void WriteDecisive(string speech, string[] options)
        {
            ClearDisplay();
            m_optionTexts = new OptionText[options.Length];

            m_speechText.text = speech;
            for (int i = 0; i < options.Length; i++)
            {
                var createdOption = Instantiate(m_optionTextPrefab, m_optionBank);
                m_optionTexts[i] = createdOption;
                createdOption.Initialize(i, options[i]);
                createdOption.OnClickAction = OnOptionPicked;
            }
        }
        public void WriteFast(string speech)
        {
            ClearDisplay();
            m_speechText.text = speech;
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