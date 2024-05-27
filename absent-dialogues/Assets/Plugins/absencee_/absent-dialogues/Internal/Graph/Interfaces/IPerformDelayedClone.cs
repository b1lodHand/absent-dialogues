namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Any node subtypes with this interface implemented will have a second method to invoke right after the cloning process.
    /// </summary>
    internal interface IPerformDelayedClone
    {
        /// <summary>
        /// This method will get called right after the dialogue gets cloned.
        /// </summary>
        /// <param name="originalDialogue">This is the dialogue the cloned dialogue had cloned from.</param>
        void DelayedClone(Dialogue originalDialogue);
    }
}