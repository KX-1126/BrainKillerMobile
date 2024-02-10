using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndCanvasController : MonoBehaviour
{
    public TextMeshProUGUI title;
    public Button button1;
    public Button button2;
    
    public void setResult(bool result)
    {
        if (result)
        {
            title.text = "You Win!";
            button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Next Level";
            button1.onClick.AddListener(FlipLevelController.Instance.NextLevel);
            button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Leave Game";
        }
        else
        {
            title.text = "You Fail!";
            button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Retry";
            button1.onClick.AddListener(FlipLevelController.Instance.Retry);
            button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Leave Game";
        }
        button2.onClick.AddListener(FlipLevelController.Instance.BackToMenu);
    }
}
