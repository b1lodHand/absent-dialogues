using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System;
using TMPro;
using UnityEngine;

namespace com.absence.dialoguesystem.examples
{
    /// <summary>
    /// A small component that manages the functionality of an option's drawing and input.
    /// </summary>
    [AddComponentMenu("absencee_/absent-dialogues/UI/Option Text")]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueOptionText.html")]
    public class OptionText : MonoBehaviour
    {
        [SerializeField, Required, Tooltip("The text that will show the option speech.")] 
        private TMP_Text m_text;

        public OptionHandle Handle { get; private set; }

        public event Action<OptionHandle> OnClickAction;
        public event Action OnSelectAction;

        /// <summary>
        /// Sets the index and the text of this option.
        /// </summary>
        /// <param name="optionIndex"></param>
        /// <param name="text"></param>
        public void Initialize(OptionHandle handle)
        {
            Handle = handle;

            m_text.text = Handle.Text;
        }

        /// <summary>
        /// Calls <see cref="OnClickAction"/>.
        /// </summary>
        public void OnClick()
        {
            OnClickAction?.Invoke(Handle);
        }

        public void OnSelect()
        {
            OnSelectAction?.Invoke();
        }
    }

}