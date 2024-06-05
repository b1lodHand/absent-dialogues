using com.absence.attributes;
using com.absence.variablebanks.internals;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.variablebanks
{
    /// <summary>
    /// The scriptable object represents a bank of variables.
    /// </summary>
    public class VariableBank : ScriptableObject
    {
        /// <summary>
        /// A constant string that represents a null variable name (with the prefix).
        /// </summary>
        public const string Null = "null: null";

        [SerializeField, Readonly, Tooltip("Guid of this bank.")] 
        private string m_guid = System.Guid.NewGuid().ToString();

        /// <summary>
        /// Guid of this bank.
        /// </summary>
        public string Guid => m_guid;

        /// <summary>
        /// Use to get a cloned bank with a specific Guid. <b>Runtime Only.</b>
        /// </summary>
        /// <param name="targetGuid">Target Guid.</param>
        /// <returns>Throws an error if a clone with the target Guid does not exist. Returns the bank otherwise.</returns>
        public static VariableBank GetInstance(string targetGuid) => VariableBanksCloningHandler.GetCloneWithGuid(targetGuid);

        [SerializeField, Readonly, Tooltip("If true, this bank won't get cloned in the startup and also will not get shown on the variable bank name lists.")] 
        private bool m_forExternalUse = false;

        /// <summary>
        /// If true, this bank won't get cloned in the startup and also will not get shown on the variable bank name lists. Set to
        /// true if you'll use direct references of such. For more information, read the docs.
        /// </summary>
        public bool ForExternalUse
        {
            get
            {
                return m_forExternalUse;
            }

            set
            {
                if (Application.isPlaying) throw new Exception("You cannot set AvoidCloning of a bank runtime!");

                m_forExternalUse = value;
            }
        }

        [SerializeField] protected List<Variable_Integer> m_ints = new();
        [SerializeField] protected List<Variable_Float> m_floats = new();
        [SerializeField] protected List<Variable_String> m_strings = new();
        [SerializeField] protected List<Variable_Boolean> m_booleans = new();

        /// <summary>
        /// All of the integer variables within this bank.
        /// </summary>
        public List<Variable_Integer> Ints { get { return m_ints; } private set { m_ints = value; } }

        /// <summary>
        /// All of the floating point variables within this bank.
        /// </summary>
        public List<Variable_Float> Floats { get { return m_floats; } private set { m_floats = value; } }

        /// <summary>
        /// All of the string variables within this bank.
        /// </summary>
        public List<Variable_String> Strings { get { return m_strings; } private set { m_strings = value; } }

        /// <summary>
        /// All of the boolean variables within this bank.
        /// </summary>
        public List<Variable_Boolean> Booleans { get { return m_booleans; } private set { m_booleans = value; } }

        /// <summary>
        /// The action gets invoked when this bank gets destroyed.
        /// </summary>
        public event Action OnDestroyAction;

        private VariableBank m_clonedFrom = null;

        /// <summary>
        /// Returns null if this is not a clone. Returns the original bank if this is a clone.
        /// </summary>
        public VariableBank ClonedFrom => m_clonedFrom;

        /// <summary>
        /// Use to check if this bank is a clone.
        /// </summary>
        public bool IsClone => m_clonedFrom != null;

        /// <summary>
        /// Use to get a list of all variables' names of this bank.
        /// </summary>
        /// <returns>A list of variable names. Example: "example_int"</returns>
        public List<string> GetAllVariableNames()
        {
            var totalCount = m_ints.Count + m_floats.Count + m_strings.Count + m_booleans.Count;
            var result = new List<string>();
            if (totalCount == 0) return result;

            m_ints.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add(k));
            m_floats.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add(k));
            m_strings.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add(k));
            m_booleans.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add(k));

            return result;
        }

        /// <summary>
        /// Use to get a list of all variables' names of this bank, each one of the names will contain a type prefix. <b>Those
        /// prefixes get trimmed when you pass them to any function of a variable bank.</b>
        /// </summary>
        /// <returns>A list of all variable names with the prefixes. Example: "int: example_int"</returns>
        public List<string> GetAllVariableNamesWithTypes()
        {
            var totalCount = m_ints.Count + m_floats.Count + m_strings.Count + m_booleans.Count;
            var result = new List<string>();
            if (totalCount == 0) return result;

            m_ints.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add($"int: {k}"));
            m_floats.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add($"float: {k}"));
            m_strings.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add($"string: {k}"));
            m_booleans.ConvertAll(v => v.Name).ToList().ForEach(k => result.Add($"bool: {k}"));

            return result;
        }


        /// <summary>
        /// Use to get value of an integer variable within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="value">Value of the variable.</param>
        /// <returns>True if a variable with the target name exists within the bank.</returns>
        public bool TryGetInt(string variableName, out int value)
        {
            variableName = TrimVariableNameType(variableName);
            value = 0;

            List<Variable_Integer> check = m_ints.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }

        /// <summary>
        /// Use to get value of a floating point variable within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="value">Value of the variable.</param>
        /// <returns>True if a variable with the target name exists within the bank.</returns>
        public bool TryGetFloat(string variableName, out float value)
        {
            variableName = TrimVariableNameType(variableName);
            value = 0f;

            List<Variable_Float> check = m_floats.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }

        /// <summary>
        /// Use to get value of a string variable within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="value">Value of the variable.</param>
        /// <returns>True if a variable with the target name exists within the bank.</returns>
        public bool TryGetString(string variableName, out string value)
        {
            variableName = TrimVariableNameType(variableName);
            value = string.Empty;

            List<Variable_String> check = m_strings.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }

        /// <summary>
        /// Use to get value of a boolean variable within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="value">Value of the variable.</param>
        /// <returns>True if a variable with the target name exists within the bank.</returns>
        public bool TryGetBoolean(string variableName, out bool value)
        {
            variableName = TrimVariableNameType(variableName);
            value = false;

            List<Variable_Boolean> check = m_booleans.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }

        /// <summary>
        /// Use to add a value change callback to an integer variable with a specific name.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="callbackAction">What to do when value of the variable changes.</param>
        public void AddValueChangeListenerToInt(string variableName, Action<VariableValueChangedCallbackContext<int>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Integer> check = m_ints.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }

        /// <summary>
        /// Use to add a value change callback to a floating point variable with a specific name.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="callbackAction">What to do when value of the variable changes.</param>
        public void AddValueChangeListenerToFloat(string variableName, Action<VariableValueChangedCallbackContext<float>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Float> check = m_floats.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }

        /// <summary>
        /// Use to add a value change callback to a string variable with a specific name.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="callbackAction">What to do when value of the variable changes.</param>
        public void AddValueChangeListenerToString(string variableName, Action<VariableValueChangedCallbackContext<string>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_String> check = m_strings.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }

        /// <summary>
        /// Use to add a value change callback to a boolean variable with a specific name.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="callbackAction">What to do when value of the variable changes.</param>
        public void AddValueChangeListenerToBoolean(string variableName, Action<VariableValueChangedCallbackContext<bool>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Boolean> check = m_booleans.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }

        /// <summary>
        /// Use to change an integer variable's value.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="newValue">New value for the variable.</param>
        /// <returns>True if value changing process ended successfully. False otherwise.</returns>
        public bool SetInt(string variableName, int newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_ints.Where(v => v.Name == variableName).FirstOrDefault();
            if(found == null) return false;

            found.Value = newValue;
            return true;
        }

        /// <summary>
        /// Use to change a floating point variable's value.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="newValue">New value for the variable.</param>
        /// <returns>True if value changing process ended successfully. False otherwise.</returns>
        public bool SetFloat(string variableName, float newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_floats.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }

        /// <summary>
        /// Use to change a string variable's value.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="newValue">New value for the variable.</param>
        /// <returns>True if value changing process ended successfully. False otherwise.</returns>
        public bool SetString(string variableName, string newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_strings.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }

        /// <summary>
        /// Use to change a boolean variable's value.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <param name="newValue">New value for the variable.</param>
        /// <returns>True if value changing process ended successfully. False otherwise.</returns>
        public bool SetBoolean(string variableName, bool newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_booleans.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }

        /// <summary>
        /// Use to check if an integer variable with the target name exists within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasInt(string variableName) => m_ints.Any(v => v.Name == TrimVariableNameType(variableName));

        /// <summary>
        /// Use to check if a floating point variable with the target name exists within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasFloat(string variableName) => m_floats.Any(v => v.Name == TrimVariableNameType(variableName));

        /// <summary>
        /// Use to check if a string variable with the target name exists within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasString(string variableName) => m_strings.Any(v => v.Name == TrimVariableNameType(variableName));

        /// <summary>
        /// Use to check if a boolean variable with the target name exists within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasBoolean(string variableName) => m_booleans.Any(v => v.Name == TrimVariableNameType(variableName));

        /// <summary>
        /// Use to check if a variable with the target name exists within this bank.
        /// </summary>
        /// <param name="variableName">Target name.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public bool HasAny(string variableName)
        {
            return (this.HasInt(variableName) ||
                    this.HasFloat(variableName) ||
                    this.HasString(variableName) ||
                    this.HasBoolean(variableName));
        }

        string TrimVariableNameType(string nameToTrim)
        {
            if (!nameToTrim.Contains(':')) return nameToTrim;
            return nameToTrim.Split(':')[1].Trim();
        }

        /// <summary>
        /// Use to clone this bank.
        /// </summary>
        /// <returns>Returns the clone created.</returns>
        public VariableBank Clone()
        {
            VariableBank clone = Instantiate(this);

            clone.m_clonedFrom = this;
            clone.m_guid = m_guid;
            clone.OnDestroyAction = null;

            return clone;
        }

        private void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }
    }
}