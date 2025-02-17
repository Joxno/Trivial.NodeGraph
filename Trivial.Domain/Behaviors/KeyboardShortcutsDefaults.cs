using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using System.Linq;
using System.Threading.Tasks;

namespace Trivial.Domain.Behaviors;

public static class KeyboardShortcutsDefaults
{
    public static async ValueTask DeleteSelection(Diagram Diagram)
    {
        var t_WasSuspended = Diagram.SuspendRefresh;
        if (!t_WasSuspended) Diagram.SuspendRefresh = true;

        foreach (var t_Sm in Diagram.GetSelectedModels().ToArray())
        {
            if (t_Sm.Locked)
                continue;

            if (t_Sm is GroupModel t_Group && (await Diagram.Options.Constraints.ShouldDeleteGroup(t_Group)))
            {
                Diagram.Groups.Delete(t_Group);
            }
            else if (t_Sm is NodeModel t_Node && (await Diagram.Options.Constraints.ShouldDeleteNode(t_Node)))
            {
                Diagram.Nodes.Remove(t_Node);
            }
            else if (t_Sm is BaseLinkModel t_Link && (await Diagram.Options.Constraints.ShouldDeleteLink(t_Link)))
            {
                Diagram.Links.Remove(t_Link);
            }
        }

        if (!t_WasSuspended)
        {
            Diagram.SuspendRefresh = false;
            Diagram.Refresh();
        }
    }

    public static ValueTask Grouping(Diagram Diagram)
    {
        if (!Diagram.Options.Groups.Enabled)
            return ValueTask.CompletedTask;

        if (!Diagram.GetSelectedModels().Any())
            return ValueTask.CompletedTask;

        var t_SelectedNodes = Diagram.Nodes.Where(N => N.Selected).ToArray();
        var t_NodesWithGroup = t_SelectedNodes.Where(N => N.Group != null).ToArray();
        if (t_NodesWithGroup.Length > 0)
        {
            // Ungroup
            foreach (var t_Group in t_NodesWithGroup.GroupBy(N => N.Group!).Select(G => G.Key))
            {
                Diagram.Groups.Remove(t_Group);
            }
        }
        else
        {
            // Group
            if (t_SelectedNodes.Length < 2)
                return ValueTask.CompletedTask;

            if (t_SelectedNodes.Any(N => N.Group != null))
                return ValueTask.CompletedTask;

            Diagram.Groups.Group(t_SelectedNodes);
        }

        return ValueTask.CompletedTask;
    }
}
