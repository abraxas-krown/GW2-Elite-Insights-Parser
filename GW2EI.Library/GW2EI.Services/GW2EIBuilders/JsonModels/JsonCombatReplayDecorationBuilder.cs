using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels;

/// <summary>
/// Writes the combat replay's mechanic and skill drawings into <see cref="JsonCombatReplayMetaData"/>.
/// The JSON objects mirror the combat replay descriptions the HTML report is made from, field for field.
/// </summary>
internal static class JsonCombatReplayDecorationBuilder
{
    public static void AddDecorations(JsonCombatReplayMetaData jsonCR, ParsedEvtcLog log, RawFormatSettings settings,
        IReadOnlyList<SingleActorCombatReplayDescription> actors,
        IReadOnlyList<CombatReplayRenderingDescription> decorationRendering,
        IReadOnlyList<CombatReplayMetadataDescription> decorationMetadata)
    {
        var decorations = new List<JsonCombatReplayDecoration>(decorationRendering.Count);
        var usedSignatures = new HashSet<string>();
        var referencedIDs = new HashSet<long>();
        foreach (CombatReplayRenderingDescription rendering in decorationRendering)
        {
            // Actor facings, backgrounds and arena maps are not mechanics; facings and maps are written elsewhere
            if (!rendering.IsMechanicOrSkill || rendering is not DecorationRenderingDescription description)
            {
                continue;
            }
            JsonCombatReplayDecoration? decoration = BuildDecoration(description);
            if (decoration == null)
            {
                continue;
            }
            decorations.Add(decoration);
            usedSignatures.Add(description.MetadataSignature);
            AddReferencedIDs(referencedIDs, decoration.ConnectedTo);
            AddReferencedIDs(referencedIDs, decoration.ConnectedFrom);
            AddReferencedIDs(referencedIDs, decoration.RotationConnectedTo);
        }
        jsonCR.Decorations = decorations;

        var metadata = new List<JsonCombatReplayDecorationMetadata>(usedSignatures.Count);
        foreach (CombatReplayMetadataDescription description in decorationMetadata)
        {
            if (description is DecorationMetadataDescription decorationDescription && usedSignatures.Remove(decorationDescription.Signature))
            {
                metadata.Add(BuildMetadata(decorationDescription));
            }
        }
        jsonCR.DecorationMetadata = metadata;

        jsonCR.Actors = BuildActors(log, settings, actors, referencedIDs);
    }

    private static void AddReferencedIDs(HashSet<long> ids, JsonCombatReplayConnector? connector)
    {
        if (connector?.MasterID != null)
        {
            ids.Add(connector.MasterID.Value);
        }
        if (connector?.DstMasterID != null)
        {
            ids.Add(connector.DstMasterID.Value);
        }
    }

    /// <summary>
    /// The actors decorations are attached to that the JSON does not list as players or targets
    /// </summary>
    private static List<JsonCombatReplayActor> BuildActors(ParsedEvtcLog log, RawFormatSettings settings,
        IReadOnlyList<SingleActorCombatReplayDescription> actors, HashSet<long> referencedIDs)
    {
        var listedIDs = new HashSet<long>(log.Friendlies.Select(x => (long)x.UniqueID));
        listedIDs.UnionWith(log.LogData.Logic.Targets.Select(x => (long)x.UniqueID));
        var agentsByID = new Dictionary<long, AgentItem>();
        foreach (AgentItem agent in log.AgentData.AllAgents)
        {
            agentsByID.TryAdd(agent.UniqueID, agent);
        }
        var result = new List<JsonCombatReplayActor>();
        foreach (SingleActorCombatReplayDescription description in actors)
        {
            // Decorations refer to the englobing agent of an englobed one
            bool referenced = referencedIDs.Contains(description.ID) || (description.ParentID >= 0 && referencedIDs.Contains(description.ParentID));
            if (!referenced || listedIDs.Contains(description.ID) || !agentsByID.TryGetValue(description.ID, out AgentItem? agent))
            {
                continue;
            }
            SingleActor actor = log.FindActor(agent);
            result.Add(new JsonCombatReplayActor()
            {
                UniqueID = description.ID,
                ParentID = description.ParentID >= 0 ? description.ParentID : null,
                Id = actor.ID,
                Name = actor.Character,
                CombatReplayData = JsonActorCombatReplayDataBuilder.BuildJsonActorCombatReplayDataBuilder(actor, log, settings),
            });
        }
        return result;
    }

