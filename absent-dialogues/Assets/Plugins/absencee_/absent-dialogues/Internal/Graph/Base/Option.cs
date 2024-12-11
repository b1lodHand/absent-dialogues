using com.absence.attributes;
using com.absence.variablesystem.banksystembase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// The type to hold references to dialogue options.
    /// </summary>
    [System.Serializable]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Option.html")]
    public class Option
    {
        /// <summary>
        /// Speech of this option.
        /// </summary>
        [HideInInspector] public string Text;

        /// <summary>
        /// Additional speech data this option contains.
        /// </summary>
        public ExtraDialogueData ExtraData;

        [Space(10)]

        [SerializeField] private bool m_useShowIf = false;
        public bool UseShowIf
        {
            get
            {
                return m_useShowIf;
            }

            set
            {
                m_useShowIf = value;
            }
        }

        [SerializeField, ShowIf(nameof(m_useShowIf))] public ShowIf Visibility;

        /// <summary>
        /// The node this option leads to.
        /// </summary>
        [HideInInspector] public Node LeadsTo;

        /// <summary>
        /// Use to get a clone of this option.
        /// </summary>
        /// <param name="overrideBank"></param>
        /// <returns></returns>
        public Option Clone(VariableBank overrideBank)
        {
            Option clone = new Option();
            clone.Text = Text;
            clone.m_useShowIf = UseShowIf;
            clone.Visibility = Visibility.Clone(overrideBank);
            clone.LeadsTo = LeadsTo;
            clone.ExtraData = ExtraData;

            return clone;
        }

        /// <summary>
        /// Calculates the visibility of this option.
        /// </summary>
        /// <returns>Returns true if the option is visible, returns false otherwise.</returns>
        public bool IsVisible()
        {
            if (!m_useShowIf) return true;

            return Visibility.GetResult();
        }

        /// <summary>
        /// A class specifically designed for calculating an option's visibility.
        /// </summary>
        [System.Serializable]
        [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Option.ShowIf.html")]
        public class ShowIf
        {
            /// <summary>
            /// An enum which defines what to do with multiple comparers in conclusion.
            /// </summary>
            public VBProcessType Processor = VBProcessType.All;

            /// <summary>
            /// A list of all <see cref="VariableComparer"/>s which has a role on determining this option's
            /// visibility on display.
            /// </summary>
            public List<NodeVariableComparer> ShowIfList = new();

            /// <summary>
            /// Use to clone this instance.
            /// </summary>
            /// <param name="overrideBank"></param>
            /// <returns></returns>
            public ShowIf Clone(VariableBank overrideBank)
            {
                ShowIf clone = new ShowIf();
                clone.Processor = Processor;
                clone.ShowIfList = ShowIfList.ConvertAll(comparer => comparer.Clone(overrideBank));
                return clone;
            }

            /// <summary>
            /// Use to get the composite result of all of the comparers of this instance.
            /// </summary>
            /// <returns></returns>
            public bool GetResult()
            {
                if (ShowIfList.Count == 0) return true;

                return Processor switch
                {
                    VBProcessType.All => ShowIfList.All(comparer => comparer.GetResult()),
                    VBProcessType.Any => ShowIfList.Any(comparer => comparer.GetResult()),
                    _ => true,
                };
            }

            public override string ToString()
            {
                return GetConditionString(false);
            }

            public string GetConditionString(bool richText = false)
            {
                return Utilities.Comparison.GetConditionString(ShowIfList, Processor, richText);
            }
        }
    }
}