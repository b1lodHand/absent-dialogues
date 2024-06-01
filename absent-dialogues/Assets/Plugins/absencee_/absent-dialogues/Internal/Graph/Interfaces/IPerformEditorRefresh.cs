namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Any node subtype with this interface implemented will get a callback when you change any value in the inspector.
    /// </summary>
    public interface IPerformEditorRefresh
    {
        /// <summary>
        /// Use to declare what to do when any value gets changed in the inspector/
        /// </summary>
        void PerformEditorRefresh();
    }
}