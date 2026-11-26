namespace GW2EIJSON;

/// <summary>
/// The look of <see cref="JsonCombatReplayDecoration"/>s: shared by every decoration whose <see cref="JsonCombatReplayDecoration.MetadataSignature"/> is <see cref="Signature"/>. \n
/// Fields that do not apply to the <see cref="Type"/> are not written. Sizes are in inches unless stated otherwise.
/// </summary>
/// <seealso cref="JsonCombatReplayMetaData.DecorationMetadata"/>
public class JsonCombatReplayDecorationMetadata
{
    /// <summary>
    /// Kind of the decoration, see <see cref="JsonCombatReplayDecoration.Type"/>
    /// </summary>
    public int Type;
    /// <summary>
    /// Identifies the metadata
    /// </summary>
    public string? Signature;

    /// <summary>
    /// Shapes and texts: colour, as a CSS colour (e.g. "rgba(255, 0, 0, 0.5)")
    /// </summary>
    public string? Color;
    /// <summary>
    /// Progress bars: colour of the part not yet done
    /// </summary>
    public string? SecondaryColor;
    /// <summary>
    /// Texts: background colour
    /// </summary>
    public string? BackgroundColor;

    /// <summary>
    /// Circles, pies and regular polygons: radius
    /// </summary>
    public uint? Radius;
    /// <summary>
    /// Circles and pies: radius a growing shape starts from
    /// </summary>
    public uint? MinRadius;
    /// <summary>
    /// Pies: opening angle, in degrees
    /// </summary>
    public float? OpeningAngle;
    /// <summary>
    /// Doughnuts: inner radius
    /// </summary>
    public uint? InnerRadius;
    /// <summary>
    /// Doughnuts: outer radius
    /// </summary>
    public uint? OuterRadius;
    /// <summary>
    /// Rectangles, progress bars and moving platforms: width
    /// </summary>
    public uint? Width;
    /// <summary>
    /// Rectangles, progress bars and moving platforms: height
    /// </summary>
    public uint? Height;
    /// <summary>
    /// Regular polygons: number of sides
    /// </summary>
    public uint? NbPolygon;
    /// <summary>
    /// Lines: thickness, in pixels on the screen, or in inches when <see cref="WorldSizeThickness"/>
    /// </summary>
    public uint? Thickness;
    /// <summary>
    /// Lines: true when <see cref="Thickness"/> is in inches
    /// </summary>
    public bool? WorldSizeThickness;
    /// <summary>
    /// Overhead progress bars: width, in pixels on the screen
    /// </summary>
    public uint? PixelWidth;
    /// <summary>
    /// Overhead progress bars: height, in pixels on the screen
    /// </summary>
    public uint? PixelHeight;

    /// <summary>
    /// Icons, background icons and moving platforms: url of the image
    /// </summary>
    public string? Image;
    /// <summary>
    /// Icons and background icons: size, in pixels on the screen; 0 when <see cref="WorldSize"/> is used
    /// </summary>
    public uint? PixelSize;
    /// <summary>
    /// Icons and background icons: size, in inches; 0 when <see cref="PixelSize"/> is used
    /// </summary>
    public uint? WorldSize;
    /// <summary>
    /// Icons: opacity, from 0 to 1
    /// </summary>
    public float? Opacity;
}

/// <summary>
/// An actor that <see cref="JsonCombatReplayDecoration"/>s are attached to but that is neither a player nor a target of the log
/// </summary>
/// <seealso cref="JsonCombatReplayMetaData.Actors"/>
public class JsonCombatReplayActor
{
    /// <summary>
    /// Unique ID of the actor, the one decorations refer to
    /// </summary>
    /// <seealso cref="JsonCombatReplayConnector.MasterID"/>
    public int UniqueID;
    /// <summary>
    /// When the actor is one of several agents that make up the same entity over time: the unique ID of that entity.
    /// Decorations refer to the entity's ID rather than to <see cref="UniqueID"/>.
    /// </summary>
    public int? ParentID;
    /// <summary>
    /// Game ID of the actor (species ID)
    /// </summary>
    public int Id;
    /// <summary>
    /// Name of the actor
    /// </summary>
    public string? Name;
    /// <summary>
    /// When the actor is a minion: the unique ID of its master
    /// </summary>
    public int? MasterID;
    /// <summary>
    /// When the actor is a known minion of a player: true.
    /// EI's combat replay shows such minions with their master, and only draws the decorations attached to them while it shows them.
    /// </summary>
    public bool? MasterIsPlayer;
    /// <summary>
    /// Combat replay data of the actor
    /// </summary>
    public JsonActorCombatReplayData? CombatReplayData;
}
