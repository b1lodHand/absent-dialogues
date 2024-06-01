using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component which is responsible for playing the animations (if there is any) of the dialogue instance
    /// attached to the same game object.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Animations Player")]
    public class DialogueAnimationsPlayer : DialogueExtensionBase
    {
        /// <summary>
        /// Lets you select the way this extension uses the <see cref="AdditionalSpeechData.AnimatorMemberName"/>.
        /// </summary>
        private enum WorkMode
        {
            CrossFade = 0,
            SetTrigger = 1,
        }

        [SerializeField, Required] private Animator m_animator;
        [SerializeField, Tooltip("<b>Crossfade:</b> Crossfades to the state with the target name\n\n<b>SetTrigger:</b> Sets the trigger with the target name.")] 
        private WorkMode m_workMode = WorkMode.CrossFade;

        [SerializeField, ShowIf(nameof(m_workMode), WorkMode.CrossFade), Range(0f, 1f)]
        private float m_transitionTime = 0.15f;

        public override void OnHandleAdditionalData(AdditionalSpeechData data)
        {
            if (string.IsNullOrWhiteSpace(data.AnimatorMemberName)) return;

            int hash = Animator.StringToHash(data.AnimatorMemberName);

            switch (m_workMode)
            {
                case WorkMode.CrossFade:
                    m_animator.CrossFade(hash, m_transitionTime);
                    break;

                case WorkMode.SetTrigger:
                    m_animator.SetTrigger(hash);
                    break;

                default:
                    break;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Animation Player")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueAnimationsPlayer>();
        }
#endif
    }
}