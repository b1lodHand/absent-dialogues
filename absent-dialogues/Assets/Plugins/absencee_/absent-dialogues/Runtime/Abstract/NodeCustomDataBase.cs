using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Holds some extra data which you can use on the flow.
    /// </summary>
    public abstract class NodeCustomDataBase : ScriptableObject
    {
        [Tooltip("An array of strings that you can use for transmitting extra data. You can use your own conventions with those data.")]
        public string[] CustomInfo;
    }
}
