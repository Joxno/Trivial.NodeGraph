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
    private readonly Router _fallbackRouter;
    private float _shapeMargin;
    private float _globalMargin;

    public OrthogonalRouter(float shapeMargin = 10, float globalMargin = 50, Router? fallbackRouter = null)
    {
        _shapeMargin = shapeMargin;
        _globalMargin = globalMargin;
        _fallbackRouter = fallbackRouter ?? new NormalRouter();
    }

    public override Vector2[] GetRoute(Diagram diagram, BaseLinkModel link)
    {
        if (!link.IsAttached)
            return _fallbackRouter.GetRoute(diagram, link);

        if (link.Source is not SinglePortAnchor spa1)
            return _fallbackRouter.GetRoute(diagram, link);

        if (link.Target is not SinglePortAnchor targetAnchor)
            return _fallbackRouter.GetRoute(diagram, link);

        var sourcePort = spa1.Port;
        if (targetAnchor == null || sourcePort.Parent.Size == null || targetAnchor.Port.Parent.Size == null)
            return _fallbackRouter.GetRoute(diagram, link);

        var targetPort = targetAnchor.Port;

        var shapeMargin = _shapeMargin;
        var globalBoundsMargin = _globalMargin;
        var spots = new HashSet<Vector2>();
        var verticals = new List<float>();
        var horizontals = new List<float>();
        var sideA = sourcePort.Alignment;
        var sideAVertical = IsVerticalSide(sideA);
        var sideB = targetPort.Alignment;
        var sideBVertical = IsVerticalSide(sideB);
        var originA = GetPortPositionBasedOnAlignment(sourcePort);
        var originB = GetPortPositionBasedOnAlignment(targetPort);
        var shapeA = sourcePort.Parent.GetBounds(includePorts: true)!;
        var shapeB = targetPort.Parent.GetBounds(includePorts: true)!;
        var inflatedA = shapeA.Inflate(shapeMargin, shapeMargin);
        var inflatedB = shapeB.Inflate(shapeMargin, shapeMargin);

        if (inflatedA.Intersects(inflatedB))
        {
            shapeMargin = 0;
            inflatedA = shapeA;
            inflatedB = shapeB;
        }

        // Curated bounds to stick to
        var bounds = inflatedA.Union(inflatedB).Inflate(globalBoundsMargin, globalBoundsMargin);

        // Add edges to rulers
        verticals.Add(inflatedA.Left);
        verticals.Add(inflatedA.Right);
        horizontals.Add(inflatedA.Top);
        horizontals.Add(inflatedA.Bottom);
        verticals.Add(inflatedB.Left);
        verticals.Add(inflatedB.Right);
        horizontals.Add(inflatedB.Top);
        horizontals.Add(inflatedB.Bottom);

        // Rulers at origins of shapes
        (sideAVertical ? verticals : horizontals).Add(sideAVertical ? originA.X : originA.Y);
        (sideBVertical ? verticals : horizontals).Add(sideBVertical ? originB.X : originB.Y);

        // Points of shape antennas
        spots.Add(GetOriginSpot(originA, sideA, shapeMargin));
        spots.Add(GetOriginSpot(originB, sideB, shapeMargin));

        // Sort rulers
        verticals.Sort();
        horizontals.Sort();

        // Create grid
        var grid = RulersToGrid(verticals, horizontals, bounds);
        var gridPoints = GridToSpots(grid, new[] { inflatedA, inflatedB });

        // Add to spots
        spots.UnionWith(gridPoints);

        var ys = spots.Select(p => p.Y).Distinct().ToList();
        var xs = spots.Select(p => p.X).Distinct().ToList();
        ys.Sort();
        xs.Sort();

        var nodes = spots.ToDictionary(p => p, p => new Node(p));

        for (var i = 0; i < ys.Count; i++)
        {
            for (var j = 0; j < xs.Count; j++)
            {
                var b = new Vector2(xs[j], ys[i]);
                if (!nodes.ContainsKey(b))
                    continue;

                if (j > 0)
                {
                    var a = new Vector2(xs[j - 1], ys[i]);

                    if (nodes.ContainsKey(a))
                    {
                        nodes[a].ConnectedTo.Add(nodes[b]);
                        nodes[b].ConnectedTo.Add(nodes[a]);
                    }
                }

                if (i > 0)
                {
                    var a = new Vector2(xs[j], ys[i - 1]);

                    if (nodes.ContainsKey(a))
                    {
                        nodes[a].ConnectedTo.Add(nodes[b]);
                        nodes[b].ConnectedTo.Add(nodes[a]);
                    }
                }
            }
        }

        var nodeA = nodes[GetOriginSpot(originA, sideA, shapeMargin)];
        var nodeB = nodes[GetOriginSpot(originB, sideB, shapeMargin)];
        var path = AStarPathfinder.GetPath(nodeA, nodeB);

        if (path.Count > 0)
        {
            return path.ToArray();
        }
        else
        {
            return _fallbackRouter.GetRoute(diagram, link);
        }
    }

    private static Grid RulersToGrid(List<float> verticals, List<float> horizontals, Rectangle bounds)
    {
        var result = new Grid();
        verticals.Sort();
        horizontals.Sort();

        var lastX = bounds.Left;
        var lastY = bounds.Top;
        var column = 0;
        var row = 0;

        foreach (var y in horizontals)
        {
            foreach (var x in verticals)
            {
                result.Set(row, column++, new Rectangle(lastX, lastY, x, y));
                lastX = x;
            }

            // Last cell of the row
            result.Set(row, column, new Rectangle(lastX, lastY, bounds.Right, y));
            lastX = bounds.Left;
            lastY = y;
            column = 0;
            row++;
        }

        lastX = bounds.Left;

        // Last fow of cells
        foreach (var x in verticals)
        {
            result.Set(row, column++, new Rectangle(lastX, lastY, x, bounds.Bottom));
            lastX = x;
        }

        // Last cell of last row
        result.Set(row, column, new Rectangle(lastX, lastY, bounds.Right, bounds.Bottom));
        return result;
    }

    private static HashSet<Vector2> GridToSpots(Grid grid, Rectangle[] obstacles)
    {
        bool IsInsideObstacles(Vector2 p)
        {
            foreach (var obstacle in obstacles)
            {
                if (obstacle.ContainsPoint(p))
                    return true;
            }

            return false;
        }

        void AddIfNotInsideObstacles(HashSet<Vector2> list, Vector2 p)
        {
            if (!IsInsideObstacles(p))
            {
                list.Add(p);
            }
        }

        var gridPoints = new HashSet<Vector2>();
        foreach (var (row, data) in grid.Data)
        {
            var firstRow = row == 0;
            var lastRow = row == grid.Rows - 1;

            foreach (var (col, r) in data)
            {
                var firstCol = col == 0;
                var lastCol = col == grid.Columns - 1;
                var nw = firstCol && firstRow;
                var ne = firstRow && lastCol;
                var se = lastRow && lastCol;
                var sw = lastRow && firstCol;

                if (nw || ne || se || sw)
                {
                    AddIfNotInsideObstacles(gridPoints, r.NorthWest);
                    AddIfNotInsideObstacles(gridPoints, r.NorthEast);
                    AddIfNotInsideObstacles(gridPoints, r.SouthWest);
                    AddIfNotInsideObstacles(gridPoints, r.SouthEast);
                }
                else if (firstRow)
                {
                    AddIfNotInsideObstacles(gridPoints, r.NorthWest);
                    AddIfNotInsideObstacles(gridPoints, r.North);
                    AddIfNotInsideObstacles(gridPoints, r.NorthEast);
                }
                else if (lastRow)
                {
                    AddIfNotInsideObstacles(gridPoints, r.SouthEast);
                    AddIfNotInsideObstacles(gridPoints, r.South);
                    AddIfNotInsideObstacles(gridPoints, r.SouthWest);
                }
                else if (firstCol)
                {
                    AddIfNotInsideObstacles(gridPoints, r.NorthWest);
                    AddIfNotInsideObstacles(gridPoints, r.West);
                    AddIfNotInsideObstacles(gridPoints, r.SouthWest);
                }
                else if (lastCol)
                {
                    AddIfNotInsideObstacles(gridPoints, r.NorthEast);
                    AddIfNotInsideObstacles(gridPoints, r.East);
                    AddIfNotInsideObstacles(gridPoints, r.SouthEast);
                }
                else
                {
                    AddIfNotInsideObstacles(gridPoints, r.NorthWest);
                    AddIfNotInsideObstacles(gridPoints, r.North);
                    AddIfNotInsideObstacles(gridPoints, r.NorthEast);
                    AddIfNotInsideObstacles(gridPoints, r.East);
                    AddIfNotInsideObstacles(gridPoints, r.SouthEast);
                    AddIfNotInsideObstacles(gridPoints, r.South);
                    AddIfNotInsideObstacles(gridPoints, r.SouthWest);
                    AddIfNotInsideObstacles(gridPoints, r.West);
                    AddIfNotInsideObstacles(gridPoints, r.Center);
                }
            }
        }

        // Reduce repeated points and filter out those who touch shapes
        return gridPoints;
    }

    private static bool IsVerticalSide(PortAlignment alignment)
        => alignment == PortAlignment.Top || alignment == PortAlignment.Bottom;

    private static Vector2 GetOriginSpot(Vector2 p, PortAlignment alignment, float shapeMargin)
    {
        return alignment switch
        {
            PortAlignment.Top => p + new Vector2(0, -shapeMargin),
            PortAlignment.Right => p + new Vector2(shapeMargin, 0),
            PortAlignment.Bottom => p + new Vector2(0, shapeMargin),
            PortAlignment.Left => p + new Vector2(-shapeMargin, 0),
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

    public void Set(float row, float column, Rectangle rectangle)
    {
        Rows = MathF.Max(Rows, row + 1);
        Columns = MathF.Max(Columns, column + 1);

        if (!Data.ContainsKey(row))
        {
            Data.Add(row, new Dictionary<float, Rectangle>());
        }

        Data[row].Add(column, rectangle);
    }
}

static class AStarPathfinder
{
    public static IReadOnlyList<Vector2> GetPath(Node start, Node goal)
    {
        var frontier = new PriorityQueue<Node, float>();
        frontier.Enqueue(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(goal))
                break;

            foreach (var next in current.ConnectedTo)
            {
                var newCost = current.Cost + 1.0f;
                if (current.Parent != null && IsChangeOfDirection(current.Parent.Position, current.Position, next.Position))
                {
                    newCost *= newCost;
                    newCost *= newCost;
                }

                if (next.Cost == 0 || newCost < next.Cost)
                {
                    next.Cost = newCost;
                    var priority = newCost + Heuristic(next.Position, goal.Position);
                    frontier.Enqueue(next, priority);
                    next.Parent = current;
                }
            }
        }

        var result = new List<Vector2>();
        var c = goal.Parent;

        while (c != null && c != start)
        {
            result.Insert(0, c.Position);
            c = c.Parent;
        }

        if (c == start)
        {
            result = SimplifyPath(result);

            // In case of paths with one bend
            if (result.Count > 2)
            {
                if (AreOnSameLine(result[^2], result[^1], goal.Position))
                {
                    result.RemoveAt(result.Count - 1);
                }

                if (AreOnSameLine(start.Position, result[0], result[1]))
                {
                    result.RemoveAt(0);
                }
            }

            return result;
        }
        else
        {
            return Array.Empty<Vector2>();
        }
    }

    private static bool AreOnSameLine(Vector2 prev, Vector2 curr, Vector2 next)
    {
        return (prev.X == curr.X && curr.X == next.X) || (prev.Y == curr.Y && curr.Y == next.Y);
    }

    private static List<Vector2> SimplifyPath(List<Vector2> path)
    {
        for (var i = path.Count - 2; i > 0; i--)
        {
            var prev = path[i + 1];
            var curr = path[i];
            var next = path[i - 1];

            if (AreOnSameLine(prev, curr, next))
            {
                path.RemoveAt(i);
            }
        }

        return path;
    }

    private static bool IsChangeOfDirection(Vector2 a, Vector2 b, Vector2 c)
    {
        if (a.X == b.X && b.X != c.X)
            return true;

        if (a.Y == b.Y && b.Y != c.Y)
            return true;

        return false;
    }

    private static float Heuristic(Vector2 a, Vector2 b)
    {
        return MathF.Abs(a.X - b.X) + MathF.Abs(a.Y - b.Y);
    }
}

class Node
{
    public Node(Vector2 position)
    {
        Position = position;
        ConnectedTo = new List<Node>();
    }

    public Vector2 Position { get; }
    public List<Node> ConnectedTo { get; }

    public float Cost { get; internal set; }
    public Node? Parent { get; internal set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Node item)
            return false;

        return Position.Equals(item.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
