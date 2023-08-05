using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathFindAStar
{
    public class PathNode
    {
        // ���������� ����� �� �����.
        public Province Position { get; set; }
        // ����� ���� �� ������ (G).
        public int PathLengthFromStart { get; set; }
        // �����, �� ������� ������ � ��� �����.
        public PathNode CameFrom { get; set; }
        // ��������� ���������� �� ���� (H).
        public int HeuristicEstimatePathLength { get; set; }
        // ��������� ������ ���������� �� ���� (F).
        public int EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }

    private static int GetHeuristicPathLength(Province from, Province to)
    {
        return Mathf.RoundToInt(Vector3.Distance(from.Position, to.Position));
    }

    public static List<Province> FindPath(Province start, Province goal, Predicate<Province> EnterToProvinceIf = null, Division division = null)
    {
        if (division != null)
        {
            if(goal.AllowedForDivision(division) == false)
            {
                return new List<Province>();
            }
        }
        if (EnterToProvinceIf?.Invoke(goal) == false)
        {
            return new List<Province>();
        }
        // ��� 1.
        var closedSet = new List<PathNode>();
        var openSet = new List<PathNode>();
        // ��� 2.
        PathNode startNode = new PathNode()
        {
            Position = start,
            CameFrom = null,
            PathLengthFromStart = 0,
            HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
        };
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            // ��� 3.
            var currentNode = openSet.OrderBy(node =>
              node.EstimateFullPathLength).First();
            // ��� 4.
            if (currentNode.Position == goal)
                return GetPathForNode(currentNode);
            // ��� 5.
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            // ��� 6.
            foreach (var neighbourNode in GetNeighbours(currentNode, goal, EnterToProvinceIf, division))
            {
                // ��� 7.
                if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                    continue;
                var openNode = openSet.FirstOrDefault(node =>
                  node.Position == neighbourNode.Position);
                // ��� 8.
                if (openNode == null)
                    openSet.Add(neighbourNode);
                else
                  if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                {
                    // ��� 9.
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                }
            }
        }
        // ��� 10.
        return null;
    }

    private static List<PathNode> GetNeighbours(PathNode pathNode, Province goal, Predicate<Province> EnterToProvinceIf, Division division)
    {
        var result = new List<PathNode>();
        foreach (var cont in pathNode.Position.Contacts)
        {
            if (division != null)
            {
                if (cont.AllowedForDivision(division) == false)
                {
                    continue;
                }
            }
            if (EnterToProvinceIf != null)
            {
                if (EnterToProvinceIf?.Invoke(cont) == false)
                {
                    continue;
                }
            }
            PathNode startNode = new PathNode()
            {
                Position = cont,
                CameFrom = pathNode,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(cont, goal)
            };
            result.Add(startNode);
        }
        return result;
    }

    private static List<Province> GetPathForNode(PathNode pathNode)
    {
        var result = new List<Province>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.Position);
            currentNode = currentNode.CameFrom;
        }
        result.Reverse();
        return result;
    }
}
