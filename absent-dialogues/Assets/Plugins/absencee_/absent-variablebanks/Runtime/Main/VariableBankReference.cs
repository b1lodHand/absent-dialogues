using com.absence.variablebanks.internals;
using System;
using UnityEngine;

namespace com.absence.variablebanks
{
    /// <summary>
    /// The class responsible for letting you reference a <see cref="VariableBank"/> both in editor and in runtime. You can use the
    /// <see cref="VariableBank"/> class directly if the bank you are referencing is marked as <see cref="VariableBank.ForExternalUse"/>. 
    /// For more information, read the docs.
    /// </summary>
    [System.Serializable]
    public class VariableBankReference
    {
        [SerializeField] private string m_targetGuid;

        /// <summary>
        /// Use to get the referenced bank's Guid. Returns an empty string if no banks referenced.
        /// </summary>
        public string TargetGuid => m_targetGuid;

        /// <summary>
        /// Use to get the bank referenced. <b>Runtime only.</b>
        /// </summary>
        public VariableBank Bank => VariableBank.GetInstance(m_targetGuid);
    }

}