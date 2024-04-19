using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Use to manage a single dialogue player.
    /// </summary>
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private bool m_startOnAwake = false;
        [SerializeField] private Dialogue m_dialogue;
        [SerializeField] private List<Person> m_overridePeople;
        private DialoguePlayer m_player;
        bool m_inDialogue = false;
        bool m_drawn = false;

        private void Awake()
        {
            if(m_dialogue != null) m_player = new DialoguePlayer(m_dialogue);
            if (m_startOnAwake) EnterDialogue();
        }

        private void Update()
        {

        }

        private bool CheckSkipInput()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public void EnterDialogue()
        {
            if (m_overridePeople.Count > 0) m_player.OverridePeople(m_overridePeople);
            m_inDialogue = true;

            DialogueDisplayer.Instance.Occupy(m_player);
        }

        public void ExitDialogue()
        {
            m_inDialogue = false;
            m_player.RevertPeople();

            DialogueDisplayer.Instance.Release();
        }

        private void OnApplicationQuit()
        {
            ExitDialogue();
        }
    }
}