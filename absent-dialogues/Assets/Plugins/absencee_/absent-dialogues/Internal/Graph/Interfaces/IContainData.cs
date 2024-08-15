using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Interface to use if any of your dialogue elements has a speech, has options or has <see cref="ExtraDialogueData"/>.
    /// </summary>
    public interface IContainData
    {
        string Text { get; set; }
        List<Option> Options { get; set; }
        ExtraDialogueData ExtraData { get; set; }
    }
}