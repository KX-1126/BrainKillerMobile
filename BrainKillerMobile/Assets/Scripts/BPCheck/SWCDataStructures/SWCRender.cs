using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BPCheck.SwcIO;
using Unity.VisualScripting;
using UnityEngine;

public class SWCRender : MonoBehaviour
{
    private float timer = 0f;
    private float updateInterval = 1f;

    public SWC curSwc;
    string swcFilePath;
    // use a set to record rended node
    private Dictionary<int, GameObject> renderedNodeGameObjects = new Dictionary<int, GameObject>();
    private HashSet<string> renderedConnections = new HashSet<string>();

    private void Start()
    {
        // DataManager dataManager = DataManager.Instance;
        // string swcFileName = $"tree_{dataManager.userId}.swc";
        // print("swcFileName: " + swcFileName);
        // string exePath = System.AppDomain.CurrentDomain.BaseDirectory;
        // print("exePath: " + exePath);
        // swcFilePath = Path.Combine(exePath, swcFileName);
        // print("swcFilePath: " + swcFilePath);

        swcFilePath = "F:\\Repos\\Tree_CRDT\\tree_255.swc";
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
        if (swcText == "") {
            print("SWC file read failed");
            return;
        }
        SWC swc = new SWC();
        swc.buildTree(SWCDataStructure.loadSWC(swcText));
        Render(swc);
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
            if (node.pid == -1)
            {
                nodeGameObject.name = "Head-" + node.id;
            }else if (node.children.Count == 0)
            {
                nodeGameObject.name = "End-" + node.id;
            }
            else if (node.children.Count > 1)
            {
                nodeGameObject.name = "Branch-" + node.id;
            }else
            {
                nodeGameObject.name = "Node-" + node.id;
            }
            nodeGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ);
            nodeGameObject.tag = "SWCNode";
            renderedNodeGameObjects.Add(node.id, nodeGameObject);
        }
        
        // render the connections
        // foreach (Node node in swc.indexNodeMap.Values)
        // {
        //     if (node.children.Count > 0)
        //     {
        //         foreach (var child in node.children)
        //         {
        //             string connectionKey = node.id + "-" + child.id;
        //             if (renderedConnections.Contains(connectionKey))
        //             {
        //                 continue;
        //             } else
        //             {
        //                 renderedConnections.Add(connectionKey);
        //             }
        //             // render the connection
        //             GameObject connectionPrefab = Resources.Load<GameObject>("Prefabs/SwcConnection");
        //             GameObject connectionGameObject = Instantiate(connectionPrefab,this.transform);
        //             Vector3 dir = new Vector3(child.relativeX - node.relativeX, child.relativeY - node.relativeY, child.relativeZ - node.relativeZ);
        //             float distance = Vector3.Distance(new Vector3(node.relativeX, node.relativeY, node.relativeZ), new Vector3(child.relativeX, child.relativeY, child.relativeZ));
        //             connectionGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ) + dir / 2;
        //             connectionGameObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);
        //             connectionGameObject.transform.localScale = new Vector3(5.0f, distance/2.0f, 5.0f);
        //             connectionGameObject.tag = "SWCConnection";
        //         }
        //     }
        // }
    }
}
