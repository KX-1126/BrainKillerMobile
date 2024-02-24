using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelLogic;
using TMPro;
using UnityEngine.UI;

public enum MatchCardsGameState
{
    Init,
    Start,
    Play,
    End
}

public class MatchCardsLevelController : LevelControllerBase
{
    public static MatchCardsLevelController Instance;
    public GameObject cardPrefab;
    public GameObject imagesParent;
    public GameObject endCanvas;
    public GameObject imageLoader;
    public GameObject timer;

    public int TimeUsed = 0; // in seconds
    private float gameStartTime;
    private int MAX_TIME = 90;
    
    private List<ImageGridController> matchedCards = new List<ImageGridController>();
    private List<ImageGridController> flipedCards = new List<ImageGridController>();
    
    public MatchCardsGameState gameState = MatchCardsGameState.Init;
    
    matchCardsLevelConfig curLevelConfig;
    
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
        imageNames = new string[] {"0", "1", "2"},
        showTime = 5,
        numOfRow = 3,
        numOfCol = 2,
    };

    private void Awake()
    {
        Instance = this;
        TimeUsed = 0;
    }

    private void Start()
    {
        InitLevel();
    }

    public override void InitLevel()
    {
        // get config
        curLevelConfig = testLevel;
        
        // check for config
        if (!curLevelConfig.selfCheck())
        {
            Debug.LogError("Match cards Level config is not correct");
            return;
        }
        
        // load images
        List<string> imageNames = new List<string>(curLevelConfig.imageNames);
        MatchCardsImageLoader loader = imageLoader.GetComponent<MatchCardsImageLoader>();
        List<Sprite> images = loader.LoadImages(imageNames);
        if (images == null)
        {
            Debug.LogError("Load images failed");
            return;
        }
        print($"successfully load {images.Count} images");

        // generate cards
        imagesParent.GetComponent<GridLayoutGroup>().constraintCount = curLevelConfig.numOfCol;
        
        // copy the images
        images.AddRange(images);
        
        // shuffle images
        for (int i = 0; i < images.Count; i++)
        {
            Sprite temp = images[i];
            int randomIndex = UnityEngine.Random.Range(i, images.Count);
            images[i] = images[randomIndex];
            images[randomIndex] = temp;
        }

        Sprite backImage = Resources.Load<Sprite>("GamesAssets/matchCardsBack");
        if (backImage == null)
        {
            Debug.LogError("Load back image failed");
            return;
        }
        
        for (int i = 0; i < curLevelConfig.numOfRow * curLevelConfig.numOfCol; i++)
        {
            GameObject card = Instantiate(cardPrefab, imagesParent.transform);
            
            ImageGridController controller = card.GetComponent<ImageGridController>();
            controller.FlipCompleted += handleFlip;
            
            // random select image
            controller.setSprites(backImage, images[i]);
        }
        gameStartTime = Time.time;
        
        gameState = MatchCardsGameState.Start;
        DelayAction(1.0f,startLevel);
    }

    private void startLevel()
    {
        // flip the cards
        foreach (Transform child in imagesParent.transform)
        {
            ImageGridController controller = child.GetComponent<ImageGridController>();
            controller.StartFlip(0.8f, 1, false);
        }
        
        // flip the cards back
        DelayAction(5.0f, () =>
        {
            foreach (Transform child in imagesParent.transform)
            {
                ImageGridController controller = child.GetComponent<ImageGridController>();
                controller.StartFlip(0.8f, 1, false);
            }
        });
    }

    private void Update()
    {
        if (gameState == MatchCardsGameState.Start)
        {
            timer.GetComponent<TextMeshProUGUI>().color = new Color32(28, 125, 44, 255);
            TimeUsed = (int) (Time.time - gameStartTime);
            setTimerText(curLevelConfig.showTime - TimeUsed);
            if (TimeUsed > curLevelConfig.showTime)
            {
                gameState = MatchCardsGameState.Play;
                TimeUsed = 0;
                gameStartTime = Time.time;
            }
        }else if (gameState == MatchCardsGameState.Play)
        {
            // update used time
            
            timer.GetComponent<TextMeshProUGUI>().color = Color.white;
            TimeUsed = (int) (Time.time - gameStartTime);
            setTimerText(TimeUsed);
        }
        
        
        // check if the game is over
        if (TimeUsed > MAX_TIME + 0.5 && gameState == MatchCardsGameState.Play) // make sure don't call repeatedly
        {
            showEndCanvas(false);
        }
    }

    public void setTimerText(int time)
    {
        string timeString = time.ToString("D2");
        timer.GetComponent<TextMeshProUGUI>().text = timeString;
        
        // set color to red if time is more than 60
        if (time > MAX_TIME * 0.8)
        {
            timer.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }

    private void handleFlip(object sender, EventArgs e)
    {
        // print("handle flip");
        if (flipedCards.Count == 0)
        {
            ImageGridController controller = sender as ImageGridController;
            if (controller != null)
            {
                flipedCards.Add(controller);
            }
        }else if (flipedCards.Count == 1)
        {
            ImageGridController controller = sender as ImageGridController;
            if (controller != null)
            {
                flipedCards.Add(controller);
            }
            
            // see if two cards match
            if (flipedCards[0].imageFront == flipedCards[1].imageFront && flipedCards[0].imageBack == flipedCards[1].imageBack)
            {
                matchedCards.AddRange(flipedCards);
                flipedCards.Clear();

                if (matchedCards.Count == imagesParent.transform.childCount)
                {
                    // win
                    showEndCanvas(true);
                }
            }
            else
            {
                // flip back
                DelayAction(0.5f,() =>
                {
                    flipedCards[0].StartFlip(0.8f,1, false);
                    flipedCards[1].StartFlip(0.8f,1, false);
                    flipedCards.Clear();
                });
                
            }
        }
        else
        {
            Debug.LogError("illegal fliped cards number");
        }
    }
    
    public void DelayAction(float delaySeconds, Action action)
    {
        StartCoroutine(DelayCoroutine(delaySeconds, action));
    }

    private IEnumerator DelayCoroutine(float delaySeconds, Action action)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }

    private void showEndCanvas(bool result)
    {
        endCanvas.SetActive(true);
        EndCanvasController endCanvasController = endCanvas.GetComponent<EndCanvasController>();
        endCanvasController.setResult(result);
        gameState = MatchCardsGameState.End;
    }
}
