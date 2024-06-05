using System.Collections.Generic;
using UnityEngine;

namespace com.absence.variablebanks.testing
{
    public class Varcaster : MonoBehaviour
    {
        [SerializeField] private VariableBankReference m_fixedBankReference;

        [SerializeField] private List<VariableComparer> m_comparers = new();
        [SerializeField] private List<VariableSetter> m_setters = new();

        [SerializeField] private List<FixedVariableComparer> m_fixedComparers = new();
        [SerializeField] private List<FixedVariableSetter> m_fixedSetters = new();

        private void OnValidate()
        {
            RefreshFixedManipulators();
        }


        private void RefreshFixedManipulators()
        {
            m_fixedComparers.ForEach(comparer => comparer.SetFixedBank(m_fixedBankReference.TargetGuid));
            m_fixedSetters.ForEach(setter => setter.SetFixedBank(m_fixedBankReference.TargetGuid));
        }
    }

}