using System;
using System.Collections;
using System.Collections.Generic;
using LevelLogic;
using UnityEngine;
using UnityEngine.UI;

public class FlipLevelController : MonoBehaviour
{
    public GameObject ImageItemPrefab;
    public GameObject imagesParent;
    
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

    private void Start()
    {
        InitLevel();
    }
    
    

    public void InitLevel(){
        // parse level config
        flipLevelConfig curLevelConfig = testLevel;
        
        // load images
        FlipImageLoader loader = transform.Find("ImageLoader").GetComponent<FlipImageLoader>();
        
        // split images
        (List<Sprite>,List<Sprite>) imageTuples = loader.getSplitedSprites(curLevelConfig.numOfItem);
        
        // create a grid of images
        for (int i = 0; i < curLevelConfig.numOfItem; i++)
        {
            GameObject imageItem = Instantiate(ImageItemPrefab, transform);
            RectTransform rectTransform = imageItem.GetComponent<RectTransform>();
            
            
            // get cell size
            GridLayoutGroup layoutGroup = transform.Find("Images").GetComponent<GridLayoutGroup>();
            Vector2 cellSize = layoutGroup.cellSize;
            print("cell size: " + cellSize);
            imageItem.GetComponent<BoxCollider2D>().size = new Vector2(cellSize.x, cellSize.y);
            print("collider size: " + imageItem.GetComponent<BoxCollider2D>().bounds);
            imageItem.transform.SetParent(imagesParent.transform);
            imageItem.transform.GetComponent<ImageGridController>().setSprites(imageTuples.Item1[i], imageTuples.Item2[i]);
        }
    }

    public void StartLevel(){
        // mix the levels up
    }

    public void JudgeLevel(){
        // judge the level
    }

}
