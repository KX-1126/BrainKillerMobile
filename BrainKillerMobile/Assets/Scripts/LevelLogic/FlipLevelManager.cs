using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelLogic;

public class FlipLevelManager : LevelManager
{
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

    private LevelSelectorManager levelSelectorManager;

    public flipLevelConfig getCurrentLevel()
    {
        return testLevel;
    }

    public int userRecord = 5;

    private void Start()
    {
        levelSelectorManager = transform.Find("LevelSelectorPanel").GetComponent<LevelSelectorManager>();
        
        InitMode("Flip");
        InitLevel(userRecord);
    }

    public override void InitLevel(int levelId)
    {
        
    }
    
    // init mode
    public override void InitMode(string modeName)
    {
        // init level selector
        int numOfLevels = TestModeConfig.numOfLevels;
        levelSelectorManager.InitLevelSelector(userRecord,numOfLevels);
    }
}
