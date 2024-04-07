using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using LevelLogic;
using Network;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
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

    private ModeConfig curModeConfig;
    private imageDetectiveLevelConfig curLevelConfig;
    
    private ImageDetectiveLevelState gameState = ImageDetectiveLevelState.Init;

    private string diffObjectName;
    
    public async void InitMode()
    {
        RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.GET_MODE_CONFIG + "/imageDetective/");
        if (!result.Success)
        {
            Debug.LogError("fail to get mode config");
            return;
        }
        string modeConfigJson = result.Data;
        curModeConfig = JsonUtility.FromJson<ModeConfig>(modeConfigJson);
        print("Init mode:" + curModeConfig.modeName);
    }

    public async override void InitLevel()
    {
        // get level config
        int imageDetectiveProgress = UserInfoCache.getUserProfile().imageDetectiveProgress;
        string mode = "imageDetective";
        string url = NetworkURL.GET_LEVEL_CONFIG + $"/{mode}/{imageDetectiveProgress}/";
        RequestResult<string> result = await NetworkRequest.PostRequest(url, "");
        if (!result.Success)
        {
            Debug.LogError("Get level config failed");
            print("error message:" + result.ErrorMessage);
            return;
        }
        // print(result.Data);
        
        curLevelConfig = JsonUtility.FromJson<imageDetectiveLevelConfig>(result.Data);
        // print(curLevelConfig.normalConfig.levelId);
        // print(curLevelConfig.imageNames);
        // print(curLevelConfig.numOfRow);
        
        // load images
        
        bool downloadResult =
            await imageLoader.GetComponent<ImageDetectiveImageLoader>().LoadImages(curLevelConfig.imageNames.Split(","));
        if (!downloadResult)
        {
            Debug.LogError("Image load failed");
            return;
        }
        
        List<Sprite> images = imageLoader.GetComponent<ImageDetectiveImageLoader>().GetImages();
        if (images == null)
        {
            Debug.LogError("Get all loaded images failed");
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
        
        // clear old images
        for (int i = imagesParent.transform.childCount-1; i >= 0 ; i--)
        {
            Destroy(imagesParent.transform.GetChild(i).gameObject);
        }
        
        
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
        InitMode();
        InitLevel();
    }

    public void handleClick(string name)
    {
        print(name + " clicked");
        if (name == diffObjectName)
        {
            showEndCanvas(true);
        }
        else
        {
            showEndCanvas(false);
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
            
            setTimerText(curLevelConfig.timeLimit - TimeUsed);
        }
    }

    public void setTimerText(int time)
    {
        string timeString = time.ToString("D2");
        timer.GetComponent<TextMeshProUGUI>().text = timeString;
        
        // set color to red if time is more than 60
        if (time < curLevelConfig.timeLimit * 0.2)
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

    public override void retry()
    {
        endCanvas.SetActive(false);
        InitLevel();
    }

    public async override void nextLevel()
    {
        // update user profile
        User newUser = UserInfoCache.getUserProfile().DeepCopy();
        newUser.imageDetectiveProgress += 1;
        string jsonBody = JsonUtility.ToJson(newUser);
        print("update user profile:" + jsonBody);
        RequestResult<string> result = await NetworkRequest.PostRequest(NetworkURL.USER_PROFILE, jsonBody);
        if (!result.Success)
        {
            Debug.LogError("fail to update user profile");
            print("error message:" + result.ErrorMessage);
            return;
        }
        print("update flip progress feedback:" + result.Data);
        
        // refresh user info cache
        User user = JsonUtility.FromJson<User>(result.Data);
        UserInfoCache.setUserProfile(user);
        
        // get next level config
        InitLevel();
        
        // hide end canvas
        endCanvas.SetActive(false);
    }
    

}
