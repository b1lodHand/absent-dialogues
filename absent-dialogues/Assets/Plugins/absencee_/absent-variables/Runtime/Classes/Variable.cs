using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.variablesystem
{
    /// <summary>
    /// The very base class of whole system. Needed because of the serialization limitations
    /// of unity on generic types.
    /// </summary>
    [System.Serializable]
    public abstract class Variable
    {

    }

    /// <summary>
    /// The base class for any type of variable. You can override the effect of mutators by deriving this class. Or you can
    /// use it diretly as a generic class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Variable<T> : Variable
    {
        protected event Action<VariableValueChangedCallbackContext<T>> m_onValueChanged;

        [SerializeField] protected string m_name;
        public string Name { get => m_name; protected set => m_name = value; }

        [SerializeField] protected T m_value;
        public T Value
        {
            get
            {
                return m_value;
            }

            set
            {
                var previous = m_value;
                var context = new VariableValueChangedCallbackContext<T>() { previousValue = previous, newValue = value };
                m_onValueChanged?.Invoke(context);
                m_value = value;
            }
        }

        protected List<Mutation<T>> m_mutations  = new List<Mutation<T>>();

        /// <summary>
        /// Mutate this variable.
        /// </summary>
        /// <param name="mutation"></param>
        public void AddMutation(Mutation<T> mutation)
        {
            RevertMutations();
            m_mutations.Add(mutation);
            ApplyMutations();
        }

        /// <summary>
        /// Remove a specific mutation from this variable.
        /// </summary>
        /// <param name="mutation"></param>
        public void RemoveMutation(Mutation<T> mutation)
        {
            RevertMutations();
            if(m_mutations.Contains(mutation)) m_mutations.Remove(mutation);
            ApplyMutations();
        }

        /// <summary>
        /// Clear all of the mutations on this variable.
        /// </summary>
        public void ClearMutationa()
        {
            RevertMutations();
            m_mutations.Clear();
            ApplyMutations();
        }

        protected abstract void RevertMutations();
        protected abstract void ApplyMutations();

        /// <summary>
        /// Get notified when the value of this variable changes.
        /// </summary>
        /// <param name="evt"></param>
        public void AddValueChangeListener(Action<VariableValueChangedCallbackContext<T>> evt)
        {
            m_onValueChanged += evt;
        }

        /// <summary>
        /// Remove the notification you've added.
        /// </summary>
        /// <param name="evt"></param>
        public void RemoveValueChangeListener(Action<VariableValueChangedCallbackContext<T>> evt)
        {
            m_onValueChanged -= evt;
        }
    }

    [System.Serializable]
    public class Variable_Integer : Variable<int>
    {
        protected override void RevertMutations()
        {
            if (m_mutations.Count == 0) return;

            var value = Value;

            var additives = m_mutations.Where(m => m.MutationType == MutationType.Additive).ToList();
            var multiplicatives = m_mutations.Where(m => m.MutationType == MutationType.Multiplicative).ToList();

            multiplicatives.ForEach(m =>
            {
                value /= m.MutationValue;
            });

            additives.ForEach(m =>
            {
                value -= m.MutationValue;
            });

            Value = value;
        }

        protected override void ApplyMutations()
        {
            if(m_mutations.Count == 0) return;

            var value = Value;

            var additives = m_mutations.Where(m => m.MutationType == MutationType.Additive).ToList();
            var multiplicatives = m_mutations.Where(m => m.MutationType == MutationType.Multiplicative).ToList();

            additives.ForEach(m =>
            {
                value += m.MutationValue;
            });

            multiplicatives.ForEach(m =>
            {
                value *= m.MutationValue;
            });

            Value = value;
        }
    }

    [System.Serializable]
    public class Variable_Float : Variable<float>
    {
        protected override void RevertMutations()
        {
            var value = Value;

            var additives = m_mutations.Where(m => m.MutationType == MutationType.Additive).ToList();
            var multiplicatives = m_mutations.Where(m => m.MutationType == MutationType.Multiplicative).ToList();

            multiplicatives.ForEach(m =>
            {
                value /= m.MutationValue;
            });

            additives.ForEach(m =>
            {
                value -= m.MutationValue;
            });

            Value = value;
        }

        protected override void ApplyMutations()
        {
            var value = Value;

            var additives = m_mutations.Where(m => m.MutationType == MutationType.Additive).ToList();
            var multiplicatives = m_mutations.Where(m => m.MutationType == MutationType.Multiplicative).ToList();

            additives.ForEach(m =>
            {
                value += m.MutationValue;
            });

            multiplicatives.ForEach(m =>
            {
                value *= m.MutationValue;
            });

            Value = value;
        }
    }

    [System.Serializable]
    public class Variable_Vector3
    {
        public Variable_Float X {  get; set; } = new Variable_Float();
        public Variable_Float Y { get; set; } = new Variable_Float();
        public Variable_Float Z { get; set; } = new Variable_Float();

        public Vector3 GetValue() => new Vector3(X.Value, Y.Value, Z.Value);
        public void SetValue(Vector3 newValue)
        {
            X.Value = newValue.x;
            Y.Value = newValue.y;
            Z.Value = newValue.z;
        }

        public void AddMutation(Mutation<float> mutation)
        {
            X.AddMutation(mutation);
            Y.AddMutation(mutation);
            Z.AddMutation(mutation);
        }
    }

    [System.Serializable]
    public class Variable_String : Variable<string>
    {
        protected override void ApplyMutations()
        {
            
        }

        protected override void RevertMutations()
        {
            
        }
    }

    [System.Serializable]
    public class Variable_Boolean : Variable<bool>
    {
        protected override void ApplyMutations()
        {

        }

        protected override void RevertMutations()
        {
            
        }
    }

    /// <summary>
    /// Used for the event system of variables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VariableValueChangedCallbackContext<T>
    {
        public T previousValue { get; set; }
        public T newValue { get; set; }
    }
}
