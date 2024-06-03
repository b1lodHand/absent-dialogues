using System;
using UnityEngine;

namespace com.absence.variablesystem
{
    public class VariableBankAcquirer : MonoBehaviour
    {
        [SerializeField] private string m_targetGuid;
        public string TargetGuid => m_targetGuid;

        [SerializeField] private VariableBank m_bank;
        public VariableBank Bank
        {
            get
            {
                if (!Application.isPlaying) throw new Exception("You cannot retrive the bank directly from an acquirer in editor. Use TargetGuid instead.");
                if (!VariableBank.CloningCompleted) throw new Exception("Cloning has not yet been completed.");

                return m_bank;
            }
        }

        public event Action OnTargetGuidChanged;

        private void OnEnable()
        {
            VariableBank.OnCloningCompleted += FetchFixedBank;
        }

        private void OnDisable()
        {
            VariableBank.OnCloningCompleted -= FetchFixedBank;
        }

        private void OnValidate()
        {
            OnTargetGuidChanged?.Invoke();
        }

        private void FetchFixedBank()
        {
            m_bank = VariableBank.GetClone(m_targetGuid);
        }
    }
}