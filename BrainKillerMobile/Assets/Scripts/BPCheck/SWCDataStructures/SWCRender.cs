using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWCRender : MonoBehaviour
{
    private void Start()
    {
        string testSWCPath = "Assets/Resources/GamesAssets/test.swc";
        SWC swc = new SWC();
        swc.buildTree(SWCDataStructure.loadSWCFromLocalFile(testSWCPath));
        Render(swc);
    }

    public void Render(SWC swc)
    {
        print("Rendering SWC of " + swc.numNodes + " nodes");
        foreach (Node node in swc.indexNodeMap.Values)
        {
            GameObject nodePrefab = Resources.Load<GameObject>("Prefabs/SwcNode");
            GameObject nodeGameObject = Instantiate(nodePrefab,this.transform);
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
        }
        
        // render the connections
        foreach (Node node in swc.indexNodeMap.Values)
        {
            if (node.children.Count > 0)
            {
                foreach (var child in node.children)
                {
                    // render the connection
                    GameObject connectionPrefab = Resources.Load<GameObject>("Prefabs/SwcConnection");
                    GameObject connectionGameObject = Instantiate(connectionPrefab,this.transform);
                    Vector3 dir = new Vector3(child.relativeX - node.relativeX, child.relativeY - node.relativeY, child.relativeZ - node.relativeZ);
                    float distance = Vector3.Distance(new Vector3(node.relativeX, node.relativeY, node.relativeZ), new Vector3(child.relativeX, child.relativeY, child.relativeZ));
                    connectionGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ) + dir / 2;
                    connectionGameObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);
                    connectionGameObject.transform.localScale = new Vector3(5.0f, distance/2.0f, 5.0f);
                }
            }
        }
        
        print("Rendering SWC finished");
    }
}
