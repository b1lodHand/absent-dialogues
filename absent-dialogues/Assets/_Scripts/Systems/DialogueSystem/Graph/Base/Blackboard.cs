using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    [System.Serializable]
    public class Blackboard
    {
        [HideInInspector] public VariableBank Bank;
        [HideInInspector] public Dialogue MasterDialogue;
    }
}