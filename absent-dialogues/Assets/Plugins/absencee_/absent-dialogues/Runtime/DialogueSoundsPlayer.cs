using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component which is responsible for playing the sounds (if there is any) of the <see cref="DialogueInstance"/>
    /// attached to the same gameobject.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Sounds Player")]
    public class DialogueSoundsPlayer : DialogueExtensionBase
    {
        [SerializeField, Required] private AudioSource m_source;
        [SerializeField, HideIf(nameof(m_source), null), Range(0f, 10f)] private float m_volume = 1f;

        protected override void OnHandleAdditionalData(AdditionalSpeechData data)
        {
            if (data.AudioClip == null) return;
            if (m_source == null) return;

            m_source.PlayOneShot(data.AudioClip, m_volume);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Sound Player")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueSoundsPlayer>();
        }
#endif
    }

}