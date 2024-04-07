using System;
using System.Collections.Generic;
using System.Linq;
using LevelLogic;
using UnityEngine;
using DataLoader;
using DefaultNamespace;
using Network;

enum BPCheckLevelState
{
    Init,
    Play,
    Commit, //upload results
    FinalShow, // show the whole image
    Finish
}

public struct BpCheckResult
{
    public int userId;
    public int swcId;
    public string correctBp;
    public string wrongBp;
}

public class BPCheckLevelController : LevelControllerBase
{
    public GameObject SWC;
    public GameObject image;
    public GameObject cropAction;
    public GameObject endCanvas;
    public List<GameObject> Buttons;

    private BPCheckLevelState state;
    private ModeConfig curModeConfig;
    private  BPCheckLevelConfig curLevelConfig;
    private int SWCId;

    private void Start()
    {
        state = BPCheckLevelState.Init;
        InitLevel();
    }

    public async void InitMode()
    {
        RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.GET_MODE_CONFIG + "/bpCheck/");
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
        // parse level config
        RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.BP_LEVEL_CONFIG);
        if (!result.Success)
        {
            Debug.LogError("fail to get level config");
            print("error message:" + result.ErrorMessage);
            return;
        }
        
        curLevelConfig = JsonUtility.FromJson<BPCheckLevelConfig>(result.Data);
        SWCId = curLevelConfig.swcId;
        print(curLevelConfig.imageName);
        print(curLevelConfig.swcName);
        
        // get image and swc
        string SWCPath = NetworkURL.IMG_RES + curLevelConfig.swcName;
        // string ImagePath = NetworkURL.IMG_RES + curLevelConfig.imageName;
        
        SWCRender swcRender = SWC.GetComponent<SWCRender>();
        List<Node> nodes = await SWCDataStructure.loadSWCFromURL(SWCPath);
        if (nodes.Count == 0)
        {
            print("load swc from url failed");
            return;
        }
        swcRender.swc.buildTree(nodes);
        swcRender.Render(swcRender.swc);
        
        VolumeGenerator volumeGenerator = image.GetComponent<VolumeGenerator>();
        volumeGenerator.currentDataset = await V3dRawDataLoader.readV3dRawFromURL(curLevelConfig.imageName);
        if (volumeGenerator.currentDataset == null)
        {
            print("load v3draw image from url failed");
            return;
        }
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

    public async void handleUserInput(BPCheckResultType result)
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
        
        state = BPCheckLevelState.Commit;
        
        // upload result
        CropWithSwcBp cropper = cropAction.GetComponent<CropWithSwcBp>();
        string bpIndex = cropper.getCurrentBpIndex().ToString() ;
        string correctBpIndex = "";
        string wrongBpIndex = "";
        if (result == BPCheckResultType.Correct)
        {
            correctBpIndex = bpIndex;
        }
        else
        {
            wrongBpIndex = bpIndex;
        }
        BpCheckResult BpResult = new BpCheckResult()
        {
            userId = UserInfoCache.getUserProfile().ID,
            swcId = SWCId,
            correctBp = correctBpIndex,
            wrongBp = wrongBpIndex,
        };

        string jsonBody = JsonUtility.ToJson(BpResult);
        print(jsonBody);
        
        RequestResult<string> postResult = await NetworkRequest.PostRequest(NetworkURL.POST_BP_CHECK_RESULT,jsonBody);
        if (!postResult.Success)
        {
            Debug.LogError("post annotations failed");
            print("error message:" + postResult.ErrorMessage);
        }
        else
        {
            print($"post annotations success, mark bp of index {bpIndex} as {result.ToString()}");
        }
        // show next bp 
        // should be called after upload result
        Invoke(nameof(nextBp),1.0f);
    }

    public override void nextLevel()
    {
        InitLevel();
        endCanvas.SetActive(false);
    }
}
