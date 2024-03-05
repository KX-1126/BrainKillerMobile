using System;
using System.Collections;
using System.Collections.Generic;
using DataLoader;
using UnityEngine;

public class CropWithSwcBp : MonoBehaviour
{
    public GameObject swcParent;
    public GameObject imageParent;
    public float filterBpDistance = 5.0f; //filter out bp with distance to center
    
    private SWC swc = null;
    private Dataset3D dataset = null;
    
    private List<Tuple<Dataset3D, SWC>> croppedData = new List<Tuple<Dataset3D, SWC>>();
    private int croppedDataIndex = 0;

    private bool getData()
    {
        swc = swcParent.GetComponent<SWCRender>().swc;
        
        dataset = imageParent.GetComponent<VolumeGenerator>().currentDataset;
        
        if (swc == null || dataset == null)
        {
            Debug.LogError("SWC or dataset is null");
            return false;
        }
        
        print("dateset name: " + dataset.datasetName);
        print("dataset size: " + dataset.dimX + " " + dataset.dimY + " " + dataset.dimZ);

        return true;
    }

    [ContextMenu("Crop Data")]
    private void cropAction()
    {
        crop(30);
        print("crop finished");
    }
    
    
    private void crop(int dim)
    {
        if (getData() != true)
        {
            print("Data not ready");
            return;
        }
        
        // get centers(bp point)
        List<Vector3> bpPoints = new List<Vector3>();
        croppedData = new List<Tuple<Dataset3D, SWC>>();
        int count = 0;
        
        foreach (var node in swc.branchNodes)
        {
            Vector3 curBpPos = new Vector3(node.relativeX, node.relativeY, node.relativeZ);
            Vector3 swcHead = new Vector3(swc.head.relativeX, swc.head.relativeY, swc.head.relativeZ);
            if (Vector3.Distance(curBpPos, swcHead) < filterBpDistance)
            {
                continue;
            }
            
            count += 1;
            Vector3 offset = new Vector3(dataset.dimX/2, dataset.dimY/2, dataset.dimZ/2);
            Vector3 bpPoint = new Vector3(node.relativeX, node.relativeY, node.relativeZ) + offset;
            bpPoints.Add(bpPoint);
            
            // crop dataset
            Vector3 intBpPos = new Vector3(Mathf.RoundToInt(bpPoint.x), Mathf.RoundToInt(bpPoint.y), Mathf.RoundToInt(bpPoint.z));
            Dataset3D croppedDataset = new Dataset3D();
            croppedDataset = dataset.crop(dataset.datasetName + "_cropped_" + count.ToString(), dim, dim, dim, intBpPos);
            
            // crop swc
            // check for bp and center distance
            
            SWC croppedSwc = new SWC();
            croppedSwc = swc.cropFromBp("cropped_swc_" + count.ToString(), node, dim, dim, dim);
            
            // add to list
            croppedData.Add(new Tuple<Dataset3D, SWC>(croppedDataset, croppedSwc));
            croppedDataIndex = 0;
        }
        
        print($"total {count} cropped data: ");
    }
    
    [ContextMenu("Render cropped data")]
    private void renderCroppedData()
    {
        if (croppedData.Count == 0)
        {
            print("No cropped data");
            return;
        }
        
        if (croppedDataIndex >= croppedData.Count)
        {
            croppedDataIndex = 0;
        }
        
        print("cropped data index: " + croppedDataIndex);
        var cropped = croppedData[croppedDataIndex];
        
        // check for cropped data
        Dataset3D dataset = cropped.Item1;
        SWC swc = cropped.Item2;
        
        print(dataset.datasetName + " has dim: " + dataset.dimX + " " + dataset.dimY + " " + dataset.dimZ);
        print(dataset.datasetName + " has data length: " + dataset.data.Length);
        
        print(swc.swcName + " has numNodes: " + swc.numNodes);
        
        
        VolumeGenerator vg = imageParent.GetComponent<VolumeGenerator>();
        vg.GenerateVolumeObject(dataset);
        
        swcParent.GetComponent<SWCRender>().Render(swc);
        croppedDataIndex += 1;
        
        print("finished rendering cropped data with index " + croppedDataIndex);
    }
    
    
}
