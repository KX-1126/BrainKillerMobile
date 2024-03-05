using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeActiveSwcScale : MonoBehaviour
{
    public GameObject swcParent;
    public Dictionary<GameObject,Vector3> scaleMap = new Dictionary<GameObject, Vector3>();
    
    public void changeScale(float scale)
    {
        // find active child of swcParent
        foreach (Transform child in swcParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (scaleMap.ContainsKey(child.gameObject) == false)
                {
                    scaleMap.Add(child.gameObject, child.localScale);
                }
                
                Vector3 oriScale = scaleMap[child.gameObject];
                child.localScale = oriScale * (1+scale);
                print("change scale of " + child.name + " to " + child.localScale);
                break;
            }
        }
    }
}
