using System;
using System.Collections;
using System.Collections.Generic;
using LevelLogic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utilities;

public enum ImageDetectiveLevelState
{
    Init,
    Play,
    End
}

public class ImageDetectiveLevelController : LevelControllerBase
{
    public GameObject imageLoader;
    public GameObject imagesParent;
    public GameObject endCanvas;
    public GameObject timer;
    public GameObject simpleCardPrefab;

    public int TimeUsed = 0; // in seconds
    private float gameStartTime;

    public ModeConfig TestModeConfig = new ModeConfig()
    {
        modeName = "Image Detective",
        modeDescription = "Find the differences to win",
        numOfLevels = 50
    };

    public imageDetectiveLevelConfig levelConfig = new imageDetectiveLevelConfig()
    {
        normalConfig = new LevelConfig()
        {
            modeName = "Match Cards",
            levelId = 1,
            levelName = "Match Cards 1",
            levelDescription = "match the cards to win",
        },
        imageNames = new string[] { "0", "1" },
        numOfRow = 7,
        numOfCol = 6,
        timeLimit = 90
    };

    private imageDetectiveLevelConfig curLevelConfig;
    
    private ImageDetectiveLevelState gameState = ImageDetectiveLevelState.Init;

    private string diffObjectName;

    public override void InitLevel()
    {
        // get level config
        curLevelConfig = levelConfig;
        
        // load images
        List<Sprite> images = imageLoader.GetComponent<ImageDetectiveImageLoader>().LoadImages(curLevelConfig.imageNames);
        if (images == null)
        {
            Debug.LogError("Image load failed");
            return;
        }

        if (images.Count < 2)
        {
            Debug.LogError("Not enough images loaded");
            return;
        }
        
        print($"successfully loaded {images.Count} images");
        
        // choose the different index
        int diffIndex = UnityEngine.Random.Range(0, curLevelConfig.numOfRow * curLevelConfig.numOfCol);
        
        // fill image
        for (int i = 0; i < curLevelConfig.numOfRow * curLevelConfig.numOfCol; i++)
        {
            GameObject card = Instantiate(simpleCardPrefab, imagesParent.transform);
            card.name = "SimpleCard-" + i;
            SimpleCardController controller = card.GetComponent<SimpleCardController>();
            controller.levelController = this.gameObject;

            if (i == diffIndex)
            {
                controller.setSprite(images[1]);
                diffObjectName = card.name;
            }
            else
            {
                controller.setSprite(images[0]);
            }
        }
        
        Assert.IsTrue(diffObjectName != null, "diffObjectName is null");
        
        // adjust size and layout
        GridLayoutGroup gridLayoutGroup = imagesParent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = curLevelConfig.numOfCol;
        gridLayoutGroup.cellSize = GridLayoutSizeCalculator.CalculateGridMinSquareSize(
            imagesParent.GetComponent<RectTransform>().rect.size, 
            curLevelConfig.numOfCol, 
            curLevelConfig.numOfRow,
                10,
                    10);
        
        gameState = ImageDetectiveLevelState.Play;
        gameStartTime = Time.time;
    }

    private void Start()
    {
        InitLevel();
    }

    public void handleClick(string name)
    {
        print(name + " clicked");
        if (name == diffObjectName)
        {
            showEndCanvas(true);
        }
    }

    private void Update()
    {
        if (gameState == ImageDetectiveLevelState.Play)
        {
            timer.GetComponent<TextMeshProUGUI>().color = Color.white;
            TimeUsed = (int) (Time.time - gameStartTime);

            if (curLevelConfig.timeLimit <= TimeUsed)
            {
                showEndCanvas(false); // lose time out
            }
            
            setTimerText(levelConfig.timeLimit - TimeUsed);
        }
    }

    public void setTimerText(int time)
    {
        string timeString = time.ToString("D2");
        timer.GetComponent<TextMeshProUGUI>().text = timeString;
        
        // set color to red if time is more than 60
        if (time < levelConfig.timeLimit * 0.2)
        {
            timer.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }
    
    private void showEndCanvas(bool result)
    {
        endCanvas.SetActive(true);
        EndCanvasController endCanvasController = endCanvas.GetComponent<EndCanvasController>();
        endCanvasController.setResult(result);
        gameState = ImageDetectiveLevelState.End;
    }

}
