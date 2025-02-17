using Trivial.Domain.Anchors;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain;

public delegate BaseLinkModel? LinkFactory(Diagram Diagram, ILinkable Source, Anchor TargetAnchor);

public delegate Anchor AnchorFactory(Diagram Diagram, BaseLinkModel Link, ILinkable Model);

public delegate GroupModel GroupFactory(Diagram Diagram, NodeModel[] Children);
