using System.Collections.Generic;

namespace Trivial.Graph.Domain.Models.Base;

public interface ILinkable
{
    public IReadOnlyList<BaseLinkModel> Links { get; }

    public bool CanAttachTo(ILinkable Other);

    internal void AddLink(BaseLinkModel Link);

    internal void RemoveLink(BaseLinkModel Link);
}