using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System.Collections.Generic;
using System;
using Trivial.Domain.Models;
using Trivial.Domain.Anchors;
using System.Linq;
using System.Numerics;

namespace Trivial.Domain.Routers;

public class OrthogonalRouter : Router
{
    private readonly Router m_FallbackRouter;
    private float m_ShapeMargin;
    private float m_GlobalMargin;

    public OrthogonalRouter(float ShapeMargin = 10, float GlobalMargin = 50, Router? FallbackRouter = null)
    {
        m_ShapeMargin = ShapeMargin;
        m_GlobalMargin = GlobalMargin;
        m_FallbackRouter = FallbackRouter ?? new NormalRouter();
    }

    public override Vector2[] GetRoute(Diagram Diagram, BaseLinkModel Link)
    {
        if (!Link.IsAttached)
            return m_FallbackRouter.GetRoute(Diagram, Link);

        if (Link.Source is not SinglePortAnchor t_Spa1)
            return m_FallbackRouter.GetRoute(Diagram, Link);

        if (Link.Target is not SinglePortAnchor t_TargetAnchor)
            return m_FallbackRouter.GetRoute(Diagram, Link);

        var t_SourcePort = t_Spa1.Port;
        if (t_TargetAnchor == null || t_SourcePort.Parent.Size == null || t_TargetAnchor.Port.Parent.Size == null)
            return m_FallbackRouter.GetRoute(Diagram, Link);

        var t_TargetPort = t_TargetAnchor.Port;

        var t_ShapeMargin = m_ShapeMargin;
        var t_GlobalBoundsMargin = m_GlobalMargin;
        var t_Spots = new HashSet<Vector2>();
        var t_Verticals = new List<float>();
        var t_Horizontals = new List<float>();
        var t_SideA = t_SourcePort.Alignment;
        var t_SideAVertical = IsVerticalSide(t_SideA);
        var t_SideB = t_TargetPort.Alignment;
        var t_SideBVertical = IsVerticalSide(t_SideB);
        var t_OriginA = GetPortPositionBasedOnAlignment(t_SourcePort);
        var t_OriginB = GetPortPositionBasedOnAlignment(t_TargetPort);
        var t_ShapeA = t_SourcePort.Parent.GetBounds(IncludePorts: true)!;
        var t_ShapeB = t_TargetPort.Parent.GetBounds(IncludePorts: true)!;
        var t_InflatedA = t_ShapeA.Inflate(t_ShapeMargin, t_ShapeMargin);
        var t_InflatedB = t_ShapeB.Inflate(t_ShapeMargin, t_ShapeMargin);

        if (t_InflatedA.Intersects(t_InflatedB))
        {
            t_ShapeMargin = 0;
            t_InflatedA = t_ShapeA;
            t_InflatedB = t_ShapeB;
        }

        // Curated bounds to stick to
        var t_Bounds = t_InflatedA.Union(t_InflatedB).Inflate(t_GlobalBoundsMargin, t_GlobalBoundsMargin);

        // Add edges to rulers
        t_Verticals.Add(t_InflatedA.Left);
        t_Verticals.Add(t_InflatedA.Right);
        t_Horizontals.Add(t_InflatedA.Top);
        t_Horizontals.Add(t_InflatedA.Bottom);
        t_Verticals.Add(t_InflatedB.Left);
        t_Verticals.Add(t_InflatedB.Right);
        t_Horizontals.Add(t_InflatedB.Top);
        t_Horizontals.Add(t_InflatedB.Bottom);

        // Rulers at origins of shapes
        (t_SideAVertical ? t_Verticals : t_Horizontals).Add(t_SideAVertical ? t_OriginA.X : t_OriginA.Y);
        (t_SideBVertical ? t_Verticals : t_Horizontals).Add(t_SideBVertical ? t_OriginB.X : t_OriginB.Y);

        // Points of shape antennas
        t_Spots.Add(GetOriginSpot(t_OriginA, t_SideA, t_ShapeMargin));
        t_Spots.Add(GetOriginSpot(t_OriginB, t_SideB, t_ShapeMargin));

        // Sort rulers
        t_Verticals.Sort();
        t_Horizontals.Sort();

        // Create grid
        var t_Grid = RulersToGrid(t_Verticals, t_Horizontals, t_Bounds);
        var t_GridPoints = GridToSpots(t_Grid, new[] { t_InflatedA, t_InflatedB });

        // Add to spots
        t_Spots.UnionWith(t_GridPoints);

        var t_Ys = t_Spots.Select(P => P.Y).Distinct().ToList();
        var t_Xs = t_Spots.Select(P => P.X).Distinct().ToList();
        t_Ys.Sort();
        t_Xs.Sort();

        var t_Nodes = t_Spots.ToDictionary(P => P, P => new Node(P));

        for (var t_I = 0; t_I < t_Ys.Count; t_I++)
        {
            for (var t_J = 0; t_J < t_Xs.Count; t_J++)
            {
                var t_B = new Vector2(t_Xs[t_J], t_Ys[t_I]);
                if (!t_Nodes.ContainsKey(t_B))
                    continue;

                if (t_J > 0)
                {
                    var t_A = new Vector2(t_Xs[t_J - 1], t_Ys[t_I]);

                    if (t_Nodes.ContainsKey(t_A))
                    {
                        t_Nodes[t_A].ConnectedTo.Add(t_Nodes[t_B]);
                        t_Nodes[t_B].ConnectedTo.Add(t_Nodes[t_A]);
                    }
                }

                if (t_I > 0)
                {
                    var t_A = new Vector2(t_Xs[t_J], t_Ys[t_I - 1]);

                    if (t_Nodes.ContainsKey(t_A))
                    {
                        t_Nodes[t_A].ConnectedTo.Add(t_Nodes[t_B]);
                        t_Nodes[t_B].ConnectedTo.Add(t_Nodes[t_A]);
                    }
                }
            }
        }

        var t_NodeA = t_Nodes[GetOriginSpot(t_OriginA, t_SideA, t_ShapeMargin)];
        var t_NodeB = t_Nodes[GetOriginSpot(t_OriginB, t_SideB, t_ShapeMargin)];
        var t_Path = AStarPathfinder.GetPath(t_NodeA, t_NodeB);

        if (t_Path.Count > 0)
        {
            return t_Path.ToArray();
        }
        else
        {
            return m_FallbackRouter.GetRoute(Diagram, Link);
        }
    }

