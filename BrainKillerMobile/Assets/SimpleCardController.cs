using UnityEngine;
using UnityEngine.UI;

public class SimpleCardController : MonoBehaviour
{
    public GameObject levelController;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(notifyLevelController);
    }

    public void setSprite(Sprite s)
    {
        if (s == null)
        {
            Debug.LogError("sprite is null");
            return;
        }
        GetComponent<Image>().sprite = s;
    }
    
    private void notifyLevelController()
    {
        levelController.GetComponent<ImageDetectiveLevelController>().handleClick(this.gameObject.name);
    }
}
