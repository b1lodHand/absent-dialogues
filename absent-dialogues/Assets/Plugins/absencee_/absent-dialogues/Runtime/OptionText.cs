using System;
using TMPro;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component that manages the functionality of an option's drawing and input.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/Option Text")]
    public class OptionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;
        int m_index;

        public Action<int> OnClickAction;

        public void Initialize(int optionIndex, string text)
        {
            m_index = optionIndex;
            m_text.text = text;
        }

        public void OnClick()
        {
            OnClickAction?.Invoke(m_index);
        }
    }

}