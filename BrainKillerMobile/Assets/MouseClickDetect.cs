using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseClickDetect : MonoBehaviour
{
    public testVolumeGeneration targetColliderParent;
    public GameObject swcParent;
    private CoordinateMapping coordinateMapping = new CoordinateMapping();
    private List<GameObject> addedNodes = new List<GameObject>();
    private List<Vector3> texturePointsHistory = new List<Vector3>();
    private MoveGenerator moveGenerator = new MoveGenerator();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider targetCollider = targetColliderParent.getChildCollider();
            if (targetCollider == null)
            {
                Debug.Log("targetCollider is null");
                return;
            }

            RaycastHit hit;
            if (targetCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject nodePrefab = Resources.Load<GameObject>("Prefabs/SwcNode");

                // 将世界坐标系的交点转换为碰撞体的局部坐标系
                Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
                // Debug.Log("Hit point in local space: " + localHitPoint);

                // 将世界坐标系的射线转换为碰撞体的局部坐标系
                // Vector3 localRayOrigin = hit.collider.transform.InverseTransformPoint(ray.origin);
                Vector3 localRayDirection = hit.collider.transform.InverseTransformDirection(ray.direction);
                Ray localRay = new Ray(localHitPoint, localRayDirection);

                // Debug.Log("Local Ray Direction: " + localRay.direction);

                // 获取局部坐标系下的最大强度点
                Vector3 localMaxIntensityPoint = getLocalMaxIntensityPoint(localHitPoint, localRay);
                texturePointsHistory.Add(localMaxIntensityPoint);
                // Debug.Log("Local Max Intensity Point: " + localMaxIntensityPoint);

                // 将局部坐标系下的最大强度点转换为世界坐标系
                Vector3 worldMaxIntensityPoint = hit.collider.transform.TransformPoint(localMaxIntensityPoint);
                // Debug.Log("World Max Intensity Point: " + worldMaxIntensityPoint);

                // 在世界坐标系下创建最大强度点 不直接生成，发送消息
                GameObject maxIntensityNode = Instantiate(nodePrefab);
                maxIntensityNode.transform.localScale = Vector3.one;
                maxIntensityNode.transform.position = worldMaxIntensityPoint;
                maxIntensityNode.transform.SetParent(hit.collider.transform);
                addedNodes.Add(maxIntensityNode);

                // 一段时间后移除，只起到提示作用
                Destroy(maxIntensityNode, 5.0f);

                // 将世界坐标系的点转换成 SWC Parent 坐标系下
                Vector3 swcParentMaxIntensityPoint = swcParent.transform.InverseTransformPoint(worldMaxIntensityPoint);

                // 转换 gameobject 信息到 move
                Move newMove = moveGenerator.generateMove(swcParentMaxIntensityPoint, 1.0f);
                moveGenerator.sendMove(newMove);
            }
        }
    }

    // public Move generateMoveFromGameObject(GameObject nodeGameObject, GameObject ParentGameObject = null)
    // {
    //     Vector3 worldMaxIntensityPoint = nodeGameObject.transform.position;
    //     string moveMeta = $"x:{worldMaxIntensityPoint.x},y:{worldMaxIntensityPoint.y},z:{worldMaxIntensityPoint.z},r:{nodeGameObject.transform.localScale.x}";
    //     int replica = 255;
    //     int parentID = 10000001;
    //     if (ParentGameObject != null)
    //     {
    //         parentID = ParentGameObject.GetInstanceID();
    //         return null;
    //     }
    //     int nodeGameObjectID = nodeGameObject.GetInstanceID();
    //     Move move = new Move(0, replica, parentID, nodeGameObjectID, moveMeta);
    //     return move;
    // }

    public Vector3 getLocalMaxIntensityPoint(Vector3 hitPoint, Ray ray)
    {
        return coordinateMapping.calculateMaxIntensityPoint(targetColliderParent.getDataset(), hitPoint, ray, 128);
    }

    public List<GameObject> getAddedNodes()
    {
        return addedNodes;
    }

    public List<Vector3> getTexturePointsHistory()
    {
        return texturePointsHistory;
    }

    public GameObject getImageGameObject()
    {
        return targetColliderParent.transform.GetChild(0).GetChild(0).gameObject;
    }
}