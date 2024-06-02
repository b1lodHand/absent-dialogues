using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Holds some extra data which you can use on the flow.
    /// </summary>
    [System.Serializable]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.AdditionalSpeechData.html")]
    public class AdditionalSpeechData
    {
        [SerializeField] private AudioClip m_audioClip;

        [SerializeField, Tooltip("A string that you can use with the Animator. Parsing it to hash is the recommended way.")] 
        private string m_animMemberName;

        [SerializeField] private Sprite m_sprite;


        [SerializeField, Tooltip("An array of strings that you can use for transmitting extra data. You can use your own conventions with those data.")] 
        private string[] m_customInfo;

        public AudioClip AudioClip => m_audioClip;
        public string AnimatorMemberName => m_animMemberName;
        public Sprite Sprite => m_sprite;
        public string[] CustomInfo => m_customInfo;
    }

}