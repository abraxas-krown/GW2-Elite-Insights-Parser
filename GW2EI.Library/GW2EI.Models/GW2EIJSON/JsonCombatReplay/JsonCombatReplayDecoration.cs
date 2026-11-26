namespace GW2EIJSON;
/// <summary>
/// A mechanic or skill drawing of the combat replay: an area, a cone, a tether, an icon over an actor, etc.
/// Its look (shape, size, colour, image) is in the <see cref="JsonCombatReplayDecorationMetadata"/> whose <see cref="JsonCombatReplayDecorationMetadata.Signature"/> is <see cref="MetadataSignature"/>.
/// Fields that do not apply to the decoration's <see cref="Type"/> are not written.
/// </summary>
/// <seealso cref="JsonCombatReplayMetaData.Decorations"/>
public class JsonCombatReplayDecoration
{
    /// <summary>
    /// Kind of the decoration:
    /// 1: BackgroundIcon (an image on an actor that is a part of the arena, such as a platform, see <see cref="Opacities"/>),
    /// 2: Circle, 3: Doughnut, 6: Icon, 7: IconOverhead, 8: Line,
    /// 10: MovingPlatform (a part of the arena that moves, see <see cref="Positions"/>),
    /// 11: Pie, 13: ProgressBar, 14: ProgressBarOverhead, 15: Rectangle, 16: SquadMarker, 17: SquadMarkerOverhead,
    /// 20: Text, 21: RegularPolygon, 22: TextOverhead, 24: CustomPolygon. 
    /// </summary>
    public int Type;
    /// <summary>
    /// <see cref="JsonCombatReplayDecorationMetadata.Signature"/> of the decoration's metadata
    /// </summary>
    public string? MetadataSignature;
    /// <summary>
    /// Time at which the decoration appears, in ms
    /// </summary>
    public long Start;
    /// <summary>
    /// Time at which the decoration disappears, in ms
    /// </summary>
    public long End;
    /// <summary>
    /// Where the decoration is drawn
    /// </summary>
    public JsonCombatReplayConnector? ConnectedTo;
    /// <summary>
    /// Lines only: where the line starts; it ends at <see cref="ConnectedTo"/>
    /// </summary>
    public JsonCombatReplayConnector? ConnectedFrom;
    /// <summary>
    /// How the decoration is rotated.
    /// Not written for decorations that are not rotated.
    /// </summary>
    public JsonCombatReplayConnector? RotationConnectedTo;
    /// <summary>
    /// Present when the decoration shows a skill of an actor rather than a mechanic of the encounter
    /// </summary>
    public JsonCombatReplaySkillMode? SkillMode;
    /// <summary>
    /// Shapes: true when the shape is filled, absent when only its outline is drawn
    /// </summary>
    public bool? Fill;
    /// <summary>
    /// Shapes: the shape grows from <see cref="Start"/> until this time, in ms.
    /// Absent when the shape does not grow.
    /// </summary>
    public long? GrowingEnd;
    /// <summary>
    /// Shapes: true when the shape shrinks instead of growing
    /// </summary>
    public bool? GrowingReverse;
    /// <summary>
    /// Custom polygons: the points of the polygon, in inches, relative to the decoration's position
    /// </summary>
    public IReadOnlyList<IReadOnlyList<float>>? Points;
    /// <summary>
    /// Moving platforms: where the platform is over time, as [x, y, z, angle, opacity, time] keyframes ordered by time.
    /// x and y are in pixels on the combat replay map, z is the height (lower values are higher up), angle is in radians
    /// (clockwise on the map, as y points down) and opacity is from 0 to 1. Values are linear between two keyframes; the
    /// platform is not shown before the first keyframe and stays as the last one after it.
    /// Moving platforms have no <see cref="ConnectedTo"/>.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? Positions;
    /// <summary>
    /// Background icons: the opacity over time, as [opacity, time, opacity, time, ...] from 0 to 1.
    /// Each value holds until the next one, which it fades into linearly over the 1500 ms before it.
    /// </summary>
    public IReadOnlyList<float>? Opacities;
    /// <summary>
    /// Background icons: the height over time, as [height, time, height, time, ...] (lower values are higher up).
    /// Each value holds until the next one. It only orders the drawing.
    /// </summary>
    public IReadOnlyList<float>? Heights;
    /// <summary>
    /// Progress bars: [time in ms, percent] values
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? Progress;
    /// <summary>
    /// Progress bars: how to go from a <see cref="Progress"/> value to the next. 0: linear, 1: step
    /// </summary>
    public int? InterpolationMethod;
    /// <summary>
    /// Texts and icons: the text shown
    /// </summary>
    public string? Text;
    /// <summary>
    /// Texts: true when the text is bold
    /// </summary>
    public bool? Bold;
    /// <summary>
    /// Texts: the font of the text
    /// </summary>
    public string? FontType;
    /// <summary>
    /// Texts: the size of the text, in pixels
    /// </summary>
    public uint? FontSize;
}
/// <summary>
/// A position or a rotation of a <see cref="JsonCombatReplayDecoration"/>. Which fields are present tells what it is.
/// Positions:
/// - <see cref="MasterID"/> alone: the position of that actor
/// - <see cref="Position"/> (x, y): a fixed position
/// - <see cref="Positions"/>: positions over time
/// - <see cref="Position"/> (x, y, time), <see cref="MasterID"/> and <see cref="Velocity"/>: from that position, moves towards that actor
/// - <see cref="IsScreenSpace"/>: a position on the screen, in pixels, not on the map
/// Rotations:
/// - <see cref="Angle"/> alone: a fixed angle
/// - <see cref="Angle"/> and <see cref="SpinAngle"/>: spins from that angle
/// - <see cref="Angles"/>: angles over time
/// - <see cref="MasterID"/>: the facing of that actor, with <see cref="RotationOffset"/>
/// - <see cref="MasterID"/> and <see cref="DstMasterID"/>: facing from the first actor towards the second
/// Actor ids are the actors' <see cref="JsonActor.UniqueID"/>; actors that are neither players nor targets are listed in <see cref="JsonCombatReplayMetaData.Actors"/>.
/// Positions are in pixels, like <see cref="JsonActorCombatReplayData.Positions"/>; angles are in degrees, like <see cref="JsonActorCombatReplayData.Orientations"/>.
/// </summary>
public class JsonCombatReplayConnector
{
    /// <summary>
    /// <see cref="JsonActor.UniqueID"/> of the actor the decoration is attached to, or whose facing it follows
    /// </summary>
    public int? MasterID;
    /// <summary>
    /// <see cref="JsonActor.UniqueID"/> of the actor <see cref="MasterID"/> faces
    /// </summary>
    public int? DstMasterID;
    /// <summary>
    /// A fixed position: [x, y] in pixels.
    /// With <see cref="Velocity"/>: [x, y, time in ms], the position the decoration starts moving from, and when.
    /// </summary>
    public IReadOnlyList<float>? Position;
    /// <summary>
    /// Positions over time: x, y in pixels and time in ms, repeated
    /// </summary>
    public IReadOnlyList<float>? Positions;
    /// <summary>
    /// How to go from a value of <see cref="Positions"/> or <see cref="Angles"/> to the next. 0: linear, 1: step
    /// </summary>
    public int? InterpolationMethod;
    /// <summary>
    /// Speed towards <see cref="MasterID"/>, in inches per ms
    /// </summary>
    public float? Velocity;
    /// <summary>
    /// Offset from the position, [x, y] in inches
    /// </summary>
    public IReadOnlyList<float>? Offset;
    /// <summary>
    /// True when <see cref="Offset"/> is applied after the rotation, so that it turns with the decoration
    /// </summary>
    public bool? OffsetAfterRotation;
    /// <summary>
    /// True when the position is on the screen, in pixels, rather than on the map
    /// </summary>
    public bool? IsScreenSpace;
    /// <summary>
    /// A fixed angle, in degrees
    /// </summary>
    public float? Angle;
    /// <summary>
    /// Degrees the decoration turns by between its start and its end
    /// </summary>
    public float? SpinAngle;
    /// <summary>
    /// Angles over time: angle in degrees and time in ms, repeated
    /// </summary>
    public IReadOnlyList<float>? Angles;
    /// <summary>
    /// Degrees added to the facing of <see cref="MasterID"/>
    /// </summary>
    public float? RotationOffset;
    /// <summary>
    /// How <see cref="RotationOffset"/> is applied.
    /// 0: added to the facing, 1: replaces the facing, 2: added after the offset translation
    /// </summary>
    public int? RotationOffsetMode;
}
/// <summary>
/// The skill a <see cref="JsonCombatReplayDecoration"/> shows
/// </summary>
public class JsonCombatReplaySkillMode
{
    /// <summary>
    /// <see cref="JsonActor.UniqueID"/> of the actor whose skill it is
    /// </summary>
    public long? OwnerID;
    /// <summary>
    /// Bit flags of the skill's category:
    /// 1: show on select (always set), 2: important buffs, 4: projectile management, 8: heal, 16: cleanse,
    /// 32: strip, 64: portal, 128: crowd control
    /// </summary>
    public uint Category;
    /// <summary>
    /// ID of the skill, or of the buff when <see cref="IsBuff"/>
    /// </summary>
    /// <seealso cref="JsonLog.SkillMap"/>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long SkillID;
    /// <summary>
    /// True when <see cref="SkillID"/> is a buff
    /// </summary>
    public bool? IsBuff;
}
