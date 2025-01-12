using UnityEngine;
using System.Collections.Generic;

public class PathSmoother
{
    [System.Serializable]
    public class SmoothedPathPoint
    {
        public Vector3 position;
        public float timeStamp;

        public SmoothedPathPoint(Vector3 pos, float time)
        {
            position = pos;
            timeStamp = time;
        }
    }

    public static List<Vector3> SmoothPathByWindow(List<Vector3Int> originalPath, int windowSize = 5)
    {
        if (windowSize < 2)
        {
            return originalPath.ConvertAll(v => (Vector3)v); // 直接返回原始线条
        }

        int length = originalPath.Count;
        int halfWindowSize = windowSize / 2;

        List<Vector3> smoothedLine = new List<Vector3>(originalPath.Count);
        foreach (var point in originalPath)
        {
            smoothedLine.Add(point); // 初始化 smoothedLine
        }

        for (int i = 1; i < length - 1; i++)
        {
            List<Vector3> winC = new List<Vector3>();
            List<float> winW = new List<float>();

            // 添加中心点及其权重
            winC.Add(originalPath[i]);
            winW.Add(1.0f + halfWindowSize);

            // 添加左右相邻的点及其权重
            for (int j = 1; j <= halfWindowSize; j++)
            {
                if (i - j >= 0)
                {
                    winC.Add(originalPath[i - j]);
                    winW.Add(1.0f);
                }
                if (i + j < length)
                {
                    winC.Add(originalPath[i + j]);
                    winW.Add(1.0f);
                }
            }

            float x = 0f;
            float y = 0f;
            float z = 0f;
            float s = 0f;

            for (int k = 0; k < winW.Count; k++)
            {
                x += winW[k] * winC[k].x;
                y += winW[k] * winC[k].y;
                z += winW[k] * winC[k].z;
                s += winW[k];
            }

            smoothedLine[i] = new Vector3(x / s, y / s, z / s);
        }

        return smoothedLine;
    }

    public static List<SmoothedPathPoint> SmoothPath(List<Vector3Int> originalPath, float pointSpacing = 0.5f)
    {
        var smoothedPath = new List<SmoothedPathPoint>();
        if (originalPath.Count < 2) return smoothedPath;

        // Convert to Vector3
        var points = new List<Vector3>();
        foreach (var point in originalPath)
        {
            points.Add(new Vector3(point.x, point.y, point.z));
        }

        // Generate smoothed points using Catmull-Rom spline
        float totalDistance = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1] : points[i];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i < points.Count - 2 ? points[i + 2] : p2;

            float segmentLength = Vector3.Distance(p1, p2);
            int subdivisions = Mathf.CeilToInt(segmentLength / pointSpacing);

            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                Vector3 smoothedPoint = CatmullRomPoint(p0, p1, p2, p3, t);
                smoothedPath.Add(new SmoothedPathPoint(smoothedPoint, totalDistance));
                totalDistance += pointSpacing;
            }
        }

        // Add final point
        smoothedPath.Add(new SmoothedPathPoint(points[points.Count - 1], totalDistance));
        return smoothedPath;
    }

    private static Vector3 CatmullRomPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (-t3 + 2f * t2 - t) * p0 +
            (3f * t3 - 5f * t2 + 2f) * p1 +
            (-3f * t3 + 4f * t2 + t) * p2 +
            (t3 - t2) * p3
        );
    }
}