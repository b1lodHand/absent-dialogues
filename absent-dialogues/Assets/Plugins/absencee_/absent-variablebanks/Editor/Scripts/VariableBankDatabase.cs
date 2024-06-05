using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace com.absence.variablebanks.editor
{
    /// <summary>
    /// The static class responsible for holding a list of all <see cref="VariableBank"/>s in the project. <b>Editor only! For runtime, use 
    /// <see cref="VariableBank.GetInstance(string)"/> instead.</b>
    /// </summary>
    [InitializeOnLoad]
    public static class VariableBankDatabase
    {
        static List<VariableBank> m_banks;

        /// <summary>
        /// All of the banks in the project.
        /// </summary>
        public static List<VariableBank> BanksInAssets => m_banks;

        static VariableBankDatabase()
        {
            m_banks = new List<VariableBank>();
            Refresh();
        }

        /// <summary>
        /// Returns true when there are no variable banks in the project's assets.
        /// </summary>
        public static bool NoBanks => m_banks.Count == 0;

        /// <summary>
        /// Use to refresh the variable bank database.
        /// </summary>
        public static void Refresh()
        {
            m_banks = AssetDatabase.FindAssets("t:VariableBank").ToList().ConvertAll(foundGuid =>
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundGuid), typeof(VariableBank)) as VariableBank;
            });
        }

        /// <summary>
        /// Use to check if a bank with the target Guid exists.
        /// </summary>
        /// <param name="bankGuid">Target Guid.</param>
        /// <returns>True if exists, false otherwise.</returns>
        public static bool Exists(string bankGuid)
        {
            return m_banks.Where(b => b.Guid == bankGuid).ToList().Count > 0;
        }

        public static VariableBank GetBankIfExists(string bankGuid)
        {
            return m_banks.Where(b => b.Guid == bankGuid).FirstOrDefault();
        }

        /// <summary>
        /// Get the index of the bank with the target Guid.
        /// </summary>
        /// <param name="bankGuid">Target Guid.</param>
        /// <returns>Returns <b>-1</b> if the bank with the target Guid does not exits. Returns the index otherwise.</returns>
        public static int GetIndexOf(string bankGuid)
        {
            VariableBank targetBank = m_banks.Where(bank => bank.Guid == bankGuid).FirstOrDefault();

            if (targetBank != null) return m_banks.IndexOf(targetBank);

            return -1;
        }

        /// <summary>
        /// Use to get Guid of a bank with a specific name.
        /// </summary>
        /// <param name="bankName">Target name.</param>
        /// <returns>Returns null if a bank with the target name does not exist. Returns the Guid otherwise.</returns>
        public static string NameToGuid(string bankName)
        {
            VariableBank bank = m_banks.Where(b => b.name == bankName).FirstOrDefault();

            if (bank != null) return bank.Guid;

            return null;
        }

        /// <summary>
        /// Use to get a list of all variable banks' names.
        /// </summary>
        /// <returns>Returns a list of all variable banks' (except of the ones marked as <see cref="VariableBank.ForExternalUse"/>) names.</returns>
        public static List<string> GetBankNameList()
        {
            List<string> result = new();

            if (m_banks.Count == 0) return result;

            m_banks.ForEach(bank =>
            {
                if (bank.ForExternalUse) return;

                result.Add(bank.name);
            });

            return result;
        }
    }
}
