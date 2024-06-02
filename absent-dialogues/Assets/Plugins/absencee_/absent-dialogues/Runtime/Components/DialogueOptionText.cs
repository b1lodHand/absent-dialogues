using com.absence.attributes;
using System;
using TMPro;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component that manages the functionality of an option's drawing and input.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Option Text")]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueOptionText.html")]
    public class DialogueOptionText : MonoBehaviour
    {
        [SerializeField, Required, Tooltip("The text that will show the option speech.")] private TMP_Text m_text;
        int m_index;

        public event Action<int> OnClickAction;

        /// <summary>
        /// Sets the index and the text of this option.
        /// </summary>
        /// <param name="optionIndex"></param>
        /// <param name="text"></param>
        public void Initialize(int optionIndex, string text)
        {
            m_index = optionIndex;
            m_text.text = text;
        }

        /// <summary>
        /// Calls <see cref="OnClickAction"/>.
        /// </summary>
        public void OnClick()
        {
            OnClickAction?.Invoke(m_index);
        }
    }

}