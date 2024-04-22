using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class MultiDialogueInstance : MonoBehaviour
    {
        [SerializeField] private List<DialogueReference> m_dialogues = new();
    }

    [System.Serializable]
    public class DialogueReference
    {
        public Dialogue Dialogue = null;
        public List<Person> OverridePeople = new();
    }
}
