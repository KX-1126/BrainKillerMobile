using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDetectiveImageLoader : MonoBehaviour
{
    public List<Sprite> LoadImages(string[] imageNames)
    {
        List<Sprite> images = new List<Sprite>();
        foreach (string name in imageNames)
        {
            Sprite s = Resources.Load<Sprite>("GamesAssets/SimilarImages/" + name);
            if (s == null)
            {
                Debug.LogError("ImageDetectiveImageLoader: " + name + " not found");
                continue;
            }
            images.Add(s);
        }
        return images;
    }
}
