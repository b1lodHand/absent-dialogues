using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]   
    public class GenericOptionReference
    {
        public bool Bypass = false;
        public GenericOption Target;
        public Node LeadingNode;

        public GenericOptionReference(GenericOption target)
        {
            Target = target;
        }
    }
}
