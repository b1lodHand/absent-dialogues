using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    [System.Serializable]
    public class Blackboard
    {
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