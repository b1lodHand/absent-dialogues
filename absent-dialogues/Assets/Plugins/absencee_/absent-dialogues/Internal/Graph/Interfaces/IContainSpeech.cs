using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Interface to use if any of your dialogue elements has a speech, has options or has <see cref="AdditionalSpeechData"/>.
    /// </summary>
    public interface IContainSpeech
    {
        string Speech { get; set; }
        List<Option> Options { get; set; }
        AdditionalSpeechData GetAdditionalSpeechData();
    }
}