    private static JsonCombatReplayDecoration? BuildDecoration(DecorationRenderingDescription description)
    {
        var decoration = new JsonCombatReplayDecoration()
        {
            Type = (int)description.Type,
            MetadataSignature = description.MetadataSignature,
            Start = description.Start,
            End = description.End,
        };
        switch (description)
        {
            case AttachedDecorationRenderingDescription attached:
                decoration.ConnectedTo = BuildConnector(attached.ConnectedTo);
                decoration.RotationConnectedTo = BuildConnector(attached.RotationConnectedTo);
                decoration.SkillMode = BuildSkillMode(attached.SkillMode);
                break;
            case TextDecorationRenderingDescription text:
                decoration.ConnectedTo = BuildConnector(text.ConnectedTo);
                decoration.RotationConnectedTo = BuildConnector(text.RotationConnectedTo);
                decoration.SkillMode = BuildSkillMode(text.SkillMode);
                decoration.Text = text.Text;
                decoration.Bold = text.Bold ? true : null;
                decoration.FontType = text.FontType;
                decoration.FontSize = text.FontSize;
                break;
            default:
                return null;
        }
        // A decoration that can not be placed can not be drawn
        if (decoration.ConnectedTo == null)
        {
            return null;
        }
        if (description is FormDecorationRenderingDescription form)
        {
            decoration.Fill = form.Fill ? true : null;
            decoration.GrowingEnd = form.GrowingEnd > form.Start ? form.GrowingEnd : null;
            decoration.GrowingReverse = form.GrowingReverse ? true : null;
        }
        switch (description)
        {
            case LineDecorationRenderingDescription line:
                decoration.ConnectedFrom = BuildConnector(line.ConnectedFrom);
                if (decoration.ConnectedFrom == null)
                {
                    return null;
                }
                break;
            case CustomPolygonDecorationRenderingDescription polygon:
                decoration.Points = polygon.Points;
                break;
            case ProgressBarDecorationRenderingDescription progressBar:
                decoration.Progress = progressBar.Progress;
                decoration.InterpolationMethod = progressBar.InterpolationMethod;
                break;
            case IconDecorationRenderingDescription icon:
                decoration.Text = icon.Text;
                break;
        }
        return decoration;
    }

    private static JsonCombatReplaySkillMode? BuildSkillMode(SkillModeDescription? description)
    {
        if (description == null)
        {
            return null;
        }
        return new JsonCombatReplaySkillMode()
        {
            OwnerID = (description.Owner as SkillConnectorDescription)?.OwnerID,
            Category = description.Category,
            SkillID = description.SkillID,
            IsBuff = description.IsBuff ? true : null,
        };
    }

