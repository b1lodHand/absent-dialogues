using com.absence.attributes;
using com.absence.dialoguesystem;
using com.absence.variablesystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueInstance))]
public class Demo_GUI : MonoBehaviour
{
    const string K_MISSIONPENDING = "b_missionPending";
    const string K_MISSIONDONE = "b_missionDone";
    const string K_MISSIONCOMMITTED = "b_missionCommitted";

    [SerializeField, Readonly] private DialogueInstance m_instance;

    private Variable_Boolean b_missionPending;
    private Variable_Boolean b_missionDone;
    private Variable_Boolean b_missionCommitted;

    private void Start()
    {
        b_missionPending = m_instance.Player.Dialogue.Blackboard.Bank.GetBoolean(K_MISSIONPENDING);
        b_missionDone = m_instance.Player.Dialogue.Blackboard.Bank.GetBoolean(K_MISSIONDONE);
        b_missionCommitted = m_instance.Player.Dialogue.Blackboard.Bank.GetBoolean(K_MISSIONCOMMITTED);
    }

    private void OnGUI()
    {
        if (!b_missionPending.Value) return;
        if (b_missionDone.Value) return;
        if (b_missionCommitted.Value) return;

        if(GUI.Button(new Rect(10f, 10f, 100f, 20f), "Get apples."))
        {
            b_missionDone.Value = true;
        }
    }

    private void Reset()
    {
        m_instance = GetComponent<DialogueInstance>();
    }
}
