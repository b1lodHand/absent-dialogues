using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using com.absence.utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A singleton with the duty of displaying the current dialogue context. Written for the Unity UI package. Not compatible with
    /// the UI Toolkit.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Displayer")]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueDisplayer.html")]
    public class DialogueDisplayer : Singleton<DialogueDisplayer>
    {
        [Header("Speaker Fields")]

        [SerializeField, Tooltip("The image used to display look of the speaker. This field is optional.")] private Image m_speakerIcon;
        [SerializeField, Tooltip("The text used to display name of the speaker. This field is optional.")] private TMP_Text m_speakerNameText;
        [SerializeField, Required, Tooltip("The text used to display the speech.")] private TMP_Text m_speechText;

        [Space(10)]

        [Header("Utility Fields")]

        [SerializeField, Required, Tooltip("The panel that gets activated/deactivated with the dialogue state.")] private GameObject m_panel;

        [Space(10)]

        [Header("Option Fields")]

        [SerializeField, Required, Tooltip("The container for option boxes.")] private Transform m_optionContainer;
        [SerializeField, Required, Tooltip("The prefab of the option box.")] private DialogueOptionText m_optionPrefab;

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
        public void Display(Person speaker, string speech, List<Option> options, Action<int> optionPressAction)
        {
            ClearOptionContainer();

            Display(speaker, speech);
            for (int i = 0; i < options.Count; i++)
            {
                Option option = options[i];

                if (!option.IsVisible()) continue;

                DialogueOptionText optionText = Instantiate(m_optionPrefab, m_optionContainer);
                optionText.Initialize(i, option.Speech);
                optionText.OnClickAction += optionPressAction;
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