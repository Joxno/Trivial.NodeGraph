using Trivial.Domain.Models;
using System.Linq;

namespace Trivial.Domain.Layers;

public class GroupLayer : BaseLayer<GroupModel>
{
    public GroupLayer(Diagram Diagram) : base(Diagram)
    {
    }

    public GroupModel Group(params NodeModel[] Children)
    {
        return Add(Diagram.Options.Groups.Factory(Diagram, Children));
    }

    /// <summary>
    /// Removes the group AND its children
    /// </summary>
    public void Delete(GroupModel Group)
    {
        Diagram.Batch(() =>
        {
            var t_Children = Group.Children.ToArray();

            Remove(Group);

            foreach (var t_Child in t_Children)
            {
                if (t_Child is GroupModel t_G)
                {
                    Delete(t_G);
                }
                else
                {
                    Diagram.Nodes.Remove(t_Child);
                }
            }
        });
    }

    protected override void OnItemRemoved(GroupModel Group)
    {
        Diagram.Links.Remove(Group.PortLinks.ToArray());
        Diagram.Links.Remove(Group.Links.ToArray());
        Group.Ungroup();
        Group.Group?.RemoveChild(Group);
    }
}