    private static Grid RulersToGrid(List<float> Verticals, List<float> Horizontals, Rectangle Bounds)
    {
        var t_Result = new Grid();
        Verticals.Sort();
        Horizontals.Sort();

        var t_LastX = Bounds.Left;
        var t_LastY = Bounds.Top;
        var t_Column = 0;
        var t_Row = 0;

        foreach (var t_Y in Horizontals)
        {
            foreach (var t_X in Verticals)
            {
                t_Result.Set(t_Row, t_Column++, new Rectangle(t_LastX, t_LastY, t_X, t_Y));
                t_LastX = t_X;
            }

            // Last cell of the row
            t_Result.Set(t_Row, t_Column, new Rectangle(t_LastX, t_LastY, Bounds.Right, t_Y));
            t_LastX = Bounds.Left;
            t_LastY = t_Y;
            t_Column = 0;
            t_Row++;
        }

        t_LastX = Bounds.Left;

        // Last fow of cells
        foreach (var t_X in Verticals)
        {
            t_Result.Set(t_Row, t_Column++, new Rectangle(t_LastX, t_LastY, t_X, Bounds.Bottom));
            t_LastX = t_X;
        }

        // Last cell of last row
        t_Result.Set(t_Row, t_Column, new Rectangle(t_LastX, t_LastY, Bounds.Right, Bounds.Bottom));
        return t_Result;
    }

    private static HashSet<Vector2> GridToSpots(Grid Grid, Rectangle[] Obstacles)
    {
        bool IsInsideObstacles(Vector2 P)
        {
            foreach (var t_Obstacle in Obstacles)
            {
                if (t_Obstacle.ContainsPoint(P))
                    return true;
            }

            return false;
        }

        void AddIfNotInsideObstacles(HashSet<Vector2> List, Vector2 P)
        {
            if (!IsInsideObstacles(P))
            {
                List.Add(P);
            }
        }

        var t_GridPoints = new HashSet<Vector2>();
        foreach (var (t_Row, t_Data) in Grid.Data)
        {
            var t_FirstRow = t_Row == 0;
            var t_LastRow = t_Row == Grid.Rows - 1;

            foreach (var (t_Col, t_R) in t_Data)
            {
                var t_FirstCol = t_Col == 0;
                var t_LastCol = t_Col == Grid.Columns - 1;
                var t_Nw = t_FirstCol && t_FirstRow;
                var t_Ne = t_FirstRow && t_LastCol;
                var t_Se = t_LastRow && t_LastCol;
                var t_Sw = t_LastRow && t_FirstCol;

                if (t_Nw || t_Ne || t_Se || t_Sw)
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthEast);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthEast);
                }
                else if (t_FirstRow)
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.North);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthEast);
                }
                else if (t_LastRow)
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthEast);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.South);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthWest);
                }
                else if (t_FirstCol)
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.West);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthWest);
                }
                else if (t_LastCol)
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthEast);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.East);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthEast);
                }
                else
                {
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.North);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.NorthEast);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.East);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthEast);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.South);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.SouthWest);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.West);
                    AddIfNotInsideObstacles(t_GridPoints, t_R.Center);
                }
            }
        }

        // Reduce repeated points and filter out those who touch shapes
        return t_GridPoints;
    }

    private static bool IsVerticalSide(PortAlignment Alignment)
        => Alignment == PortAlignment.Top || Alignment == PortAlignment.Bottom;

    private static Vector2 GetOriginSpot(Vector2 P, PortAlignment Alignment, float ShapeMargin)
    {
        return Alignment switch
        {
            PortAlignment.Top => P + new Vector2(0, -ShapeMargin),
            PortAlignment.Right => P + new Vector2(ShapeMargin, 0),
            PortAlignment.Bottom => P + new Vector2(0, ShapeMargin),
            PortAlignment.Left => P + new Vector2(-ShapeMargin, 0),
            _ => throw new NotImplementedException()
        };
    }
}

