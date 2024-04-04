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
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(levelController.retry);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(levelController.returnToLobby);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(levelController.nextLevel);
        }
        
        if (result)
        {
            title.text = "You Win!";
            retryButton.gameObject.SetActive(false);
        }
        else
        {
            title.text = "You Fail!";
            nextButton.gameObject.SetActive(false);
        }
    }
}
