using Trivial.Domain.Anchors;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain;

public delegate BaseLinkModel? LinkFactory(Diagram diagram, ILinkable source, Anchor targetAnchor);

public delegate Anchor AnchorFactory(Diagram diagram, BaseLinkModel link, ILinkable model);

public delegate GroupModel GroupFactory(Diagram diagram, NodeModel[] children);
