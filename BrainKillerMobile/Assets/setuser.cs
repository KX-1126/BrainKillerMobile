using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setuser : MonoBehaviour
{
    public Text userid;
    public Image usercolor;

    public void setUserColor(string id, Color color) {
        if (id == DataManager.Instance.userId.ToString()) {
            userid.text = id + "(You)";
        } else {
            userid.text = id;
        }
        usercolor.color = color;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
