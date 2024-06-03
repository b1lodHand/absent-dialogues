using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// This is a class for holding any variables in the dialogues. It also contains a <see cref="VariableBank"/>.
    /// </summary>
    [System.Serializable]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Blackboard.html")]
    public class Blackboard
    {
        /// <summary>
        /// Bank of this blackboard.
        /// </summary>
        [HideInInspector] public VariableBank Bank;

        /// <summary>
        /// Use to clone this blackboard.
        /// </summary>
        /// <returns></returns>
        public Blackboard Clone()
        {
            Blackboard blackboard = new Blackboard();
            blackboard.Bank = Bank.Clone();

            return blackboard;
        }
    }
}