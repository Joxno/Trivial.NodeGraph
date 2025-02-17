using System.Collections.Generic;
using Trivial.Domain.Models;

namespace Trivial.Graph.Models;

public class SvgGroupModel : GroupModel
{
    public SvgGroupModel(IEnumerable<NodeModel> Children, byte Padding = 30, bool AutoSize = true) : base(Children, Padding, AutoSize)
    {
    }
}