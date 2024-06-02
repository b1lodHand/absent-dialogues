using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// This is the base class to derive from in order to handle some custom logic
    /// over the system.
    /// </summary>
    /// <remarks>
    /// Execution order goes like:
    /// <code>
    /// OnHandleAdditionalData(...);
    /// OnBeforeSpeech(...);
    /// </code>
    /// </remarks>
    [RequireComponent(typeof(DialogueInstance))]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueExtensionBase.html")]
    public abstract class DialogueExtensionBase : MonoBehaviour
    {
        /// <summary>
        /// <see cref="DialogueInstance"/> component attached to the current gameobject.
        /// </summary>
        [SerializeField, Readonly] protected DialogueInstance m_instance;

        private void OnEnable()
        {
            m_instance.OnHandleAdditionalData += OnHandleAdditionalData;
            m_instance.OnBeforeSpeech += OnBeforeSpeech;
            m_instance.OnAfterCloning += OnAfterCloning;
        }

        private void OnDisable()
        {
            m_instance.OnHandleAdditionalData -= OnHandleAdditionalData;
            m_instance.OnBeforeSpeech -= OnBeforeSpeech;
            m_instance.OnAfterCloning -= OnAfterCloning;
        }

        /// <summary>
        /// Use to define what to do with the current <see cref="AdditionalSpeechData"/>. Gets called when the <see cref="m_instance"/>
        /// progresses.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnHandleAdditionalData(AdditionalSpeechData data)
        {

        }

        /// <summary>
        /// Use to define what to do with the original speech data right before displaying it.
        /// </summary>
        /// <param name="speaker">Speaker of this speech.</param>
        /// <param name="speech">Speech in context.</param>
        /// <param name="options">Option of this speech.</param>
        protected virtual void OnBeforeSpeech(ref Person speaker, ref string speech, ref List<Option> options)
        {
            
        }

        /// <summary>
        /// Use to define what to do right after the target instance clones it's <see cref="DialogueInstance.ReferencedDialogue"/>.
        /// </summary>
        protected virtual void OnAfterCloning()
        {

        }

        /// <summary>
        /// Use to define what to do on each frame when the target instance is <see cref="DialogueInstance.InDialogue"/>
        /// </summary>
        protected virtual void OnDialogueUpdate()
        {

        }

        private void Start()
        {
            if(m_instance == null)
            {
                Debug.LogWarning("There are no instances for this extension to use. Disabling it.");
                enabled = false;
            }
        }

        private void Update()
        {
            if (!m_instance.InDialogue) return;

            OnDialogueUpdate();
        }

        private void Reset()
        {
            m_instance = GetComponent<DialogueInstance>();
        }
    }
}