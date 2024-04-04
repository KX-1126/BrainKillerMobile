using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using LevelLogic;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Utilities;

public class FlipLevelController : LevelControllerBase
{
    public GameObject ImageItemPrefab;
    public GameObject imagesParent;
    public GameObject endCanvas;
    public TextMeshProUGUI title;

    private int MAX_TRY_COUNT;
    private int triedCount;
    private bool startJudge = false;
    
    // static instance
    public static FlipLevelController Instance;

    private ModeConfig modeConfig;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitMode();
        InitLevel();
    }

    public async void InitMode()
    {
        RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.GET_MODE_CONFIG + "/flip/");
        if (!result.Success)
        {
            Debug.LogError("fail to get mode config");
            return;
        }
        string modeConfigJson = result.Data;
        modeConfig = JsonUtility.FromJson<ModeConfig>(modeConfigJson);
        print("Init mode:" + modeConfig.modeName);
    }

    public async override void InitLevel(){
        int flipLevelProgress = UserInfoCache.getUserProfile().flipProgress;
        string mode = "flip";
        title.text = $"Level - {flipLevelProgress}";
        string url = NetworkURL.GET_LEVELCONFIG + $"/{mode}/{flipLevelProgress}/";
        RequestResult<string> result = await NetworkRequest.PostRequest(url, "");
        if (!result.Success)
        {
            Debug.LogError("fail to get level config");
            print("error message:" + result.ErrorMessage);
            return;
        }
        
        string flipLevelConfigString = result.Data;
        Debug.Log("get config json:" + flipLevelConfigString);
        
        // parse level config
        flipLevelConfig curLevelConfig = JsonUtility.FromJson<flipLevelConfig>(flipLevelConfigString); 
        // print(curLevelConfig.normalConfig.levelDescription);
        // print(curLevelConfig.frontImageName);
        // print(curLevelConfig.backImageName);
        
        // set max try count
        MAX_TRY_COUNT = curLevelConfig.numOfItem * 2;
        
        // load images
        FlipImageLoader loader = transform.Find("ImageLoader").GetComponent<FlipImageLoader>();
        bool success = await loader.loadImage(curLevelConfig.frontImageName, curLevelConfig.backImageName); // save to loader
        if (!success)
        {
            Debug.LogError("fail to load images");
            return;
        }
        
        // split images
        (List<Sprite>,List<Sprite>) imageTuples = loader.getSplitedSprites(curLevelConfig.numOfItem);
        
        // clear old images
        for (int i = imagesParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        // create a grid of images
        for (int i = 0; i < curLevelConfig.numOfItem; i++)
        {
            GameObject imageItem = Instantiate(ImageItemPrefab, imagesParent.transform);
            imageItem.name = "ImageItem-" + i;
            
            imageItem.GetComponent<ImageGridController>().FlipCompleted += handleFlipCompleted;
            imageItem.transform.GetComponent<ImageGridController>().setSprites(imageTuples.Item1[i], imageTuples.Item2[i]);
        }
        
        // adjust cell size
        GridLayoutGroup gridLayoutGroup = imagesParent.GetComponent<GridLayoutGroup>();
        int rowCount = (int)Mathf.Sqrt(curLevelConfig.numOfItem);
        gridLayoutGroup.constraintCount = rowCount;
        gridLayoutGroup.cellSize = GridLayoutSizeCalculator.CalculateGridMinSquareSize(
            imagesParent.GetComponent<RectTransform>().rect.size, 
            rowCount, 
            rowCount, 
            20, 
            0);
        print("adjust size to " + gridLayoutGroup.cellSize);
        
        // mix images
        Invoke(nameof(StartLevel), 1.0f);
    }
    
    private void handleFlipCompleted(object sender, EventArgs e){
        if (startJudge)
        {
            addTryCount();
            JudgeLevelResult();
        }
    }

    public void StartLevel(){
        // mix the levels up
        for (int i = 0; i < imagesParent.transform.childCount; i++)
        {
            GameObject imageItem = imagesParent.transform.GetChild(i).gameObject;
            int randFlipCount = Random.Range(1, 4);
            imageItem.GetComponent<ImageGridController>().StartFlip(0.5f, Random.Range(1, 4), false);
            // print("flip " + imageItem.name + " " + randFlipCount + " times");
        }
        Invoke(nameof(startJudgeLevel), 5 * 0.5f);   
    }
    
    private void startJudgeLevel(){
        startJudge = true;
    }
    
    [ContextMenu("Judge Level")]
    public bool JudgeLevelResult(){
        // judge the level
        List<bool> states = new List<bool>();
        for (int i = 0; i < imagesParent.transform.childCount; i++)
        {
            GameObject imageItem = imagesParent.transform.GetChild(i).gameObject;
            states.Add(imageItem.GetComponent<ImageGridController>().isFlipped);
        }
        
        //print states in sqrt(rows)
        int sqrt = (int)Mathf.Sqrt(states.Count);
        print(sqrt);
        string allRows = "";
        for (int i = 0; i < sqrt; i++)
        {
            string row = "";
            for (int j = 0; j < sqrt; j++)
            {
                row += states[i * sqrt + j] + " ";
            }
            allRows += row + "\n";
        }
        print(allRows);
        
        // check if all the tiles are the same
        bool isAllSame = true;
        for (int i = 1; i < states.Count; i++)
        {
            if (states[i] != states[i - 1])
            {
                isAllSame = false;
                break;
            }
        }
        
        print("Judge Result: " + isAllSame);
        if (isAllSame) // show panel
        {
            showEndCanvas(true);
        }
        
        return isAllSame;
    }

    public void addTryCount()
    {
        triedCount++;
        if (triedCount >= MAX_TRY_COUNT)
        {
            showEndCanvas(false);
        }
    }

    public void showEndCanvas(bool result)
    {
        endCanvas.SetActive(true);
        EndCanvasController endCanvasController = endCanvas.GetComponent<EndCanvasController>();
        endCanvasController.setResult(result);
    }
    
    public async override void nextLevel()
    {
        // update user profile
        User newUser = UserInfoCache.getUserProfile().DeepCopy();
        newUser.flipProgress += 1;
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
