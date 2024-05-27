using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// This is a class for holding any variables in the dialogues. It also contains a <see cref="VariableBank"/>.
    /// </summary>
    [System.Serializable]
    public class Blackboard
    {
        /// <summary>
        /// Bank of this blackboard.
        /// </summary>
        [HideInInspector] public VariableBank Bank;

        /// <summary>
        /// Use to clone a blackboard.
        /// <br></br>
        /// <br></br>
        /// <b>CAUTION.</b> This function <b>DOES NOT</b> clone the <see cref="Blackboard.MasterDialogue"/>.
        /// </summary>
        /// <returns></returns>
        public Blackboard Clone()
        {
            Blackboard blackboard = new Blackboard();
            blackboard.Bank = ScriptableObject.Instantiate(Bank);

            return blackboard;
        }
    }
}