using com.absence.variablebanks.internals;
using System;
using UnityEngine;

namespace com.absence.variablebanks
{
    /// <summary>
    /// A component to reference banks both in editor and runtime.
    /// </summary>
    public class VariableBankAcquirer : MonoBehaviour
    {
        [SerializeField] private string m_targetGuid;

        /// <summary>
        /// Use to get the Guid of the referenced bank.
        /// </summary>
        public string TargetGuid => m_targetGuid;

        [SerializeField] private VariableBank m_bank;

        /// <summary>
        /// Use to get clone of the referenced bank. <b>Runtime only.</b>
        /// </summary>
        public VariableBank Bank
        {
            get
            {
                if (!Application.isPlaying) throw new Exception("You cannot retrive the bank directly from an acquirer in editor. Use TargetGuid instead.");
                if (!VariableBanksCloningHandler.CloningCompleted) throw new Exception("Cloning has not yet been completed.");

                return m_bank;
            }
        }

        private void Awake()
        {
            VariableBanksCloningHandler.AddCloningCompleteCallbackOrInvoke(FetchFixedBank);
        }

        private void FetchFixedBank()
        {
            m_bank = VariableBank.GetInstance(m_targetGuid);
        }
    }
}