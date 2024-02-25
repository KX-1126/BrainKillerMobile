using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCardsImageLoader : MonoBehaviour
{
    public List<Sprite> LoadImages(List<string> imageNames)
    {
        // local retrive
        List<Sprite> images = new List<Sprite>();
        foreach (var name in imageNames)
        {
            Sprite img = Resources.Load<Sprite>("GamesAssets/localRawImage/" + name);
            if (img == null)
            {
                Debug.LogError("Image " + name + " not found");
                return null;
            }
            images.Add(img);
        }

        return images;
    }
}
