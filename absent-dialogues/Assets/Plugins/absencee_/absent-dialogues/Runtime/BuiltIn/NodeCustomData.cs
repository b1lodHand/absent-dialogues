using UnityEngine;

namespace com.absence.dialoguesystem.builtin
{
    [CreateAssetMenu(menuName = "Create/absencee_/absent-dialogues/Node Custom Data", fileName = "New Node Custom Data")]
    public class NodeCustomData : NodeCustomDataBase, IAudioData, IAnimatorData, ISpriteData
    {
        [field: SerializeField] 
        public AudioClip AudioClip { get; set; }

        [field: SerializeField]
        public Sprite Sprite { get; set; }

        [field: SerializeField, Tooltip("A string that you can use with the Animator. Parsing it to hash is the recommended way.")]
        public string AnimatorMemberName { get; set; }
    }
}
