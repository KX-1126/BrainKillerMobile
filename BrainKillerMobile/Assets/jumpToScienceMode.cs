using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpToScienceMode : MonoBehaviour
{
    public void jumpToMode(int mode)
    {
        switch (mode)
        {
            case 1:
                Debug.Log("jump to bp check mode");
                SceneManager.LoadScene("BPCheck");
                break;
            default:
                Debug.LogError("no unknown bp mode");
                break;
        } 
    }
}
