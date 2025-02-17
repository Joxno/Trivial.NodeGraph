using Trivial.Graph.Domain.Anchors;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain;

public delegate BaseLinkModel? LinkFactory(Diagram Diagram, ILinkable Source, Anchor TargetAnchor);

public delegate Anchor AnchorFactory(Diagram Diagram, BaseLinkModel Link, ILinkable Model);

public delegate GroupModel GroupFactory(Diagram Diagram, NodeModel[] Children);
