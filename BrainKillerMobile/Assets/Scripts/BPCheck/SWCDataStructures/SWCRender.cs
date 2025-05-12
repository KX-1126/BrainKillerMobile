using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using BPCheck.SwcIO;
using Unity.VisualScripting;
using UnityEngine;
using DataLoader;
public class SWCRender : MonoBehaviour
{
    private float timer = 0f;
    private float updateInterval = 0.1f;

    public SWC curSwc;

    public String curSwcText = "";
    string swcFilePath;
    // use a set to record rended node
    public Dictionary<int, GameObject> renderedNodeGameObjects = new Dictionary<int, GameObject>();
    private HashSet<string> renderedConnections = new HashSet<string>();

    private List<int> collaborators = new List<int>();

    public addUserRow addUserRowFunc;

    public MouseClickDetect detector;

    private void Start()
    {
        DataManager dataManager = DataManager.Instance;
        string swcFileName = $"tree_{dataManager.userId}.swc";
        print("swcFileName: " + swcFileName);
        string exePath = System.AppDomain.CurrentDomain.BaseDirectory;
        print("exePath: " + exePath);
        // swcFilePath = Path.Combine(exePath, swcFileName);
        // print("swcFilePath: " + swcFilePath);
        swcFilePath = "F:\\Repos\\Tree_CRDT\\tree_255.swc";
        // swcFilePath = "F:\\Repos\\Tree_CRDT\\mergeSWC\\carSWC\\Img10_X_3746.77_Y_5293.77_Z_3539.6.ano.swc"; // FOR DEBUG

        // addUserRowFunc.addRow(DataManager.Instance.userId.ToString() + "(You)", getColorForId(DataManager.Instance.userId));
    }

    private void Update() {
        timer += Time.deltaTime; 
        if (timer >= updateInterval) {
            timer = 0f;
            // Call the method to update the SWC
            UpdateSwc();
        }
    }

    private void UpdateSwc() {
        // Logic to update the SWC
        // This could involve re-reading the SWC file and re-rendering
        SwcFileReader swcFileReader = new SwcFileReader();
        string swcText = swcFileReader.readSwcFile(swcFilePath);
        // string swcText = File.ReadAllText(swcFilePath); // FOR DEBUG

        if (swcText == "") {
            print("SWC file read failed");
            return;
        }

        // 优化大字符串比较，先比较长度，再用Equals避免分配
        if (swcText.Length == curSwcText.Length && swcText.Equals(curSwcText, StringComparison.Ordinal)) {
            // No changes in the SWC file, no need to update
            return;
        }

        curSwcText = swcText;

        SWC swc = new SWC();
        swc.buildTree(SWCDataStructure.loadSWC(swcText));
        Render(swc);
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

    private void addCollaborator(int nodeId) {
        string id = nodeId.ToString();
        // find first zero in id,cut the string from 0 to the first zero
        int firstZeroIndex = id.IndexOf('0');
        if (firstZeroIndex != -1)
        {
            id = id.Substring(0, firstZeroIndex);
        }
        int collaboratorId = int.Parse(id);
        if (collaboratorId == 1) 
        {
            return;
        }
        if (!collaborators.Contains(collaboratorId))
        {
            collaborators.Add(collaboratorId);
            addUserRowFunc.addRow(collaboratorId.ToString(), getColorForId(collaboratorId));
        }
    }

    public void Render(SWC swc)
    {
        curSwc = swc;
        // remove existing nodes not in the new swc
        foreach (var kvp in renderedNodeGameObjects)
        {
            if (!swc.nodeIDSet.Contains(kvp.Key))
            {
                Debug.Log("Destroying node: " + kvp.Key);
                Destroy(kvp.Value);
            }
        }
        foreach (Node node in swc.indexNodeMap.Values)
        {
            if (renderedNodeGameObjects.ContainsKey(node.id))
            {
                continue;
            }
            GameObject nodePrefab = Resources.Load<GameObject>("Prefabs/SwcNode");
            GameObject nodeGameObject = Instantiate(nodePrefab,transform);
            nodeGameObject.GetComponent<Renderer>().material.color = getColorForId(node.id);
            addCollaborator(node.id);
            
            // 使用 swc 的数据结构来判断节点类型
            Debug.Log($"node {node.id} is a {node.type} node with {node.children.Count} children");
            if (node.pid == -1)
            {
                nodeGameObject.name = "Head-" + node.id;
            }
            else if (swc.branchNodes.Contains(node))
            {
                nodeGameObject.name = "Branch-" + node.id;
                Debug.Log($"Setting node {node.id} as Branch node");
            }
            else if (swc.endNodes.Contains(node))
            {
                nodeGameObject.name = "End-" + node.id;
            }
            else
            {
                nodeGameObject.name = "Node-" + node.id;
            }
            
            nodeGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ);
            nodeGameObject.tag = "SWCNode";
            renderedNodeGameObjects.Add(node.id, nodeGameObject);
        }

        // 自动选中上一次连线的末尾节点
        if (ConnectTwoDots.lastTracedNodeId != -1 && renderedNodeGameObjects.ContainsKey(ConnectTwoDots.lastTracedNodeId))
        {
            GameObject lastNode = renderedNodeGameObjects[ConnectTwoDots.lastTracedNodeId];
            detector.appendToLastTwoClickedNodes(lastNode);
            Debug.Log("Auto select last node: " + ConnectTwoDots.lastTracedNodeId);
            // 清空 last id
            ConnectTwoDots.lastTracedNodeId = -1;
        }
    }

}
