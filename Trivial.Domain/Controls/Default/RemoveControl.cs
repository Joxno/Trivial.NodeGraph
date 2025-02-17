using System.Numerics;
using System.Threading.Tasks;
using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Positions;

namespace Trivial.Graph.Domain.Controls.Default;

public class RemoveControl : ExecutableControl
{
    private readonly IPositionProvider m_PositionProvider;

    public RemoveControl(float X, float Y, float OffsetX = 0, float OffsetY = 0)
        : this(new BoundsBasedPositionProvider(X, Y, OffsetX, OffsetY))
    {
    }

    public RemoveControl(IPositionProvider PositionProvider)
    {
        m_PositionProvider = PositionProvider;
    }

    public override Vector2? GetPosition(Model Model) => m_PositionProvider.GetPosition(Model);

    public override async ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs _)
    {
        if (await ShouldDeleteModel(Diagram, Model))
        {
            DeleteModel(Diagram, Model);
        }
    }

    private static void DeleteModel(Diagram Diagram, Model Model)
    {
       switch (Model)
        {
            case GroupModel t_Group:
                Diagram.Groups.Delete(t_Group);
                return;
            case NodeModel t_Node:
                Diagram.Nodes.Remove(t_Node);
                return;

            case BaseLinkModel t_Link:
                Diagram.Links.Remove(t_Link);
                return;
        }
    }

    private static async ValueTask<bool> ShouldDeleteModel(Diagram Diagram, Model Model)
    {
        if (Model.Locked)
        {
            return false;
        }

        return Model switch
        {
            GroupModel t_Group => await Diagram.Options.Constraints.ShouldDeleteGroup.Invoke(t_Group),
            NodeModel t_Node => await Diagram.Options.Constraints.ShouldDeleteNode.Invoke(t_Node),
            BaseLinkModel t_Link => await Diagram.Options.Constraints.ShouldDeleteLink.Invoke(t_Link),
            _ => false,
        };
    }
}