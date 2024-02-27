using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndCanvasController : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject levelControllerGameObject;
    public Button retryButton;
    public Button nextButton;
    public Button quitButton;
    
    public void setResult(bool result)  
    {
        //get level base
        LevelControllerBase levelController = this.levelControllerGameObject.GetComponent<LevelControllerBase>();
        
        //set button action
        retryButton.onClick.AddListener(levelController.retry);
        quitButton.onClick.AddListener(levelController.returnToLobby);
        nextButton.onClick.AddListener(levelController.nextLevel);
        
        if (result)
        {
            title.text = "You Win!";
        }
        else
        {
            title.text = "You Fail!";
            nextButton.gameObject.SetActive(false);
        }
    }
}
