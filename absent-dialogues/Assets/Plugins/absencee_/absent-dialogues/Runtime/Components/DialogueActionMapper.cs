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

        public override void OnProgress(DialogueFlowContext context)
        {
            base.OnProgress(context);

            if (!context.InvokeAction) return;

            ActionMapPair targetPair = 
                m_actionMapPairs.FirstOrDefault(pair => pair.TargetActionNode.UniqueMapperId.Equals(context.ActionId));

            if (targetPair == null) return;

            targetPair.AttachedEvent?.Invoke();
            context.InvokeAction = false;
            context.ActionId = string.Empty;
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

                if (pair.TargetActionNode == null) pair.Enabled = false;
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
            public string BackupId;
            public string BackupGuid;

#if UNITY_EDITOR
            [SerializeField] private EditorProps m_editorProperties = new();
#endif

            public ActionMapPair(ActionNode targetActionNode)
            {
                TargetActionNode = targetActionNode;
                AttachedEvent = new();
                Enabled = true;
                BackupId = targetActionNode.UniqueMapperId;
                BackupGuid = targetActionNode.Guid;

                TargetActionNode.OnValidation -= OnNodeValidate;
                TargetActionNode.OnValidation += OnNodeValidate;

                TargetActionNode.OnRemove -= OnNodeRemove;
                TargetActionNode.OnRemove += OnNodeRemove;
            }

            ~ActionMapPair()
            {
                TargetActionNode.OnValidation -= OnNodeValidate;
                TargetActionNode.OnRemove -= OnNodeRemove;
            }

            private void OnNodeRemove()
            {
                Enabled = false;
            }

            private void OnNodeValidate()
            {
                BackupId = TargetActionNode.UniqueMapperId;
                Enabled = TargetActionNode.UsedByMapper;
            }
        }
    }
}
