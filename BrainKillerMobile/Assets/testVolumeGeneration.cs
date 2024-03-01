using System.Collections;
using System.Collections.Generic;
using DataLoader;
using UnityEngine;

public class testVolumeGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string testImagePath = "Assets/Resources/GamesAssets/test.v3draw";
        Dataset3D dataset = V3dRawDataLoader.readV3dRawFromLocalFile(testImagePath);
        
        GameObject volumeObject = VolumeObjectFactory.createVolumeObject(dataset);
        
        volumeObject.transform.parent = this.transform;
        volumeObject.transform.localPosition = Vector3.zero;
        volumeObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        volumeObject.transform.localScale = Vector3.one * 500;
        
    }
}
