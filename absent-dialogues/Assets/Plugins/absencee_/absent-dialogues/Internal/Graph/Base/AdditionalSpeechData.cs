using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class AdditionalSpeechData
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private string m_animMemberName;
        [SerializeField] private Sprite m_sprite;
        [SerializeField] private string[] m_customInfo;

        public AudioClip AudioClip => m_audioClip;
        public string AnimatorMemberName => m_animMemberName;
        public Sprite Sprite => m_sprite;
        public string[] CustomInfo => m_customInfo;
    }

}