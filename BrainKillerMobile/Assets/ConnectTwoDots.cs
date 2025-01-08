using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataLoader;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectTwoDots : MonoBehaviour
{
    public testVolumeGeneration targetColliderParent;
    public CoordinateMapping coordinateMapping;
    public MouseClickDetect mouseClickDetect;
    public GameObject swcParent;
    public testVolumeGeneration imageParent;
    public MoveGenerator moveGenerator;

    private GameObject currentPathHead;

    public void connectLastTwoDots() {
        List<Vector3> dots = fetchLastTwoDots();
        if (dots == null || dots.Count < 2) {
            Debug.LogWarning("[ConnectLastTwoDots] dots is not enough");
            return;
        }
        connectTwoDots(dots);
    }

    public List<Vector3> fetchLastTwoDots() {
        List<GameObject> lastTwoClickedNodes = mouseClickDetect.getLastTwoClickedNodes();
        if (lastTwoClickedNodes.Count != 2) {
            Debug.LogWarning("[fetch last two dots] There are less than 2 dots");
            return null;
        }
        if (lastTwoClickedNodes[0].tag != "SWCNode" ) {
            Debug.LogWarning("[fetch last two dots] The first clicked node should be SWCNode");
            return null;
        } 
        
        if (lastTwoClickedNodes[1].tag == "SWCNode") {
            Debug.LogWarning("[fetch last two dots] The second clicked node should not be SWCNode");
            return null;
        }
        
        currentPathHead = lastTwoClickedNodes[0];

        List<Vector3> dots = new List<Vector3>();

        for (int i = 0; i < 2; i++) {
            GameObject node = lastTwoClickedNodes[i];
            Vector3 worldPos = node.transform.position;
            GameObject imageCollider = imageParent.getChildCollider().gameObject;
            Vector3 imageLocalPos = imageCollider.transform.InverseTransformPoint(worldPos);
            dots.Add(imageLocalPos);
        }

        return dots;
    }

    public void connectTwoDots(List<Vector3> dots) {
        if (dots.Count < 2) {
            Debug.LogWarning("[ConnectTwoDots] There are less than 2 dots");
        }
        coordinateMapping = new CoordinateMapping();
        Dataset3D dataset = targetColliderParent.getDataset();
        List<Vector3> worldPos = new List<Vector3> { dots[0], dots[1] };
        List<Vector3> localPos = coordinateMapping.rayPoints2TexturePoints(worldPos, dataset);
        List<Vector3> path = calculateMaxIntensityPath(coordinateMapping.dataArray3D, localPos[0], localPos[1]);
        List<Vector3> worldPath = new List<Vector3>();
        for (int i = 0; i < path.Count; i++) {
            worldPath.Add(coordinateMapping.texturePoint2RayPoint(path[i], dataset));
        }
        GameObject imageObject = mouseClickDetect.getImageGameObject();
        int gap = 4;

        string headName = currentPathHead.name;
        string[] headNameArray = headName.Split('-');
        if (headNameArray.Length != 2) {
            Debug.LogWarning("[ConnectTwoDots] headNameArray.Length != 2");
            return;
        }
        long prevIndex = long.Parse(headNameArray[1]);

        for (int i = 0; i < worldPath.Count; i++) {
            if (i != 0 && i != worldPath.Count - 1 && (i % gap != 0)) {
                continue;
            }
            Vector3 worldPoint = imageObject.transform.TransformPoint(worldPath[i]);
            Vector3 swcParentLocalPoint = swcParent.transform.InverseTransformPoint(worldPoint);
            
            Move newMove = moveGenerator.generateMove(swcParentLocalPoint, prevIndex,  1.0f);
            prevIndex = newMove.Child;
            moveGenerator.addToSendingQueue(newMove);

            // GameObject nodePrefab = Resources.Load<GameObject>("Prefabs/SwcNode");
            // GameObject nodeGameObject = Instantiate(nodePrefab);
            // nodeGameObject.name = "[AutoConnect]" + nodeGameObject.name;
            // nodeGameObject.transform.localScale = Vector3.one;
            // nodeGameObject.transform.position = worldPoint;
            // nodeGameObject.transform.SetParent(imageObject.transform);
        }
    }

    public List<Vector3> calculateMaxIntensityPath ( List<List<List<float>>> dataArray3D, Vector3 startPos, Vector3 endPos) {
        PathFinder pathFinder = new PathFinder();
        List<Vector3Int> path = pathFinder.calculateMaxIntensityPath(dataArray3D, new Vector3Int((int)startPos.x, (int)startPos.y, (int)startPos.z), new Vector3Int((int)endPos.x, (int)endPos.y, (int)endPos.z));
        var smoothedPath = PathSmoother.SmoothPath(path, 0.5f);
        List<Vector3> smoothedPathVector3 = new List<Vector3>();
        foreach (var point in smoothedPath) {
            smoothedPathVector3.Add(new Vector3(point.position.x, point.position.y, point.position.z));
        }
        Debug.Log("[CalculateMaxIntensityPath] pathVector3.Count: " + smoothedPathVector3.Count);
        return smoothedPathVector3;
    }
}
