using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameMode
{
    Game = 0,
    Science = 1,
    Awards = 2
}

public class TabSwitch : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject sciencePanel;
    public GameObject awardsPanel;
    private List<GameObject> panels;

    public GameObject gameButton;
    public GameObject scienceButton;
    public GameObject awardsButton;
    private List<GameObject> buttons;
    
    private GameObject currentPanel;
    private GameObject currentButton;
    
    private Color activeButtonColor = new Color(1.0f, 0.43f, 0.0f, 1.0f);
    private Color inactiveButtonColor = new Color(0.61f, 0.57f, 0.54f, 1.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        gamePanel.SetActive(true);
        currentPanel = gamePanel;
        buttons = new List<GameObject>() { gameButton, scienceButton, awardsButton };
        panels = new List<GameObject>() { gamePanel, sciencePanel, awardsPanel };
        currentButton = gameButton;
        highLightButton();
    }

    private void highLightButton()
    {
        foreach (var button in buttons)
        {
            if (button == currentButton)
            {
                button.GetComponent<Image>().color = activeButtonColor;
            }
            else
            {
                button.GetComponent<Image>().color = inactiveButtonColor;
            }
        }
    }

    private void activatePanel()
    {
        foreach (var panel in panels)
        {
            if (panel == currentPanel)
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }

    public void SwitchTab(int tab)
    {
        switch (tab)
        {
            case 0:
                currentButton = gameButton;
                currentPanel = gamePanel;
                break;
            case 1:
                currentPanel = sciencePanel;
                currentButton = scienceButton;
                break;
            case 2:
                currentPanel = awardsPanel;
                currentButton = awardsButton;
                break;
        }
        highLightButton();
        activatePanel();
    }
}
