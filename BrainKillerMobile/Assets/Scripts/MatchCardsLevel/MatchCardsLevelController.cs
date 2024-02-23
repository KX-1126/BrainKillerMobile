using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelLogic;

public class MatchCardsLevelController : LevelControllerBase
{
    public static MatchCardsLevelController Instance;
    public GameObject cardPrefab;
    public GameObject imagesParent;
    public GameObject endCanvas;

    public int TimeUsed; // in seconds
    
    public ModeConfig TestModeConfig = new ModeConfig()
    {
        modeName = "Match Cards",
        modeDescription = "match the cards to win",
        numOfLevels = 50
    };
    
    public matchCardsLevelConfig testLevel = new matchCardsLevelConfig()
    {
        normalConfig = new LevelConfig()
        {
            modeName = "Match Cards",
            levelId = 1,
            levelName = "Match Cards 1",
            levelDescription = "match the cards to win",
        },
        imageNames = new string[] {"front1", "front2", "front3", "front4", "front5", "front6", "front7"},
        showTime = 5,
        numOfRow = 4,
        numOfCol = 3,
    };

    private void Awake()
    {
        Instance = this;
    }

    public override void InitLevel()
    {
        // get config
        matchCardsLevelConfig curLevelConfig = testLevel;
        
        // load images
        List<string> imageNames = new List<string>(curLevelConfig.imageNames);
        
        // generate cards
        
        // start time the game
    }

    public void startLevel()
    {
        // flip the cards for 5 seconds
        
        // flip the cards back
    }
    
    public void JudgeAction()
    {
        
    }
    
    public override void JudgeLevel()
    {
        
    }
    
}
