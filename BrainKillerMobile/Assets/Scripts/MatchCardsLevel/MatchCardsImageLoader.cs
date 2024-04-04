using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Network;

public class MatchCardsImageLoader : MonoBehaviour
{
    private List<Sprite> images = new List<Sprite>();

    public List<Sprite> getImage()
    {
        return images;
    }
    
    public async Task<bool> LoadImages(List<string> imageNames)
    {
        foreach (var name in imageNames)
        {
            Sprite img = await NetworkRequest.DownloadImage(name);
            if (img == null)
            {
                Debug.LogError("Image " + name + " not found");
                return false;
            }
            images.Add(img);
        }

        return true;
    }
}
