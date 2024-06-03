using com.absence.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace com.absence.variablesystem
{
    public class VariableBank : ScriptableObject
    {
        public static readonly string Null = "null: null";
        [SerializeField, Readonly] private string m_guid = Guid.NewGuid().ToString();
        public string GUID => m_guid;

        #region Cloning
        private static Dictionary<string, VariableBank> m_bankTable = new();
        public static event Action OnCloningCompleted;
        public static bool CloningCompleted { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void CloneAll()
        {
            m_bankTable.Clear();
            CloningCompleted = false;

            Addressables.LoadAssetsAsync<VariableBank>("variable-banks", null, true).Completed += asyncOperationHandle =>
            {
                if (asyncOperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
                {
                    throw new Exception("Failed to load variable banks.");
                }

                else if (asyncOperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    List<VariableBank> originalBanks = asyncOperationHandle.Result.ToList();

                    originalBanks.ForEach(bank =>
                    {
                        if (bank.IsClone) return;

                        VariableBank clonedBank = bank.Clone();

                        m_bankTable.Add(bank.GUID, clonedBank);

                        Debug.Log(clonedBank.name);
                    });

                    Addressables.Release(asyncOperationHandle);

                    CloningCompleted = true;

                    OnCloningCompleted?.Invoke();
                    OnCloningCompleted = null;
                }
            };
        }
        public static VariableBank GetClone(string guidOfOriginalBank)
        {
            if (!m_bankTable.ContainsKey(guidOfOriginalBank)) throw new Exception("Specified variablebank is not cloned by the internal system. " +
                "Check if it's included in the Addressables menu. And also check it's AvoidCloning property.");

            return m_bankTable[guidOfOriginalBank];
        }
        #endregion

        [SerializeField] protected List<Variable_Integer> m_ints = new();
        [SerializeField] protected List<Variable_Float> m_floats = new();
        [SerializeField] protected List<Variable_String> m_strings = new();
        [SerializeField] protected List<Variable_Boolean> m_booleans = new();

        public List<Variable_Integer> Ints { get { return m_ints; } internal set { m_ints = value; } }
        public List<Variable_Float> Floats { get { return m_floats; } internal set { m_floats = value; } }
        public List<Variable_String> Strings { get { return m_strings; } internal set { m_strings = value; } }
        public List<Variable_Boolean> Booleans { get { return m_booleans; } internal set { m_booleans = value; } }

        public event Action OnDestroyAction;

        private bool m_avoidCloning = false;
        public bool AvoidCloning
        {
            get
            {
                return m_avoidCloning;
            }

            set
            {
                if (Application.isPlaying) throw new Exception("You cannot set AvoidCloning of a bank runtime!");

                m_avoidCloning = value;
            }
        }

        private bool m_showOnList = true;
        public bool ShowOnList
        {
            get
            {
                return m_showOnList;
            }

            set
            {
                if (Application.isPlaying) throw new Exception("You cannot set AvoidCloning of a bank runtime!");

                m_showOnList = value;
            }
        }

        private VariableBank m_clonedFrom = null;
        public VariableBank ClonedFrom => m_clonedFrom;

        public bool IsClone => m_clonedFrom != null;

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

        public bool TryGetInt(string variableName, out int value)
        {
            variableName = TrimVariableNameType(variableName);
            value = 0;

            List<Variable_Integer> check = m_ints.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }
        public bool TryGetFloat(string variableName, out float value)
        {
            variableName = TrimVariableNameType(variableName);
            value = 0f;

            List<Variable_Float> check = m_floats.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }
        public bool TryGetString(string variableName, out string value)
        {
            variableName = TrimVariableNameType(variableName);
            value = string.Empty;

            List<Variable_String> check = m_strings.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }
        public bool TryGetBoolean(string variableName, out bool value)
        {
            variableName = TrimVariableNameType(variableName);
            value = false;

            List<Variable_Boolean> check = m_booleans.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return false;

            value = check.FirstOrDefault().Value;
            return true;
        }

        public void AddValueChangeListenerToInt(string variableName, Action<VariableValueChangedCallbackContext<int>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Integer> check = m_ints.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }
        public void AddValueChangeListenerToFloat(string variableName, Action<VariableValueChangedCallbackContext<float>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Float> check = m_floats.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }
        public void AddValueChangeListenerToString(string variableName, Action<VariableValueChangedCallbackContext<string>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_String> check = m_strings.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }
        public void AddValueChangeListenerToBoolean(string variableName, Action<VariableValueChangedCallbackContext<bool>> callbackAction)
        {
            variableName = TrimVariableNameType(variableName);

            List<Variable_Boolean> check = m_booleans.Where(v => v.Name == variableName).ToList();
            if (check.Count == 0) return;

            check.FirstOrDefault().AddValueChangeListener(callbackAction);
        }

        public bool SetInt(string variableName, int newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_ints.Where(v => v.Name == variableName).FirstOrDefault();
            if(found == null) return false;

            found.Value = newValue;
            return true;
        }
        public bool SetFloat(string variableName, float newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_floats.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }
        public bool SetString(string variableName, string newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_strings.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }
        public bool SetBoolean(string variableName, bool newValue)
        {
            variableName = TrimVariableNameType(variableName);

            var found = m_booleans.Where(v => v.Name == variableName).FirstOrDefault();
            if (found == null) return false;

            found.Value = newValue;
            return true;
        }

        public bool HasInt(string variableName) => m_ints.Any(v => v.Name == TrimVariableNameType(variableName));
        public bool HasFloat(string variableName) => m_floats.Any(v => v.Name == TrimVariableNameType(variableName));
        public bool HasString(string variableName) => m_strings.Any(v => v.Name == TrimVariableNameType(variableName));
        public bool HasBoolean(string variableName) => m_booleans.Any(v => v.Name == TrimVariableNameType(variableName));
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
        string AddVariableNameType(string nameToAddType)
        {
            StringBuilder builder = new StringBuilder();
            if (this.HasInt(nameToAddType)) builder.Append("int: ");
            else if (this.HasFloat(nameToAddType)) builder.Append("float: ");
            else if (this.HasString(nameToAddType)) builder.Append("string: ");
            else if (this.HasBoolean(nameToAddType)) builder.Append("bool: ");

            builder.Append(" ");
            builder.Append(nameToAddType);

            return builder.ToString();
        }

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