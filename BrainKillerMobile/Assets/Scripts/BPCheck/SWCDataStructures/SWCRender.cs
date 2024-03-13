using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SWCRender : MonoBehaviour
{
    public readonly SWC swc = new SWC();
    public int basicScale = 5;
    public float moveInDistance = 2500;
    
    private void Start()
    {
        // string testSWCPath = "Assets/Resources/GamesAssets/test2.swc";
        // swc.buildTree(SWCDataStructure.loadSWCFromLocalFile(testSWCPath));
        // Render(swc);
    }

    public void Render(SWC newswc)
    {
        //reset rotation
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        // disable old swc parent object
        foreach (Transform child in transform)
        {
            StartCoroutine(moveObject.MoveOverTime(child.gameObject, new Vector3(-moveInDistance, 0, 0), 2.0f, true));
        }
        
        // create empty parent
        GameObject parent = new GameObject(newswc.swcName);
        parent.transform.SetParent(this.transform);
        parent.transform.localScale = Vector3.one * 6;
        parent.transform.localPosition = new Vector3(moveInDistance, 0, 0);
        
        print("Rendering SWC of " + newswc.numNodes + " nodes");
        foreach (Node node in newswc.indexNodeMap.Values)
        {
            if (node == newswc.head)
            {
                GameObject headNodePrefab = Resources.Load<GameObject>("Prefabs/ImportantSwcNode");
                headNodePrefab.transform.localScale = Vector3.one * basicScale * 2;
                GameObject newNodeGameObject = Instantiate(headNodePrefab,parent.transform);
                newNodeGameObject.name = "Head-" + node.id;
                newNodeGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ);
                continue;
            }
            
            GameObject nodePrefab = Resources.Load<GameObject>("Prefabs/SwcNode");
            nodePrefab.transform.localScale = Vector3.one * basicScale;
            GameObject nodeGameObject = Instantiate(nodePrefab,parent.transform);
            if (node.children.Count == 0)
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
        foreach (Node node in newswc.indexNodeMap.Values)
        {
            if (node.children.Count > 0)
            {
                foreach (var child in node.children)
                {
                    if (swc.indexNodeMap.ContainsKey(child.id) == false) //skip the node that is not in the swc(maybe cropped swc)
                    {
                        continue;
                    }
                    // render the connection
                    GameObject connectionPrefab = Resources.Load<GameObject>("Prefabs/SwcConnection");
                    GameObject connectionGameObject = Instantiate(connectionPrefab,parent.transform);
                    Vector3 dir = new Vector3(child.relativeX - node.relativeX, child.relativeY - node.relativeY, child.relativeZ - node.relativeZ);
                    float distance = Vector3.Distance(new Vector3(node.relativeX, node.relativeY, node.relativeZ), new Vector3(child.relativeX, child.relativeY, child.relativeZ));
                    connectionGameObject.transform.localPosition = new Vector3(node.relativeX, node.relativeY, node.relativeZ) + dir / 2;
                    connectionGameObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);
                    connectionGameObject.transform.localScale = new Vector3(basicScale, distance/2.0f, basicScale);
                }
            }
        }
        
        print("Rendering SWC finished");
        
        StartCoroutine(moveObject.MoveOverTime(parent, Vector3.zero, 2.0f, false)); 
    }
    
}
