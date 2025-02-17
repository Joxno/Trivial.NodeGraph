using Trivial.Domain.Models.Base;
using Trivial.Graph.Models;

namespace Trivial.Graph.Extensions;

public static class ModelExtensions
{
    public static bool IsSvg(this Model Model)
    {
        return Model is SvgNodeModel or SvgGroupModel or BaseLinkModel;
    }
}