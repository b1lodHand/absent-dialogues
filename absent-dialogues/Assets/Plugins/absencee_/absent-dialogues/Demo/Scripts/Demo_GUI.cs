using com.absence.attributes;
using com.absence.dialoguesystem;
using UnityEngine;

[RequireComponent(typeof(DialogueInstance))]
public class Demo_GUI : MonoBehaviour
{
    const string K_MISSIONPENDING = "b_missionPending";
    const string K_MISSIONDONE = "b_missionDone";
    const string K_MISSIONCOMMITTED = "b_missionCommitted";

    [SerializeField, Readonly] private DialogueInstance m_instance;

    private void OnGUI()
    {
        m_instance.Player.ClonedDialogue.Blackboard.Bank.TryGetBoolean(K_MISSIONPENDING, out bool pending);
        m_instance.Player.ClonedDialogue.Blackboard.Bank.TryGetBoolean(K_MISSIONDONE, out bool done);
        m_instance.Player.ClonedDialogue.Blackboard.Bank.TryGetBoolean(K_MISSIONCOMMITTED, out bool committed);

        if (!pending) return;
        if (done) return;
        if (committed) return;

        if(GUI.Button(new Rect(10f, 10f, 100f, 20f), "Get apples."))
        {
            m_instance.Player.ClonedDialogue.Blackboard.Bank.SetBoolean(K_MISSIONDONE, true);
        }
    }

    private void Reset()
    {
        m_instance = GetComponent<DialogueInstance>();
    }
}
