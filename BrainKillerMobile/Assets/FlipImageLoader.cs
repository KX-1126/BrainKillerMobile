using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class FlipImageLoader : MonoBehaviour
{
    public Sprite imageFront;
    public Sprite imageBack;

    private void Awake()
    {
        loadImage();
    }

    public void loadImage()
    {
        imageFront = Resources.Load<Sprite>("GamesAssets/front1");
        if (imageFront == null)
            Debug.Log("Image front not found");
        imageBack = Resources.Load<Sprite>("GamesAssets/front2");
        if (imageBack == null)
            Debug.Log("Image back not found");
        print("load image success");
    }

    public (List<Sprite>, List<Sprite>) getSplitedSprites(int numOfSprites)
    {
        List<Sprite> splitedFront = splitSprite(imageFront, numOfSprites);
        List<Sprite> splitedBack = splitSprite(imageBack, numOfSprites);
        return (splitedFront, splitedBack);
    }


    public List<Sprite> splitSprite(Sprite sprite, int numOfSprites)
    {
        List<Sprite> splitedSprites = new List<Sprite>();
        Texture2D originalTexture = sprite.texture;

        int numOfRowsColumns = (int)Mathf.Sqrt(numOfSprites);
        int partWidth = originalTexture.width / numOfRowsColumns;
        int partHeight = originalTexture.height / numOfRowsColumns;

        for (int y = numOfRowsColumns-1; y >= 0; y--)
        {
            for (int x = 0; x < numOfRowsColumns; x++)
            {
                Texture2D partTexture = new Texture2D(partWidth, partHeight);
                partTexture.SetPixels(originalTexture.GetPixels(x * partWidth, y * partHeight, partWidth, partHeight));
                partTexture.Apply();

                Sprite partSprite = Sprite.Create(partTexture, new Rect(0, 0, partWidth, partHeight),
                    new Vector2(0.5f, 0.5f));
                splitedSprites.Add(partSprite);
            }
        }

        return splitedSprites;
    }

}