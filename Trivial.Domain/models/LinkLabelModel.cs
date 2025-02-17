using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Models;

public class LinkLabelModel : Model
{
    public LinkLabelModel(BaseLinkModel parent, string id, string content, float? distance = null, Vector2? offset = null) : base(id)
    {
        Parent = parent;
        Content = content;
        Distance = distance;
        Offset = offset;
    }

    public LinkLabelModel(BaseLinkModel parent, string content, float? distance = null, Vector2? offset = null)
    {
        Parent = parent;
        Content = content;
        Distance = distance;
        Offset = offset;
    }

    public BaseLinkModel Parent { get; }
    public string Content { get; set; }
    /// <summary>
    /// 3 types of values are possible:
    /// <para>- A number between 0 and 1: Position relative to the link's length</para>
    /// <para>- A positive number, greater than 1: Position away from the start</para>
    /// <para>- A negative number, less than 0: Position away from the end</para>
    /// </summary>
    public float? Distance { get; set; }
    public Vector2? Offset { get; set; }
}