    private static JsonCombatReplayConnector? BuildConnector(ConnectorDescription? description)
    {
        var connector = new JsonCombatReplayConnector();
        switch (description)
        {
            case null:
                return null;
            // Positions; subclasses before their base classes
            case PositionToAgentConnectorDescription toAgent:
                connector.MasterID = toAgent.MasterID;
                connector.Position = toAgent.Position;
                connector.Velocity = toAgent.Velocity;
                break;
            case AgentConnectorDescription agent:
                connector.MasterID = agent.MasterID;
                break;
            case PositionConnectorDescription position:
                connector.Position = position.Position;
                break;
            case InterpolationConnectorDescription interpolation:
                connector.Positions = interpolation.Positions;
                connector.InterpolationMethod = interpolation.InterpolationMethod;
                break;
            case ScreenSpaceConnectorDescription screen:
                connector.Position = screen.Position;
                connector.IsScreenSpace = true;
                break;
            // Rotations
            case AgentFacingAgentConnectorDescription facingAgent:
                connector.MasterID = facingAgent.MasterID;
                connector.DstMasterID = facingAgent.DstMasterID;
                connector.RotationOffset = facingAgent.RotationOffset;
                connector.RotationOffsetMode = facingAgent.RotationOffsetMode;
                break;
            case AgentFacingConnectorDescription facing:
                connector.MasterID = facing.MasterID;
                connector.RotationOffset = facing.RotationOffset;
                connector.RotationOffsetMode = facing.RotationOffsetMode;
                break;
            case SpinningConnectorDescription spinning:
                connector.Angle = spinning.Angle;
                connector.SpinAngle = spinning.SpinAngle;
                break;
            case AngleConnectorDescription angle:
                connector.Angle = angle.Angle;
                break;
            case AngleInterpolationConnectorDescription angles:
                connector.Angles = angles.Angles;
                connector.InterpolationMethod = angles.InterpolationMethod;
                break;
            default:
                return null;
        }
        if (description is GeographicalConnectorDescription geographical && geographical.Offset != null)
        {
            connector.Offset = geographical.Offset;
            connector.OffsetAfterRotation = geographical.OffsetAfterRotation ? true : null;
        }
        return connector;
    }

    private static JsonCombatReplayDecorationMetadata BuildMetadata(DecorationMetadataDescription description)
    {
        var metadata = new JsonCombatReplayDecorationMetadata()
        {
            Type = (int)description.Type,
            Signature = description.Signature,
        };
        switch (description)
        {
            case FormDecorationMetadataDescription form:
                metadata.Color = form.Color;
                break;
            case TextDecorationMetadataDescription text:
                metadata.Color = text.Color;
                metadata.BackgroundColor = text.BackgroundColor;
                break;
        }
        switch (description)
        {
            // Subclasses before their base classes
            case PieDecorationMetadataDescription pie:
                metadata.Radius = pie.Radius;
                metadata.MinRadius = pie.MinRadius > 0 ? pie.MinRadius : null;
                metadata.OpeningAngle = pie.OpeningAngle;
                break;
            case CircleDecorationMetadataDescription circle:
                metadata.Radius = circle.Radius;
                metadata.MinRadius = circle.MinRadius > 0 ? circle.MinRadius : null;
                break;
            case DoughnutDecorationMetadataDescription doughnut:
                metadata.InnerRadius = doughnut.InnerRadius;
                metadata.OuterRadius = doughnut.OuterRadius;
                break;
            case LineDecorationMetadataDescription line:
                metadata.Thickness = line.Thickness;
                metadata.WorldSizeThickness = line.WorldSizeThickness ? true : null;
                break;
            case OverheadProgressBarDecorationMetadataDescription overheadProgressBar:
                metadata.Width = overheadProgressBar.Width;
                metadata.Height = overheadProgressBar.Height;
                metadata.SecondaryColor = overheadProgressBar.SecondaryColor;
                metadata.PixelWidth = overheadProgressBar.PixelWidth;
                metadata.PixelHeight = overheadProgressBar.PixelHeight;
                break;
            case ProgressBarDecorationMetadataDescription progressBar:
                metadata.Width = progressBar.Width;
                metadata.Height = progressBar.Height;
                metadata.SecondaryColor = progressBar.SecondaryColor;
                break;
            case RectangleDecorationMetadataDescription rectangle:
                metadata.Width = rectangle.Width;
                metadata.Height = rectangle.Height;
                break;
            case RegularPolygonDecorationMetadataDescription polygon:
                metadata.Radius = polygon.Radius;
                metadata.NbPolygon = polygon.NbPolygon;
                break;
            case IconDecorationMetadataDescription icon:
                metadata.Image = icon.Image;
                metadata.PixelSize = icon.PixelSize;
                metadata.WorldSize = icon.WorldSize;
                metadata.Opacity = icon.Opacity;
                break;
        }
        return metadata;
    }
}
