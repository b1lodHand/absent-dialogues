namespace com.absence.dialoguesystem
{
    /// <summary>
    /// Any game object with a script that implements this interface attached will display it's dialogue when gets selected.
    /// </summary>
    public interface IUseDialogueInScene
    {
        /// <summary>
        /// The dialogue cloned and in-use.
        /// </summary>
        Dialogue ClonedDialogue { get; }

        /// <summary>
        /// The original dialogue provided for the script (not the cloned one).
        /// </summary>
        Dialogue ReferencedDialogue { get; }
    }
}