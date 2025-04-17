using System;
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
    // private MoveGenerator moveGenerator = new MoveGenerator();

    // String SwcNodeColor = "4F9ED9";
    private List<GameObject> lastTwoClickedNodes = new List<GameObject>();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 检查是否点击到 tag 为 SWCNode 的物体
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hitObj in hits)
            {
                Debug.Log("Hit object: " + hitObj.collider.gameObject.name);
                if (hitObj.collider.gameObject.CompareTag("SWCNode"))
                {
                    Debug.Log("Hit SWCNode: " + hitObj.collider.gameObject.name);
                    GameObject clickedNode = hitObj.collider.gameObject;
                    appendToLastTwoClickedNodes(clickedNode);

                    return;
                }
            }
            
            // 如果没有点击到碰撞体，找到图像中的最大强度点
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

                // 获取碰撞体局部坐标系下的最大强度点
                Vector3 localMaxIntensityPoint = getLocalMaxIntensityPoint(localHitPoint, localRay);
                texturePointsHistory.Add(localMaxIntensityPoint);
                Debug.Log("Local Max Intensity Point: " + localMaxIntensityPoint);

                // 将局部坐标系下的最大强度点转换为世界坐标系
                Vector3 worldMaxIntensityPoint = hit.collider.transform.TransformPoint(localMaxIntensityPoint);
                Debug.Log("World Max Intensity Point: " + worldMaxIntensityPoint);

                // 在世界坐标系下创建最大强度点 不直接生成，发送消息
                GameObject maxIntensityNode = Instantiate(nodePrefab);
                maxIntensityNode.transform.localScale = Vector3.one;
                maxIntensityNode.transform.position = worldMaxIntensityPoint;
                maxIntensityNode.transform.SetParent(hit.collider.transform);
                addedNodes.Add(maxIntensityNode);

                // 一段时间后移除，只起到提示作用
                appendToLastTwoClickedNodes(maxIntensityNode);
                Destroy(maxIntensityNode, 5.0f);

                // 将世界坐标系的点转换成 SWC Parent 坐标系下
                Vector3 swcParentMaxIntensityPoint = swcParent.transform.InverseTransformPoint(worldMaxIntensityPoint);

                // 转换 gameobject 信息到 move
                // Move newMove = moveGenerator.generateMove(swcParentMaxIntensityPoint, 1.0f);
                // moveGenerator.sendMove(newMove);
            }
        }
    }

    private void highlightSelectedNode() {
        foreach (GameObject nodeObj in lastTwoClickedNodes) {
            if (nodeObj != null){
                nodeObj.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

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

    public Color getColorForId(int id) {
        // 定义一组低饱和度的颜色 (RGB值在0-1范围内)
        Color[] colors = new Color[] {
            new Color(1f/255f, 125f/255f, 232f/255f), 
            new Color(254f/255f, 205f/255f, 42f/255f), 
            new Color(24f/255f, 176f/255f, 93f/255f) 
        };
        
        // 获取ID的首位数字
        int firstDigit = id;
        while (firstDigit >= 10) {
            firstDigit /= 10;
        }
        firstDigit = firstDigit % 3;
        return colors[firstDigit];
    }

    public void appendToLastTwoClickedNodes(GameObject node)
    {
        lastTwoClickedNodes.RemoveAll(node => node == null);
        if (lastTwoClickedNodes.Count >= 2)
        {
            if (lastTwoClickedNodes[0].tag != "SWCNode")
            {
                // lastTwoClickedNodes[0].GetComponent<Renderer>().material.color = Color.white;
                Destroy(lastTwoClickedNodes[0]);
            } else {
                lastTwoClickedNodes[0].GetComponent<Renderer>().material.color = getColorForId(DataManager.Instance.userId);
            }
            lastTwoClickedNodes.RemoveAt(0);
        }
        if (lastTwoClickedNodes.Count == 0) {
            lastTwoClickedNodes.Add(node);
        }
        
        if (node != lastTwoClickedNodes[lastTwoClickedNodes.Count - 1]) {
            lastTwoClickedNodes.Add(node);
        }
        highlightSelectedNode();
    }

    public List<GameObject> getLastTwoClickedNodes()
    {
        return lastTwoClickedNodes;
    }
}