namespace GW2EIBuilders;

public class RawFormatSettings
{
    public bool RawFormatTimelineArrays { get; init; } = true;
    /// <summary>
    /// Adds the combat replay's mechanic and skill drawings to <see cref="GW2EIJSON.JsonCombatReplayMetaData"/>
    /// </summary>
    public bool RawFormatCombatReplayDecorations { get; init; } = false;

    public RawFormatSettings()
    {

    }
}
