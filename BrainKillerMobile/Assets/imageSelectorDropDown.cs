using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class imageSelectorDropDown : MonoBehaviour
{
    public TMP_Dropdown imageSelector;
    private List<string> imageNames = new List<string>();

    public testVolumeGeneration volumeGenerator;
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.streamingAssetsPath;
        string[] files = System.IO.Directory.GetFiles(path, "*.v3draw");
        List<string> options = new List<string>();
        foreach (var file in files)
        {
            options.Add(System.IO.Path.GetFileName(file));
        }
        // sort options by image id Img0_X_1443.62_Y_8698.13_Z_3595.19
        options.Sort((a, b) =>
        {
            int GetImgNumber(string filename)
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(filename);
                var match = System.Text.RegularExpressions.Regex.Match(name, @"img(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                    return num;
                return int.MinValue;
            }
            return GetImgNumber(a).CompareTo(GetImgNumber(b));
        });
        imageNames = options;
        imageSelector.ClearOptions();
        imageSelector.AddOptions(options);

        imageSelector.onValueChanged.AddListener(delegate {
            loadImage(imageNames[imageSelector.value]);
        });

    }
    
    private void loadImage(string imageName){
        Debug.Log("Loading image: " + imageName);
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, imageName);
        if (volumeGenerator != null)
        {
            volumeGenerator.reloadImageWithPath(path);
        }
        else
        {
            Debug.LogError("Volume generator is not assigned.");
        }
    }

    public int getCurrentImageName()
    {
        int GetImgNumber(string filename)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(filename);
            var match = System.Text.RegularExpressions.Regex.Match(name, @"img(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
            return num;
            return -1;
        }
        return GetImgNumber(imageNames[imageSelector.value]);
    }
}
