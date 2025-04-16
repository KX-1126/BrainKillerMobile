using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DataLoader;
using UnityEngine;

public class testVolumeGeneration : MonoBehaviour
{
    Dataset3D dataset;
    GameObject volumeObject;
    
    // Start is called before the first frame update
    void Start()
    {
        string testImagePath = System.IO.Path.Combine(Application.streamingAssetsPath, "test2.v3draw");        
        Dataset3D dataset = V3dRawDataLoader.readV3dRawFromLocalFile(testImagePath);
        this.dataset = dataset;
        
        volumeObject = VolumeObjectFactory.createVolumeObject(dataset);
        
        volumeObject.transform.parent = this.transform;
        volumeObject.transform.localPosition = Vector3.zero;
        volumeObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        volumeObject.transform.localScale = Vector3.one;
        
    }

    public void reloadImageWithPath(string path)
    {
        // remove child
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        // load new image
        
        this.dataset = V3dRawDataLoader.readV3dRawFromLocalFile(path);
        
        GameObject volumeObject = VolumeObjectFactory.createVolumeObject(dataset);
        
        volumeObject.transform.parent = this.transform;
        volumeObject.transform.localPosition = Vector3.zero;
        volumeObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        volumeObject.transform.localScale = Vector3.one;
    }

    public Collider getChildCollider()
    {
        return transform.GetChild(0).GetChild(0).GetComponent<Collider>();
    }

    public Dataset3D getDataset()
    {
        return dataset;
    }
}
