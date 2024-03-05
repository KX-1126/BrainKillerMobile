using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeActiveSwcScale : MonoBehaviour
{
    public GameObject swcParent;
    
    public void changeScale(float scale)
    {
        // find active child of swcParent
        foreach (Transform child in swcParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                Vector3 oriScale = child.localScale;
                child.localScale = oriScale * (1+scale);
                print("change scale of " + child.name + " to " + child.localScale);
                break;
            }
        }
    }
}
