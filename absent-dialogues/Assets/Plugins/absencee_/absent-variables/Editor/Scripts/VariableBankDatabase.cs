using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace com.absence.variablesystem.editor
{
    [InitializeOnLoad]
    public static class VariableBankDatabase
    {
        static List<VariableBank> m_banks;
        public static List<VariableBank> BanksInAssets => m_banks;

        static VariableBankDatabase()
        {
            m_banks = new List<VariableBank>();
            Refresh();
        }

        public static bool NoBanks => m_banks.Count == 0;

        public static void Refresh()
        {
            m_banks = AssetDatabase.FindAssets("t:VariableBank").ToList().ConvertAll(foundGuid =>
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundGuid), typeof(VariableBank)) as VariableBank;
            });
        }

        public static bool Exists(string bankGuid)
        {
            return m_banks.Where(b => b.GUID == bankGuid).ToList().Count > 0;
        }

        public static VariableBank GetBankIfExists(string bankGuid)
        {
            return m_banks.Where(b => b.GUID == bankGuid).FirstOrDefault();
        }

        public static int GetIndexOf(string bankGuid)
        {
            VariableBank targetBank = m_banks.Where(bank => bank.GUID == bankGuid).FirstOrDefault();

            if (targetBank != null) return m_banks.IndexOf(targetBank);

            return -1;
        }

        public static string NameToGuid(string bankName)
        {
            VariableBank bank = m_banks.Where(b => b.name == bankName).FirstOrDefault();

            if (bank != null) return bank.GUID;

            return null;
        }

        public static List<string> GetBankNameList()
        {
            List<string> result = new();

            if (m_banks.Count == 0) return result;

            m_banks.ForEach(bank =>
            {
                if (!bank.ShowOnList) return;

                result.Add(bank.name);
            });

            return result;
        }
    }
}
