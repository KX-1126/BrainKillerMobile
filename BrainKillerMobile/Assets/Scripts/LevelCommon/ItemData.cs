using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public void setItemLevelNumber(int level)
    {
        GameObject leveName = transform.Find("levelName").gameObject;
        leveName.GetComponent<TextMeshProUGUI>().text = level.ToString();
    }
}
