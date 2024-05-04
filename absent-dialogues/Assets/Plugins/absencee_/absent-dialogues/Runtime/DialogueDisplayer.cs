using com.absence.personsystem;
using com.absence.utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A singleton with the duty of displaying the current dialogue context.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Displayer")]
    public class DialogueDisplayer : Singleton<DialogueDisplayer>
    {
        [SerializeField] private Image m_speakerIcon;
        [SerializeField] private TMP_Text m_speakerNameText;
        [SerializeField] private TMP_Text m_speechText;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private Transform m_optionContainer;
        [SerializeField] private OptionText m_optionPrefab;

        bool m_occupied = false;

        /// <summary>
        /// Let's you occupy the sinleton. If it is occuppied by any other scripts about dialogues, you can't occupy.
        /// </summary>
        /// <returns>Returns false if the displayer is already occupied. Returns true otherwise.</returns>
        public bool Occupy()
        {
            if (m_occupied) return false;

            EnableView();

            m_occupied = true;
            return true;
        }

        /// <summary>
        /// Removes the occupience of the displayer. CAUTION! <see cref="DialogueDisplayer"/> does not hold a reference to the
        /// current occupier. Because of that, be careful calling this function.
        /// </summary>
        public void Release()
        {
            if (!m_occupied) return;

            ClearOptionContainer();
            DisableView();
        }

        /// <summary>
        /// Displays a speech with no options.
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="speech"></param>
        public void Display(Person speaker, string speech)
        {
            ClearOptionContainer();

            if (m_speakerIcon != null) m_speakerIcon.sprite = speaker.Icon;
            if (m_speakerNameText != null) m_speakerNameText.text = speaker.Name;
            m_speechText.text = speech;
        }

        /// <summary>
        /// Displays a speech with options.
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="speech"></param>
        /// <param name="options"></param>
        /// <param name="optionPressAction"></param>
        public void Display(Person speaker, string speech, string[] options, Action<int> optionPressAction)
        {
            ClearOptionContainer();

            Display(speaker, speech);
            for (int i = 0; i < options.Length; i++)
            {
                var o = Instantiate(m_optionPrefab, m_optionContainer);
                o.OnClickAction += optionPressAction;
            }
        }

        void EnableView()
        {
            m_panel.SetActive(true);
        }

        void DisableView()
        {
            m_panel.SetActive(false);
        }

        void ClearOptionContainer()
        {
            m_optionContainer.DestroyChildren();
        }

    }
}