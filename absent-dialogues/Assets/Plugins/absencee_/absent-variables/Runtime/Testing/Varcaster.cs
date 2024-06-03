using com.absence.attributes;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.variablesystem.testing
{
    [RequireComponent(typeof(VariableBankAcquirer))]
    public class Varcaster : MonoBehaviour
    {
        [SerializeField, Readonly] private VariableBankAcquirer m_acquirer; 

        [SerializeField] private List<VariableComparer> m_comparers = new();
        [SerializeField] private List<VariableSetter> m_setters = new();

        [SerializeField] private List<FixedVariableComparer> m_fixedComparers = new();
        [SerializeField] private List<FixedVariableSetter> m_fixedSetters = new();

        private void OnValidate()
        {
            RefreshFixedManipulators();
        }

        private void OnEnable()
        {
            m_acquirer.OnTargetGuidChanged += RefreshFixedManipulators;
        }

        private void OnDisable()
        {
            m_acquirer.OnTargetGuidChanged -= RefreshFixedManipulators;
        }

        private void RefreshFixedManipulators()
        {
            m_fixedComparers.ForEach(comparer => comparer.SetFixedBank(m_acquirer.TargetGuid));
            m_fixedSetters.ForEach(setter => setter.SetFixedBank(m_acquirer.TargetGuid));
        }

        private void Reset()
        {
            m_acquirer = GetComponent<VariableBankAcquirer>();
            m_acquirer.OnTargetGuidChanged -= RefreshFixedManipulators;
            m_acquirer.OnTargetGuidChanged += RefreshFixedManipulators;
        }
    }

}