using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.variablebanks.internals
{
    /// <summary>
    /// The static class responsible for cloning the banks at startup.
    /// </summary>
    public static class VariableBanksCloningHandler
    {
        private static Dictionary<string, VariableBank> m_bankTable = new();

        /// <summary>
        /// Action which will get invoked when cloning process gets completed successfully. It gets cleared automatically after invoking.
        /// </summary>
        public static event Action OnCloningCompleted;

        /// <summary>
        /// Use to check if the cloning process got completed successfully.
        /// </summary>
        public static bool CloningCompleted { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void CloneAllBanks()
        {
            m_bankTable.Clear();
            CloningCompleted = false;

            UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<VariableBank>(internals.Constants.K_ADDRESSABLES_TAG, null, true).Completed += asyncOperationHandle =>
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
                        if (bank.ForExternalUse) return;

                        VariableBank clonedBank = bank.Clone();

                        m_bankTable.Add(bank.Guid, clonedBank);

                        Debug.Log(clonedBank.name);
                    });

                    UnityEngine.AddressableAssets.Addressables.Release(asyncOperationHandle);

                    CloningCompleted = true;

                    OnCloningCompleted?.Invoke();
                    OnCloningCompleted = null;
                }
            };
        }

        /// <summary>
        /// Adds the action passed to <see cref="OnCloningCompleted"/> if the cloning process is not ended yet. If it is ended already,
        /// the action passed gets invoked instantly.
        /// </summary>
        /// <param name="callbackContext"></param>
        /// <returns></returns>
        public static bool AddCloningCompleteCallbackOrInvoke(Action callbackContext)
        {
            if (CloningCompleted)
            {
                callbackContext?.Invoke();
                return true;
            }

            OnCloningCompleted += callbackContext;
            return false;
        }

        /// <summary>
        /// Use to get a clone bank with a specific Guid. <b>Runtime only.</b>
        /// </summary>
        /// <param name="guidOfOriginalBank">Target Guid.</param>
        /// <returns>The clone bank or null.</returns>
        internal static VariableBank GetCloneWithGuid(string guidOfOriginalBank)
        {
            if (!Application.isPlaying) throw new Exception("You cannot call GetCloneWithGuid in editor!");
            if (!CloningCompleted) throw new Exception("Make sure the cloning process has ended successfully before calling GetCloneWithGuid() function.");

            if (!m_bankTable.ContainsKey(guidOfOriginalBank)) throw new Exception("Specified variablebank is not cloned by the internal system. " +
                "Check if it's included in the Addressables menu. And also check it's AvoidCloning property.");

            return m_bankTable[guidOfOriginalBank];
        }
    }

}