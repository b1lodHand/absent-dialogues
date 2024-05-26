using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// The type to hold references to dialogue options.
    /// </summary>
    [System.Serializable]
    public class Option
    {
        /// <summary>
        /// Speech of this option.
        /// </summary>
        [HideInInspector] public string Speech;

        /// <summary>
        /// Boolean which decides if <see cref="ShowIf"/> will be used.
        /// </summary>
        [HideInInspector] public bool UseShowIf = false;

        /// <summary>
        /// The condition checker which decides the visibility of the option.
        /// </summary>
        [HideInInspector] public VariableComparer ShowIf;

        /// <summary>
        /// The node this option leads to.
        /// </summary>
        [HideInInspector] public Node LeadsTo;

        /// <summary>
        /// Additional speech data this option contains.
        /// </summary>
        public AdditionalSpeechData AdditionalData;

        /// <summary>
        /// Use to get a clone of this option.
        /// </summary>
        /// <param name="overrideBank"></param>
        /// <returns></returns>
        public Option Clone(VariableBank overrideBank)
        {
            Option clone = new Option();
            clone.Speech = Speech;
            clone.UseShowIf = UseShowIf;
            clone.ShowIf = ShowIf.Clone(overrideBank);
            clone.LeadsTo = LeadsTo;
            clone.AdditionalData = AdditionalData;

            return clone;
        }
    }
}