class Grid
{
    public Grid()
    {
        Data = new Dictionary<float, Dictionary<float, Rectangle>>();
    }

    public Dictionary<float, Dictionary<float, Rectangle>> Data { get; }
    public float Rows { get; private set; }
    public float Columns { get; private set; }

    public void Set(float Row, float Column, Rectangle Rectangle)
    {
        Rows = MathF.Max(Rows, Row + 1);
        Columns = MathF.Max(Columns, Column + 1);

        if (!Data.ContainsKey(Row))
        {
            Data.Add(Row, new Dictionary<float, Rectangle>());
        }

        Data[Row].Add(Column, Rectangle);
    }
}

static class AStarPathfinder
{
    public static IReadOnlyList<Vector2> GetPath(Node Start, Node Goal)
    {
        var t_Frontier = new PriorityQueue<Node, float>();
        t_Frontier.Enqueue(Start, 0);

        while (t_Frontier.Count > 0)
        {
            var t_Current = t_Frontier.Dequeue();

            if (t_Current.Equals(Goal))
                break;

            foreach (var t_Next in t_Current.ConnectedTo)
            {
                var t_NewCost = t_Current.Cost + 1.0f;
                if (t_Current.Parent != null && IsChangeOfDirection(t_Current.Parent.Position, t_Current.Position, t_Next.Position))
                {
                    t_NewCost *= t_NewCost;
                    t_NewCost *= t_NewCost;
                }

                if (t_Next.Cost == 0 || t_NewCost < t_Next.Cost)
                {
                    t_Next.Cost = t_NewCost;
                    var t_Priority = t_NewCost + Heuristic(t_Next.Position, Goal.Position);
                    t_Frontier.Enqueue(t_Next, t_Priority);
                    t_Next.Parent = t_Current;
                }
            }
        }

        var t_Result = new List<Vector2>();
        var t_C = Goal.Parent;

        while (t_C != null && t_C != Start)
        {
            t_Result.Insert(0, t_C.Position);
            t_C = t_C.Parent;
        }

        if (t_C == Start)
        {
            t_Result = SimplifyPath(t_Result);

            // In case of paths with one bend
            if (t_Result.Count > 2)
            {
                if (AreOnSameLine(t_Result[^2], t_Result[^1], Goal.Position))
                {
                    t_Result.RemoveAt(t_Result.Count - 1);
                }

                if (AreOnSameLine(Start.Position, t_Result[0], t_Result[1]))
                {
                    t_Result.RemoveAt(0);
                }
            }

            return t_Result;
        }
        else
        {
            return Array.Empty<Vector2>();
        }
    }

    private static bool AreOnSameLine(Vector2 Prev, Vector2 Curr, Vector2 Next)
    {
        return (Prev.X == Curr.X && Curr.X == Next.X) || (Prev.Y == Curr.Y && Curr.Y == Next.Y);
    }

    private static List<Vector2> SimplifyPath(List<Vector2> Path)
    {
        for (var t_I = Path.Count - 2; t_I > 0; t_I--)
        {
            var t_Prev = Path[t_I + 1];
            var t_Curr = Path[t_I];
            var t_Next = Path[t_I - 1];

            if (AreOnSameLine(t_Prev, t_Curr, t_Next))
            {
                Path.RemoveAt(t_I);
            }
        }

        return Path;
    }

    private static bool IsChangeOfDirection(Vector2 A, Vector2 B, Vector2 C)
    {
        if (A.X == B.X && B.X != C.X)
            return true;

        if (A.Y == B.Y && B.Y != C.Y)
            return true;

        return false;
    }

    private static float Heuristic(Vector2 A, Vector2 B)
    {
        return MathF.Abs(A.X - B.X) + MathF.Abs(A.Y - B.Y);
    }
}

class Node
{
    public Node(Vector2 Position)
    {
        this.Position = Position;
        ConnectedTo = new List<Node>();
    }

    public Vector2 Position { get; }
    public List<Node> ConnectedTo { get; }

    public float Cost { get; internal set; }
    public Node? Parent { get; internal set; }

    public override bool Equals(object? Obj)
    {
        if (Obj is not Node t_Item)
            return false;

        return Position.Equals(t_Item.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
