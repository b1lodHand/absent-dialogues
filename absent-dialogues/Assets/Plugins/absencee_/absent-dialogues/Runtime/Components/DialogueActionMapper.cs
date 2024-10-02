using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace com.absence.dialoguesystem.runtime
{
    public class DialogueActionMapper : DialogueExtensionBase
    {
        [SerializeField] private List<ActionMapPair> m_actionMapPairs = new();

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Action Mapper")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueActionMapper>();
        }
#endif

        [Button("Seach for new mapped action nodes")]
        void Refresh()
        {
            Cleanup();
            Search();
            Fetch();
        }

        void Fetch()
        {
            m_actionMapPairs.ForEach(pair =>
            {
                if (pair.Enabled != false) return;

                bool solved = true;

                if (pair.TargetActionNode == null)
                {
                    Node backupNode = m_instance.ReferencedDialogue.AllNodes.FirstOrDefault(node => node.Guid == pair.BackupGuid);

                    if (backupNode != null) pair.TargetActionNode = backupNode as ActionNode;
                    else solved = false;
                }

                if (!pair.TargetActionNode.UsedByMapper)
                {
                    solved = false;
                }

                if (solved) pair.Enabled = true;
            });
        }

        void Search()
        {
            m_instance.ReferencedDialogue.AllNodes.ForEach(node =>
            {
                if (node is not ActionNode actionNode) return;
                if (!actionNode.UsedByMapper) return;
                if (m_actionMapPairs.Any(pair => pair.TargetActionNode == actionNode)) return;

                m_actionMapPairs.Add(new ActionMapPair(actionNode));
            });
        }

        void Cleanup()
        {
            for (int i = 0; i < m_actionMapPairs.Count; i++)
            {
                ActionMapPair pair = m_actionMapPairs[i];   

                if (pair.TargetActionNode == null) m_actionMapPairs.Remove(pair);
                if (!pair.TargetActionNode.UsedByMapper) pair.Enabled = false;
            }
        }

        [System.Serializable]
        public class ActionMapPair
        {
#if UNITY_EDITOR
            [System.Serializable]
            public class EditorProps
            {
                public bool IsFoldout = false;

                public EditorProps()
                {
                    IsFoldout = false;
                }
            }
#endif
            public ActionNode TargetActionNode;
            public UnityEvent AttachedEvent;
            public bool Enabled;
            public string BackupGuid;

#if UNITY_EDITOR
            [SerializeField] private EditorProps m_editorProperties = new();
#endif

            public ActionMapPair(ActionNode targetActionNode)
            {
                TargetActionNode = targetActionNode;
                AttachedEvent = new();
                Enabled = true;
                BackupGuid = targetActionNode.Guid;

                TargetActionNode.OnValidation -= OnNodeValidate;
                TargetActionNode.OnValidation += OnNodeValidate;

                TargetActionNode.OnRemove -= OnNodeRemove;
                TargetActionNode.OnRemove += OnNodeRemove;
            }

            private void OnNodeRemove()
            {
                Enabled = false;
            }

            private void OnNodeValidate()
            {
                Enabled = TargetActionNode.UsedByMapper;
            }
        }
    }
}
