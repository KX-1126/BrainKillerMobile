using System;
using System.Collections.Generic;
using DataLoader;
using Unity.VisualScripting;
using UnityEngine;

public class CoordinateMapping
{
    private String datasetName;
    public List<List<List<float>>> dataArray3D = null;

    public Vector3 calculateMaxIntensityPoint(Dataset3D dataset, Vector3 start, Ray ray, int numPoints)
    {
        List<Vector3> rayPoints = getRayPoints(start, ray, numPoints);
        List<Vector3> texturePoints = rayPoints2TexturePoints(rayPoints, dataset);
        return findMaxIntensityPoint(texturePoints, dataset);
    }

    public List<Vector3> getRayPoints(Vector3 start, Ray ray, int numPoints, float distance = 2.0f)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 endPoint = ray.origin + ray.direction * distance;
        
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = Vector3.Lerp(start, endPoint, t);
            points.Add(point);
        }
        
        return points;
    }

    public List<Vector3> rayPoints2TexturePoints(List<Vector3> rayPoints, Dataset3D dataset)
    {
        fillDataArrayIfNeeded(dataset);
        List<Vector3> texturePoints = new List<Vector3>();
        for (int i = 0; i < rayPoints.Count; i++)
        {
            Vector3 rayPoint = rayPoints[i];
            Vector3 texturePoint = rayPoint2TexturePoint(rayPoint, dataset);
            if (texturePoint.x < 0 || texturePoint.x >= dataset.dimX ||
                texturePoint.y < 0 || texturePoint.y >= dataset.dimY ||
                texturePoint.z < 0 || texturePoint.z >= dataset.dimZ)
            {
                // Debug.LogWarning("Texture Point out of range: " + texturePoint);
                continue;
            }
            texturePoints.Add(texturePoint);
        }
        return texturePoints;
    }

    private Vector3 rayPoint2TexturePoint(Vector3 rayPoint, Dataset3D dataset)
    {
        fillDataArrayIfNeeded(dataset);
        Vector3 newPoint = rayPoint + new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 texturePoint = Vector3.zero;
        texturePoint.x = newPoint.x * dataset.dimX;
        texturePoint.y = newPoint.y * dataset.dimY;
        texturePoint.z = newPoint.z * dataset.dimZ;
        // Debug.Log("rayPoint: " + rayPoint + "convert to Texture Point: " + texturePoint);
        return texturePoint;
    }

    public Vector3 texturePoint2RayPoint(Vector3 texturePoint, Dataset3D dataset)
    {
        Vector3 rayPoint = Vector3.zero;
        rayPoint.x = texturePoint.x / dataset.dimX;
        rayPoint.y = texturePoint.y / dataset.dimY;
        rayPoint.z = texturePoint.z / dataset.dimZ;
        rayPoint -= new Vector3(0.5f, 0.5f, 0.5f);
        // Debug.Log("Texture Point: " + texturePoint + "convert to Ray Point: " + rayPoint);
        return rayPoint;
    }

    public Vector3 findMaxIntensityPoint(List<Vector3> texturePoints, Dataset3D dataset)
    {
        float maxIntensity = float.MinValue;
        Vector3 maxIntensityPoint = Vector3.zero;
        for (int i = 0; i < texturePoints.Count; i++)
        {
            Vector3 point = texturePoints[i];
            float intensity = dataArray3D[(int)point.x][(int)point.y][(int)point.z];
            if (intensity > maxIntensity)
            {
                maxIntensity = intensity;
                maxIntensityPoint = point;
            }
        }
        Debug.Log("Max Intensity Point: " + maxIntensityPoint);
        Vector3 rayPoint = texturePoint2RayPoint(maxIntensityPoint, dataset);
        Debug.Log("Max Intensity Point in ray space: " + rayPoint);
        return rayPoint;
    }

    public void fillDataArrayIfNeeded(Dataset3D dataset)
    {
        if (dataArray3D == null || dataset.datasetName != this.datasetName)
        {
            datasetName = dataset.datasetName;
            dataArray3D = new List<List<List<float>>>();
            for (int i = 0; i < dataset.dimX; i++)
            {
                List<List<float>> yArray = new List<List<float>>();
                for (int j = 0; j < dataset.dimY; j++)
                {
                    List<float> zArray = new List<float>();
                    for (int k = 0; k < dataset.dimZ; k++)
                    {
                        zArray.Add(dataset.data[i + j * dataset.dimX + k * dataset.dimX * dataset.dimY]);
                    }
                    yArray.Add(zArray);
                }
                dataArray3D.Add(yArray);
            }
        }
    }
}