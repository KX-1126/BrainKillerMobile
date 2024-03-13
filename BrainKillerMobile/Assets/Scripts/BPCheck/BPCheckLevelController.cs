using System;
using System.Collections.Generic;
using LevelLogic;
using UnityEngine;
using DataLoader;

enum BPCheckLevelState
{
    Init,
    Play,
    Commit, //upload results
    FinalShow, // show the whole image
    Finish
}

public class BPCheckLevelController : LevelControllerBase
{
    public GameObject SWC;
    public GameObject image;
    public GameObject cropAction;
    public GameObject endCanvas;
    public List<GameObject> Buttons;

    private BPCheckLevelState state;
    
    public ModeConfig testModeConfig = new ModeConfig()
    {
        modeName = "BPCheck",
        modeDescription = "Check the BP",
        numOfLevels = 50
    };
    
    public BPCheckLevelConfig testLevelConfig = new BPCheckLevelConfig()
    {
        normalConfig = new LevelConfig()
        {
            modeName = "BPCheck",
            levelId = 1,
            levelName = "BPCheck 1",
            levelDescription = "Check the BP",
        },
        imageName = "test2.v3draw",
        swcName = "test2.swc"
    };

    private void Start()
    {
        state = BPCheckLevelState.Init;
        InitLevel();
    }

    public override void InitLevel()
    {
        // parse level config
        BPCheckLevelConfig curLevelConfig = testLevelConfig;
        
        // get image and swc
        string testSWCPath = "Assets/Resources/GamesAssets/" + curLevelConfig.swcName;
        string testImagePath = "Assets/Resources/GamesAssets/" + curLevelConfig.imageName;
        
        SWCRender swcRender = SWC.GetComponent<SWCRender>();
        swcRender.swc.buildTree(SWCDataStructure.loadSWCFromLocalFile(testSWCPath));
        swcRender.Render(swcRender.swc);
        
        VolumeGenerator volumeGenerator = image.GetComponent<VolumeGenerator>();
        volumeGenerator.currentDataset = V3dRawDataLoader.readV3dRawFromLocalFile(testImagePath);
        volumeGenerator.originalDataset = volumeGenerator.currentDataset;
        volumeGenerator.GenerateVolumeObject(volumeGenerator.currentDataset);
        
        state = BPCheckLevelState.Play;
        
        // crop image and swc
        Invoke(nameof(cropData), 1.0f);
        
        bindButtons();
    }
    
    private void cropData()
    {
        CropWithSwcBp cropper = cropAction.GetComponent<CropWithSwcBp>();
        cropper.crop(30);
        print(cropper.getCroppedDataCount());
        cropper.renderNextCroppedData();
    }

    public void nextBp()
    {
        state = BPCheckLevelState.Play;
        CropWithSwcBp cropper = cropAction.GetComponent<CropWithSwcBp>();
        if (cropper.reachCroppedDataEnd() == false)
        {
            cropAction.GetComponent<CropWithSwcBp>().renderNextCroppedData();
        }
        else
        {
            state = BPCheckLevelState.FinalShow;
            
            // show whole image
            SWCRender swcRender = SWC.GetComponent<SWCRender>();
            swcRender.Render(swcRender.swc);
        
            VolumeGenerator volumeGenerator = image.GetComponent<VolumeGenerator>();
            volumeGenerator.GenerateVolumeObject(volumeGenerator.originalDataset);
            
            
            state = BPCheckLevelState.Finish;
            // show end canvas
            endCanvas.SetActive(true);
            endCanvas.GetComponent<EndCanvasController>().setResult(true);
        }
    }

    private void bindButtons()
    {
        foreach (var button in Buttons)
        {
            LongPressEventTrigger trigger = button.GetComponent<LongPressEventTrigger>();
            trigger.onLongPress.AddListener(handleUserInput);
        }
    }

    public void handleUserInput(BPCheckResultType result)
    {
        if (state != BPCheckLevelState.Play)
        {
            print("only accept input in play state");
            return;
        }
        
        // add visual feedback
        GameObject feedback;
        feedback = result == BPCheckResultType.Correct ? Resources.Load<GameObject>("Prefabs/CorrectImageFeedback") : Resources.Load<GameObject>("Prefabs/WrongImageFeedback");
        GameObject feedbackObject = Instantiate(feedback, SWC.transform);
        feedbackObject.transform.position = SWC.transform.position;
        
        // upload result
        print("get result" + result.ToString());

        state = BPCheckLevelState.Commit;
        //upload result
        
        // show next bp 
        // should be called after upload result
        Invoke(nameof(nextBp),1.0f);
    }

    public override void nextLevel()
    {
        // reget level config
    }
}
