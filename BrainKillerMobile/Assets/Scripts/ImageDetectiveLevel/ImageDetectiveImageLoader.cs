using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using UnityEngine;

public class ImageDetectiveImageLoader : MonoBehaviour
{
    private List<Sprite> images = new List<Sprite>();
    
    public async Task<bool> LoadImages(string[] imageNames)
    {
        images.Clear();
        print(imageNames);
        foreach (string name in imageNames)
        {
            Sprite s = await NetworkRequest.DownloadImage(name);
            if (s == null)
            {
                Debug.LogError("ImageDetectiveImageLoader: image name of " + name + " not found");
                return false;
            }
            print("ImageDetectiveImageLoader: image name of " + name + " downloaded");
            images.Add(s);
        }
        return true;
    }
    
    public List<Sprite> GetImages()
    {
        return images;
    }
    
}
