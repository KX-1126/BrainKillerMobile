using System.Collections;
using System.Collections.Generic;
using System.Data;
using DataLoader;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class VolumeGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public Dataset3D originalDataset;
    public Dataset3D currentDataset;
    public float moveInDistance = 5.0f;
    
    void Start()
    {
        // string testImagePath = "Assets/Resources/GamesAssets/test2.v3draw";
        // currentDataset = V3dRawDataLoader.readV3dRawFromLocalFile(testImagePath);
        //
        // GenerateVolumeObject(currentDataset);
    }
    
    public void GenerateVolumeObject(Dataset3D dataset)
    {
        //reset rotation
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        // Move the currently active child to an off-screen position on the left
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                
                StartCoroutine(moveObject.MoveOverTime(child.gameObject, new Vector3(-moveInDistance, 0, 0), 2.0f,true)); // Replace -1000 with the appropriate value to move the object off-screen
            }
        }

        currentDataset = dataset;
        GameObject volumeObject = VolumeObjectFactory.createVolumeObject(currentDataset);

        volumeObject.transform.parent = this.transform;
        volumeObject.transform.localPosition = new Vector3(moveInDistance, 0, 0); // Start from an off-screen position on the right
        volumeObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        volumeObject.transform.localScale = Vector3.one;

        // Move the volumeObject from the right to the center of the screen
        StartCoroutine(moveObject.MoveOverTime(volumeObject, Vector3.zero, 2.0f, false)); // Replace -1000 with the appropriate value to move the object to the center of the screen
    }
    
    
}
