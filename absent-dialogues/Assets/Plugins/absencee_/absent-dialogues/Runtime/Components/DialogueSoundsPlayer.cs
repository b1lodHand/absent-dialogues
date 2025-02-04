using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// A small component which is responsible for playing the sounds (if there is any) of the <see cref="DialogueInstance"/>
    /// attached to the same gameobject.
    /// </summary>
    [RequireComponent(typeof(DialogueInstance))]
    [AddComponentMenu("absencee_/absent-dialogues/Dialogue Instance Extensions/Dialogue Sounds Player")]
    [DisallowMultipleComponent]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueSoundsPlayer.html")]
    public class DialogueSoundsPlayer : DialogueExtensionBase
    {
        [SerializeField, Required] private AudioSource m_source;
        [SerializeField, HideIf(nameof(m_source), null), Range(0f, 1f)] private float m_volume = 1f;

        Coroutine m_playingCoroutine;
        AudioClip m_clip;

        public override void OnAfterCloning()
        {
            m_playingCoroutine = null;
            if (m_source != null) m_source.loop = false;
        }

        public override void OnHandleCustomData(NodeCustomDataBase data)
        {
            //Debug.Log("1");
            ForceStop();
            if(data != null && data is IAudioData audioData)
            {
                //Debug.Log("2");
                m_clip = audioData.AudioClip;
                Play();
            }
        }

        public override void OnProgress(DialogueFlowContext context)
        {
            Debug.Log(context.State);
        }

        IEnumerator C_PlayAudio()
        {
            yield return new WaitWhile(() => m_source.isPlaying);
            ForceStop();
        }

        void Play()
        {
            if (m_clip == null)
            {
                ForceStop();
                return;
            }

            if (m_playingCoroutine != null) StopCoroutine(m_playingCoroutine);

            m_source.clip = m_clip;
            m_source.volume = m_volume;
            m_source.Play();
            m_playingCoroutine = StartCoroutine(C_PlayAudio());

            if (m_playingCoroutine != null)
                ForceStop();
        }

        [Button("Force Stop")]
        void ForceStop()
        {
            if (m_playingCoroutine != null)
            {
                StopCoroutine(m_playingCoroutine);
                m_playingCoroutine = null;
            }

            m_source.Stop();
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