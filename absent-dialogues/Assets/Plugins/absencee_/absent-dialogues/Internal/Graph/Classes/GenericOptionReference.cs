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
            if (target == null) Debug.Log("aegh");
            Target = target;
        }
    }
}
