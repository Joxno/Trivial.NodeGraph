using Trivial.Domain;
using Trivial.Domain.Anchors;
using System.Collections.Generic;
using System.Linq;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Extensions;

namespace Trivial.Graph.Algorithms;

public static class LinksReconnectionAlgorithms
{
    public static void ReconnectLinksToClosestPorts(this Diagram Diagram)
    {
        // Only refresh ports once
        var t_ModelsToRefresh = new HashSet<Model>();

        foreach (var t_Link in Diagram.Links.ToArray())
        {
            if (t_Link.Source is not SinglePortAnchor t_Spa1 || t_Link.Target is not SinglePortAnchor t_Spa2)
                continue;

            var t_SourcePorts = t_Spa1.Port.Parent.Ports;
            var t_TargetPorts = t_Spa2.Port.Parent.Ports;

            // Find the ports with minimal distance
            var t_MinDistance = float.MaxValue;
            var t_MinSourcePort = t_Spa1.Port;
            var t_MinTargetPort = t_Spa2.Port;
            foreach (var t_SourcePort in t_SourcePorts)
            {
                foreach (var t_TargetPort in t_TargetPorts)
                {
                    var t_Distance = t_SourcePort.Position.DistanceTo(t_TargetPort.Position);
                    if (t_Distance < t_MinDistance)
                    {
                        t_MinDistance = t_Distance;
                        t_MinSourcePort = t_SourcePort;
                        t_MinTargetPort = t_TargetPort;
                    }
                }
            }

            // Reconnect
            if (t_Spa1.Port != t_MinSourcePort)
            {
                t_ModelsToRefresh.Add(t_Spa1.Port);
                t_ModelsToRefresh.Add(t_MinSourcePort);
                t_Link.SetSource(new SinglePortAnchor(t_MinSourcePort));
                t_ModelsToRefresh.Add(t_Link);
            }

            if (t_Spa2.Port != t_MinTargetPort)
            {
                t_ModelsToRefresh.Add(t_Spa2.Port);
                t_ModelsToRefresh.Add(t_MinTargetPort);
                t_Link.SetTarget(new SinglePortAnchor(t_MinTargetPort));
                t_ModelsToRefresh.Add(t_Link);
            }
        }

        foreach (var t_Model in t_ModelsToRefresh)
        {
            t_Model.Refresh();
        }
    }
}
