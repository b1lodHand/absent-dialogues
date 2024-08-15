using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Any node subtype with this interface implemented will refresh its <see cref="VariableComparer"/>s and <see cref="VariableSetter"/>s
    /// to have the correct reference to the <see cref="Blackboard.Bank"/> of the current <see cref="Dialogue"/> everytime the
    /// editor window refreshes.
    /// </summary>
    internal interface IContainVariableManipulators
    {
        /// <summary>
        /// A list of comparers which you want to restrict in terms of <see cref="VariableBank"/> selection
        /// </summary>
        /// <returns></returns>
        List<NodeVariableComparer> GetComparers();

        /// <summary>
        /// A list of comparers which you want to restrict in terms of <see cref="VariableBank"/> selection
        /// </summary>
        /// <returns></returns>
        List<NodeVariableSetter> GetSetters();
    }
}