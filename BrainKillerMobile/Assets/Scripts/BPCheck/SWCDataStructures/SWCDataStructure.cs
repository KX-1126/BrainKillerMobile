using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Node
{
    public int id;
    public int type;
    public float x;
    public float y;
    public float z;
    public float relativeX;
    public float relativeY;
    public float relativeZ;
    
    public float radius;
    public int pid; // id of parent node
    public Node parent;
    public List<Node> children = new List<Node>();
}

public class SWC
{
    public string swcName = "normal_swc";
    public Node head = null;
    public Dictionary<int,Node> indexNodeMap = new Dictionary<int, Node>();
    public int numNodes = 0;
    public List<Node> endNodes = new List<Node>();
    public List<Node> branchNodes = new List<Node>();
    public Vector3 center;
    
    public SWC buildTree(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            if (node.pid == -1)
            {
                if (head == null)
                {
                    head = node;
                }
                else
                {
                    Debug.LogError("more than one head node found");
                    return null;
                }
                
            }
            
            indexNodeMap[node.id] = node;
        }
        
        if (head == null)
        {
            Debug.LogError("no head node found");
            return null;
        }

        float sumX = 0.0f;
        float sumY = 0.0f;
        float sumZ = 0.0f;
        
        foreach (Node node in nodes)
        {
            sumX += node.x;
            sumY += node.y;
            sumZ += node.z;
            
            if (node.pid == -1)
            {
                continue;
            }
            
            int pid = node.pid;
            if (indexNodeMap.ContainsKey(pid))
            {
                Node parentNode = indexNodeMap[pid];
                node.parent = parentNode;
                parentNode.children.Add(node);
            }
            else
            {
                Debug.LogError($"parent node with id {pid} not found");
                return null;    
            }
        }
        
        float centerX = sumX / nodes.Count;
        float centerY = sumY / nodes.Count;
        Debug.Log("average center of nodes: " + centerX + " " + centerY);
        center = new Vector2(centerX, centerY);
        
        foreach (Node node in nodes)
        {
            // calculate relative position
            node.relativeX = node.x - head.x;
            node.relativeY = node.y - head.y;
            node.relativeZ = node.z - head.z;
            
            if (node.children.Count > 1)
            {
                if (node.children.Count > 2) // use as normal branch node
                {
                    Debug.Log($"node {node.id} has more than 2 children");
                }
                branchNodes.Add(node);
            }
            else if (node.children.Count == 0)
            {
                endNodes.Add(node);
            }
        }

        numNodes = indexNodeMap.Count;
        Debug.Log($"Successfully build tree of {numNodes} nodes");
        return this;
    }

    public SWC cropFromBp(string name, Node bp, int dimX, int dimY, int dimZ)
    {
        if (branchNodes.Contains(bp) == false)
        {
            Debug.LogError("bp is not a branch node");
            return null;
        }
        
        SWC croppedSWC = new SWC();
        croppedSWC.swcName = name;
        croppedSWC.head = bp;

        // needs deep copy
        foreach (var pair in indexNodeMap)
        {
            Node n = pair.Value;
            if ( Math.Abs(n.relativeX-bp.relativeX) <= dimX/2.0f && 
                 Math.Abs(n.relativeY-bp.relativeY) <= dimY/2.0f && 
                 Math.Abs(n.relativeZ-bp.relativeZ) <= dimZ/2.0f )
            {
                croppedSWC.indexNodeMap[n.id] = n;
                croppedSWC.numNodes += 1;
            }
        }
        
        //remove unvalid parent child relationship
        // foreach (var pair in croppedSWC.indexNodeMap)
        // {
        //     Node n = pair.Value;
        //     if (n.pid == -1)
        //     {
        //         continue;
        //     }
        //     
        //     int pid = n.pid;
        //     if (croppedSWC.indexNodeMap.ContainsKey(pid) == false)
        //     {
        //         n.pid = -2; //mark as crop end
        //     }
        //
        //     foreach (var child in n.children)
        //     {
        //         if (croppedSWC.indexNodeMap.ContainsKey(child.id) == false)
        //         {
        //             n.children.Remove(child);
        //         }
        //     }
        // }

        return croppedSWC;
    }
}

public class SWCDataStructure : MonoBehaviour
{
    public static List<Node> loadSWC(string swcFile)
    {
        string[] lines = swcFile.Split('\n');
        List<Node> nodes = new List<Node>();
        foreach (string line in lines)
        {
            if (line.Length > 0 && line[0] != '#')
            {
                string[] parts = line.Split(' ');
                Node node = new Node();
                node.id = int.Parse(parts[0]);
                node.type = int.Parse(parts[1]);
                node.x = float.Parse(parts[2]);
                node.y = float.Parse(parts[3]);
                node.z = float.Parse(parts[4]);
                node.radius = float.Parse(parts[5]);
                node.pid = int.Parse(parts[6]);
                
                if(node.x == 0 && node.y == 0 && node.z == 0)
                {
                    // Debug.Log("skip node whose position is 0,0,0");
                    continue;
                }
                
                nodes.Add(node);
            }
        }
        return nodes;
    }
    
    public static List<Node> loadSWCFromLocalFile(string swcPath)
    {
        string swcText = System.IO.File.ReadAllText(swcPath);
        return loadSWC(swcText);
    }
}
