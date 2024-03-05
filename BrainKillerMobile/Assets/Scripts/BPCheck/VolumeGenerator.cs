using System.Collections;
using System.Collections.Generic;
using System.Data;
using DataLoader;
using UnityEngine;

public class VolumeGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public Dataset3D currentDataset;
    
    void Start()
    {
        string testImagePath = "Assets/Resources/GamesAssets/test2.v3draw";
        currentDataset = V3dRawDataLoader.readV3dRawFromLocalFile(testImagePath);
        
        GenerateVolumeObject(currentDataset);
    }
    
    public void GenerateVolumeObject(Dataset3D dataset)
    {
        // disable old volume object
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        
        currentDataset = dataset;
        GameObject volumeObject = VolumeObjectFactory.createVolumeObject(currentDataset);
        
        volumeObject.transform.parent = this.transform;
        volumeObject.transform.localPosition = Vector3.zero;
        volumeObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        volumeObject.transform.localScale = Vector3.one;
    }
}
