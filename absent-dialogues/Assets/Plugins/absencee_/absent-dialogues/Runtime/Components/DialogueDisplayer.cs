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
    [AddComponentMenu("absencee_/absent-dialogues/UI/Dialogue Displayer")]
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
        [SerializeField, Required, Tooltip("The prefab of the option box.")] private OptionText m_optionPrefab;

        /// <summary>
        /// Action which will get invoked when displayer refreshes.
        /// </summary>
        public event Action OnDisplay = null;

        bool m_occupied = false;
        List<GameObject> m_options = new();
        GameObject m_firstOption;
        GameObject m_lastSelectedOption;

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

            m_occupied = false;

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

            OnDisplay?.Invoke();
        }

        /// <summary>
        /// Displays a speech with options.
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="speech"></param>
        /// <param name="options"></param>
        /// <param name="optionPressAction"></param>
        public void Display(Person speaker, string speech, List<OptionHandle> options, Action<int> optionPressAction)
        {
            ClearOptionContainer();

            Display(speaker, speech);

            options.ForEach(option =>
            {
                OptionText optionText = Instantiate(m_optionPrefab, m_optionContainer);
                optionText.Initialize(option.TargetedIndex, option.Text);

                optionText.OnClickAction += optionPressAction;

                optionText.OnSelectAction += () =>
                {
                    m_lastSelectedOption = optionText.gameObject;
                };

                if (m_firstOption != null) return;

                m_firstOption = optionText.gameObject;
                ReselectFirstOptionIfExists();

                OnDisplay?.Invoke();
            });
        }

        /// <summary>
        /// Reselects first option if certain conditions are met.
        /// </summary>
        public void ReselectFirstOptionIfExists()
        {
            if (m_options.Count == 0) return;
            if (!m_occupied) return;
            if (UnityEngine.EventSystems.EventSystem.current == null) return;
            if (m_firstOption == null) return;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_firstOption);
        }

        /// <summary>
        /// Reselects last option selected by the user if certain conditions are met.
        /// </summary>
        public void ReselectLastOptionIfExists()
        {
            if (m_options.Count == 0) return;
            if (!m_occupied) return;
            if (UnityEngine.EventSystems.EventSystem.current == null) return;

            if (m_lastSelectedOption == null)
            {
                ReselectFirstOptionIfExists();
                return;
            }

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_lastSelectedOption);
        }

        /// <summary>
        /// When called, attempts to select the option above the current one.
        /// </summary>
        public void TrySelectUpperOption()
        {
            if (m_options.Count == 0) return;
            if (!m_occupied) return;
            if (UnityEngine.EventSystems.EventSystem.current == null) return;

            if (m_lastSelectedOption == null) return;

            int index = m_options.IndexOf(m_lastSelectedOption);
            if (index == 0) return;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_options[index - 1]);
        }

        /// <summary>
        /// When called, attempts to select the option below the current one.
        /// </summary>
        public void TrySelectLowerOption()
        {
            if (m_options.Count == 0) return;
            if (!m_occupied) return;
            if (UnityEngine.EventSystems.EventSystem.current == null) return;

            if (m_lastSelectedOption == null) return;

            int index = m_options.IndexOf(m_lastSelectedOption);
            if (index == m_options.Count - 1) return;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_options[index + 1]);
        }

        /// <returns>
        /// Index of the currently selected option.
        /// </returns>
        public int TryGetCurrentSelectedOptionIndex()
        {
            if (m_lastSelectedOption == null) return -1;

            return m_options.IndexOf(m_lastSelectedOption);
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
            m_options.Clear();
            m_optionContainer.DestroyChildren();
            m_firstOption = null;
            m_lastSelectedOption = null;
        }

    }
}