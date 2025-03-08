using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.absence.dialoguesystem.builtin
{
    public class DialogueActionMapper : DialogueExtensionBase
    {
        [SerializeField] private List<ActionMapPair> m_actionMapPairs = new();

        Dialogue m_lastCheckedDialogue;

        [Button("Search for new mapped event nodes")]
        void Refresh()
        {
            Cleanup();
            Search();
            Fetch();
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssetIfDirty(gameObject);
#endif
        }

        public override void OnProgress(Node frame, DialogueFlowContext context)
        {
            if (!context.InvokeAction) return;

            ActionMapPair targetPair = 
                m_actionMapPairs.FirstOrDefault(pair => pair.TargetActionNode.UniqueMapperId.Equals(context.ActionId));

            if (targetPair == null) return;

            targetPair.AttachedEvent?.Invoke();
            context.InvokeAction = false;
            context.ActionId = string.Empty;
        }

        public override void OnInstanceValidate()
        {
            Dialogue newDialogue = m_instance.ReferencedDialogue;
            if (m_lastCheckedDialogue != newDialogue)
            {
                m_actionMapPairs.Clear();
                Refresh();
            }

            m_lastCheckedDialogue = newDialogue;
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

                    if (backupNode != null) pair.TargetActionNode = backupNode as EventNode;
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
            if (m_instance.ReferencedDialogue == null)
                return;

            m_instance.ReferencedDialogue.AllNodes.ForEach(node =>
            {
                if (node is not EventNode actionNode) return;
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
                EventNode actionNode = pair.TargetActionNode;

                if (actionNode == null) pair.Enabled = false;
                if (!pair.TargetActionNode.UsedByMapper) pair.Enabled = false;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/Action Mapper")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<DialogueActionMapper>();
        }
#endif

        [System.Serializable]
        public class ActionMapPair
        {
            public EventNode TargetActionNode;
            public UnityEvent AttachedEvent;
            public bool Enabled;
            public string BackupId;
            public string BackupGuid;

            public ActionMapPair(EventNode targetActionNode)
            {
                TargetActionNode = targetActionNode;
                AttachedEvent = new();
                Enabled = true;
                BackupId = targetActionNode.UniqueMapperId;
                BackupGuid = targetActionNode.Guid;

                TargetActionNode.onValidation -= OnNodeValidate;
                TargetActionNode.onValidation += OnNodeValidate;

                TargetActionNode.onRemove -= OnNodeRemove;
                TargetActionNode.onRemove += OnNodeRemove;
            }

            ~ActionMapPair()
            {
                TargetActionNode.onValidation -= OnNodeValidate;
                TargetActionNode.onRemove -= OnNodeRemove;
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
