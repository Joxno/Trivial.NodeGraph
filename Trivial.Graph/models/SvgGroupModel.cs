using System.Collections.Generic;
using Trivial.Domain.Models;

namespace Trivial.Graph.Models;

public class SvgGroupModel : GroupModel
{
    public SvgGroupModel(IEnumerable<NodeModel> children, byte padding = 30, bool autoSize = true) : base(children, padding, autoSize)
    {
    }
}