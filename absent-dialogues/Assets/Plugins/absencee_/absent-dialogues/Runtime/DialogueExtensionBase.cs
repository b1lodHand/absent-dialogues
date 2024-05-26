using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// This is the base class to derive from in order to handle some custom logic
    /// over the system.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    public abstract class DialogueExtensionBase : MonoBehaviour
    {
        /// <summary>
        /// <see cref="DialogueInstance"/> component attached to the current gameobject.
        /// </summary>
        [SerializeField, Readonly] protected DialogueInstance m_instance;

        private void OnEnable()
        {
            m_instance.OnHandleAdditionalData += OnHandleAdditionalData;
        }

        private void OnDisable()
        {
            m_instance.OnHandleAdditionalData -= OnHandleAdditionalData;
        }

        /// <summary>
        /// Use to define what to do with the current <see cref="AdditionalSpeechData"/>. Gets called when the <see cref="m_instance"/>
        /// progresses.
        /// </summary>
        /// <param name="data"></param>
        public abstract void OnHandleAdditionalData(AdditionalSpeechData data);

        private void Reset()
        {
            m_instance = GetComponent<DialogueInstance>();
        }
    }
}