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
    public LevelConfig testLevel = new LevelConfig()
    {
        modeName = "Flip",
        levelId = 1,
        levelName = "Flip 1",
        levelDescription = "Flip the tiles to match the pattern"
    };

    public int userRecord = 5;
    
    // public LevelSelectorManager levelSelectorManager = GameObject.Find("LevelSelectorPanel").GetComponent<LevelSelectorManager>();

    private void Start()
    {
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
        // levelSelectorManager.InitLevelSelector(userRecord,numOfLevels);
    }
}
