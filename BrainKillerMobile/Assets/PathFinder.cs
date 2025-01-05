using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
public class PathFinder
{
    private string GetMemoryUsage()
    {
        float memoryMB = (float)GC.GetTotalMemory(false) / (1024 * 1024);
        return $"{memoryMB:F2}MB";
    }

    public List<Vector3Int> calculateMaxIntensityPath(List<List<List<float>>> dataArray3D, Vector3Int startPos, Vector3Int endPos, float timeout = 20.0f)
    {
        int startTimestamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        // 计算搜索空间的 x y z 限制
        int offset = 5; // 外扩搜索空间
        int xMin = Math.Min(startPos.x, endPos.x) - offset;
        int xMax = Math.Max(startPos.x, endPos.x) + offset;
        int yMin = Math.Min(startPos.y, endPos.y) - offset;
        int yMax = Math.Max(startPos.y, endPos.y) + offset;
        int zMin = Math.Min(startPos.z, endPos.z) - offset;
        int zMax = Math.Max(startPos.z, endPos.z) + offset;
        Debug.Log("total search space size: " + (xMax - xMin) * (yMax - yMin) * (zMax - zMin));

        int width = dataArray3D.Count;
        int height = dataArray3D[0].Count;
        int depth = dataArray3D[0][0].Count;

        // 1. 输入验证
        if (dataArray3D == null || width == 0 || height == 0 || depth == 0)
        {
            return new List<Vector3Int>();
        }
        if (!IsValidPosition(startPos, width, height, depth) || !IsValidPosition(endPos, width, height, depth))
        {
            return new List<Vector3Int>();
        }

        // 2. 初始化
        var costs = new Dictionary<Vector3Int, float>();
        var previous = new Dictionary<Vector3Int, Vector3Int>();
        var priorityQueue = new PriorityQueue<Vector3Int, float>(); 
        var visited = new HashSet<Vector3Int>();

        costs[startPos] = 0;
        priorityQueue.Enqueue(startPos, 0);
        
        // 3. Dijkstra 算法核心
        while (priorityQueue.Count > 0)
        {
            if (DateTimeOffset.Now.ToUnixTimeSeconds() - startTimestamp > timeout)
            {
                Debug.LogError("Pathfinding timeout");
                return new List<Vector3Int>();
            }
            if (priorityQueue.Count % 100 == 0)
            {
                float memoryMB = (float)GC.GetTotalMemory(false) / (1024 * 1024);
                if (memoryMB > 500.0f)
                {
                    Debug.LogError("Memory usage exceeded 500MB");
                    return new List<Vector3Int>();
                }
            }
            var currentPos = priorityQueue.Dequeue(); 
            // Debug.Log("currentPos: " + currentPos);
            // Debug.Log("queue count: " + priorityQueue.Count);
            if (visited.Contains(currentPos))
            {
                continue;
            }
            visited.Add(currentPos);

            if (currentPos.Equals(endPos))
            {
                break; // 找到目标点
            }

            // 判断是否在搜索空间内
            if (currentPos.x < xMin || currentPos.x > xMax || currentPos.y < yMin || currentPos.y > yMax || currentPos.z < zMin || currentPos.z > zMax)
            {
                continue;
            }

            int[] dx = { 0, 0, 0, 0, 1, -1 };
            int[] dy = { 0, 0, 1, -1, 0, 0 };
            int[] dz = { 1, -1, 0, 0, 0, 0 };

            for (int i = 0; i < 6; i++)
            {
                var neighborPos = new Vector3Int(currentPos.x + dx[i], currentPos.y + dy[i], currentPos.z + dz[i]);
                
                if (visited.Contains(neighborPos))
                {
                    continue;
                }

                if (IsValidPosition(neighborPos, width, height, depth))
                {
                    if (costs.ContainsKey(neighborPos) == false)
                    {
                        costs[neighborPos] = float.MaxValue;
                    }
                    float newCost = (float)GetCost(currentPos, neighborPos, dataArray3D);
                    // Debug.Log("newCost for " + neighborPos + ": " + newCost);
                    if (newCost < costs[neighborPos])
                    {
                        costs[neighborPos] = newCost;
                        previous[neighborPos] = currentPos;
                        priorityQueue.Enqueue(neighborPos, newCost);
                        // Debug.Log("Enqueue " + neighborPos);
                    }
                }
            }
        }

        // 4. 路径回溯
        var path = new List<Vector3Int>();
        int maxIterations = 100;
        int iterations = 0;
        if (previous.ContainsKey(endPos))
        {
            var current = endPos;
            var visitedInPath = new HashSet<Vector3Int>(); // 防止环路

            while (!current.Equals(startPos))
            {
                if (iterations++ > maxIterations)
                {
                    Debug.LogError($"Path reconstruction exceeded {maxIterations} iterations - possible loop detected");
                    return new List<Vector3Int>();
                }

                if (!previous.ContainsKey(current))
                {
                    Debug.LogError($"Path broken at {current}");
                    return new List<Vector3Int>();
                }

                if (!visitedInPath.Add(current))
                {
                    Debug.LogError($"Loop detected at {current}");
                    return new List<Vector3Int>();
                }

                path.Add(current);
                current = previous[current];
                // Debug.Log($"Path reconstruction: current={current}, pathLength={path.Count}");
            }

            path.Add(startPos);
            path.Reverse();
        }
        else
        {
            Debug.LogError("No path found");
        }
        // log path content
        string pathLine = "";
        // join path with ->
        for (int i = 0; i < path.Count; i++)
        {
            pathLine += path[i].ToString();
            if (i < path.Count - 1)
            {
                pathLine += " -> ";
            }
        }
        Debug.Log("find path: " + pathLine);


        Debug.Log($"Final memory usage: {GetMemoryUsage()}");
        return path;
    }

    private bool IsValidPosition(Vector3Int pos, int width, int height, int depth)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height && pos.z >= 0 && pos.z < depth;
    }

    private double GetCost(Vector3Int pos1, Vector3Int pos2, List<List<List<float>>> dataArray3D)
    {
        float lambuda = 10.0f;
        float nomolizedIntensity1 = dataArray3D[pos1.x][pos1.y][pos1.z] / 255.0f;
        float nomolizedIntensity2 = dataArray3D[pos2.x][pos2.y][pos2.z] / 255.0f;
        double g1 = lambuda * Math.Pow(1 - nomolizedIntensity1, 2);
        g1 = Math.Exp(g1);
        double g2 = lambuda * Math.Pow(1 - nomolizedIntensity2, 2);
        g2 = Math.Exp(g2);
        double euclideanDistance = Math.Sqrt(Math.Pow(pos1.x - pos2.x, 2) + Math.Pow(pos1.y - pos2.y, 2) + Math.Pow(pos1.z - pos2.z, 2));
        return euclideanDistance * (g1 + g2) / 2;
    }
}

public class PriorityQueue<TItem, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TItem item, TPriority priority)> _elements = new List<(TItem item, TPriority priority)>();

    public int Count => _elements.Count;

    public void Enqueue(TItem item, TPriority priority)
    {
        _elements.Add((item, priority));
        _elements.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public TItem Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty.");
        }
        var item = _elements[0].item;
        _elements.RemoveAt(0);
        return item;
    }
}