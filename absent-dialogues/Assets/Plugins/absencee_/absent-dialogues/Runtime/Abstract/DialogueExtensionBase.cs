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
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueExtensionBase.html")]
    public abstract class DialogueExtensionBase : MonoBehaviour
    {
        [SerializeField, Readonly, Tooltip("Dialogue instance component attached to the current gameobject.")] 
        protected DialogueInstance m_instance;

        [SerializeField, Tooltip("Execution order of this extension. Lower values means sooner execution.")] 
        private sbyte m_order = 0;

        public sbyte Order => m_order;

        /// <summary>
        /// Use to define what to do with the current <see cref="ExtraDialogueData"/>. Gets called when the <see cref="m_instance"/>
        /// progresses.
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnHandleCustomData(NodeCustomDataBase data)
        {

        }
        
        public virtual void OnHandleOptionData(NodeCustomDataBase data)
        {

        }

        /// <summary>
        /// Use to define what to do with the original speech data right before displaying it.
        /// </summary>
        /// <param name="frame">.</param>
        /// <param name="context">.</param>
        public virtual void OnProgress(Node frame, DialogueFlowContext context)
        {
            
        }

        /// <summary>
        /// Use to define what to do right after the target instance clones it's <see cref="DialogueInstance.ReferencedDialogue"/>.
        /// </summary>
        public virtual void OnInitialize()
        {

        }

        /// <summary>
        /// Use to define what to do on each frame when the target instance is <see cref="DialogueInstance.InDialogue"/>
        /// </summary>
        public virtual void OnInstanceUpdate()
        {

        }

        public virtual void OnInstanceValidate()
        {

        }

        public void FindInstance()
        {
            m_instance = GetComponent<DialogueInstance>();
        }

        private void Start()
        {
            if(m_instance == null)
            {
                Debug.LogWarning("There are no instances for this extension to use. Disabling it.");
                enabled = false;
            }
        }

        private void Reset()
        {
            FindInstance();
        }

        [ContextMenu("Find Instance")]
        void FindInstance_Editor()
        {
            FindInstance();
        }
    }
}