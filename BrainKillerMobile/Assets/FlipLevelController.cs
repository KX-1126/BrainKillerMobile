using System;
using System.Collections;
using System.Collections.Generic;
using LevelLogic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FlipLevelController : MonoBehaviour
{
    public GameObject ImageItemPrefab;
    public GameObject imagesParent;
    public GameObject endCanvas;

    private int MAX_TRY_COUNT;
    private int triedCount;
    
    // static instance
    public static FlipLevelController Instance;
    
    public ModeConfig TestModeConfig = new ModeConfig()
    {
        modeName = "Flip",
        modeDescription = "Flip the tiles to match the pattern",
        numOfLevels = 50
    };
    
    public flipLevelConfig testLevel = new flipLevelConfig()
    {
        normalConfig = new LevelConfig()
        {
            modeName = "Flip",
            levelId = 1,
            levelName = "Flip 1",
            levelDescription = "Flip the tiles to match the pattern",
        },
        fullImgFrontName = "front1",
        fullImgBackName = "front2",
        numOfItem = 16,
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitLevel();
    }
    

    public void InitLevel(){
        // parse level config
        flipLevelConfig curLevelConfig = testLevel;
        
        // set max try count
        MAX_TRY_COUNT = curLevelConfig.numOfItem * 2;
        
        // load images
        FlipImageLoader loader = transform.Find("ImageLoader").GetComponent<FlipImageLoader>();
        
        // split images
        (List<Sprite>,List<Sprite>) imageTuples = loader.getSplitedSprites(curLevelConfig.numOfItem);
        
        // create a grid of images
        for (int i = 0; i < curLevelConfig.numOfItem; i++)
        {
            GameObject imageItem = Instantiate(ImageItemPrefab, transform);
            imageItem.name = "ImageItem-" + i;
            
            // get cell size
            GridLayoutGroup layoutGroup = transform.Find("Images").GetComponent<GridLayoutGroup>();
            Vector2 cellSize = layoutGroup.cellSize;
            imageItem.GetComponent<BoxCollider2D>().size = new Vector2(cellSize.x, cellSize.y);
            imageItem.transform.SetParent(imagesParent.transform);
            imageItem.transform.GetComponent<ImageGridController>().setSprites(imageTuples.Item1[i], imageTuples.Item2[i]);
        }
        
        // mix images
        Invoke(nameof(StartLevel), 1.0f);
    }

    public void StartLevel(){
        // mix the levels up
        for (int i = 0; i < imagesParent.transform.childCount; i++)
        {
            GameObject imageItem = imagesParent.transform.GetChild(i).gameObject;
            int randFlipCount = Random.Range(1, 4);
            imageItem.GetComponent<ImageGridController>().StartFlip(0.5f, Random.Range(1, 4), false);
            print("flip " + imageItem.name + " " + randFlipCount + " times");
        }
    }
    
    [ContextMenu("Judge Level")]
    public bool JudgeLevel(){
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
    
    public void NextLevel()
    {
        print("Next Level");
    }
    
    public void Retry()
    {
        print("Retry");
    }
    
    public void BackToMenu()
    {
        print("Back to Menu");
    }

}
