using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace com.absence.dialoguesystem.internals 
{
    /// <summary>
    /// Node which displays a speech with options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html")]
    [MovedFrom("DecisionSpeechNode")]
    public class PromptNode : Node
    {
        public static string CreationMenuName => "Prompt";

        [Space(10)]
        
        [HideInInspector, SerializeField, Tooltip("All of the options of this node.")] 
        private List<Option> m_options = new List<Option>();

        [HideInInspector] public string m_text = "||NO TEXT||";

        [HideInInspector] public Node NativeNextNode; 

        public override bool PersonDependent => true;
        public override bool UseGenericOptions => true;

        public override string Text { get => m_text; set { m_text = value; } }
        public override List<Option> Options { get => m_options; set { m_options = value; } }

        public override string Title
        {
            get
            {
                if (NoOptionsOverall) return "Prompt (Optionless)";
                else return "Prompt";
            }
        }

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/PromptNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            int optionSelected = context.OptionIndex;
            int optionCount = m_options.Count;

            if (NoOptionsOverall)
                return NativeNextNode;

            if (NoCertainOptions && optionSelected == -1)
                return NativeNextNode;
            
            if (optionSelected >= optionCount)
                return GenericOptions[optionSelected - optionCount].LeadingNode;

            return m_options[optionSelected].LeadingNode;
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = nextWillBeAdded;
                return;
            }

            if (NoCertainOptions)
            {
                if (atPort == 0)
                {
                    NativeNextNode = nextWillBeAdded;
                    return;
                }

                else
                {
                    atPort--;
                }
            }

            if (atPort >= m_options.Count)
            {
                atPort -= m_options.Count;
#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Prompt Node (Add Generic Option Connection)");
#endif
                GenericOptions[atPort].LeadingNode = nextWillBeAdded;
                return;
            }

            m_options[atPort].LeadingNode = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = null;
                return;
            }

            if (NoCertainOptions)
            {
                if (atPort == 0)
                {
                    NativeNextNode = null;
                    return;
                }

                else
                {
                    atPort--;
                }
            }

            if (atPort >= m_options.Count)
            {
                atPort -= m_options.Count;
#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Prompt Node (Remove Generic Option Connection)");
#endif
                GenericOptions[atPort].LeadingNode = null;
                return;
            }

            m_options[atPort].LeadingNode = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            if (NoOptionsOverall)
            {
                result.Add(NativeNextNode);
                return;
            }

            if (NoCertainOptions)
            {
                result.Add(NativeNextNode);
            }

            foreach (Option option in m_options)
            {
                if (option != null) result.Add(option.LeadingNode);
                else result.Add(null);
            }

            foreach (GenericOptionReference genericOption in GenericOptions) 
            {
                if (genericOption != null && genericOption.Target != null)
                    result.Add(genericOption.LeadingNode);
                else
                    result.Add(null);
            }
        }

        public override List<string> GetDefaultOutputPortNames()
        {
            if (NoOptionsOverall)
                return new List<string>() { "To" };
            else if (NoCertainOptions) 
                return new List<string>() { "To" };

            return new List<string>();
        }

        public override List<NodeVariableComparer> Comparers
        {
            get
            {
                List<NodeVariableComparer> result = new();

                m_options.ForEach(option =>
                {
                    option.Visibility.ShowIfList.ForEach(comparer =>
                    {
                        result.Add(comparer);
                    });
                });

                return result;
            }
        }

        public override List<NodeVariableSetter> Setters => null;

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            m_text = dataToRead.Data;
            m_options = dataToRead.OptionData.ToList().ConvertAll(optionData => DataReader.ReadOptionData(optionData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.Data = m_text;
            dataToWrite.OptionData = m_options.ConvertAll(option => DataGenerator.GenerateOptionData(option)).ToArray();
        }
    }
}