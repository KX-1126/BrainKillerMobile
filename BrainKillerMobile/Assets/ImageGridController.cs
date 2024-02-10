using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGridController : MonoBehaviour
{
    public float flipTime;
    public Sprite imageFront;
    public Sprite imageBack;


    [ContextMenu("StartFlip")]
    public void StartFlip()
    {
        print("Begin Flip");
        StopAllCoroutines();
        StartCoroutine(Flip());
        // print("StartFlip");
    }

    IEnumerator Flip()
    {
        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        float elapsedTime = 0;
        bool switched = false;

        while (elapsedTime < flipTime)
        {
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, elapsedTime / flipTime);
            // print(transform.rotation.eulerAngles.y);
            if (transform.rotation.eulerAngles.y > 85 && transform.rotation.eulerAngles.y < 90 && !switched)
            {
                swithSprite();
                switched = true;
            }
            if (transform.rotation.eulerAngles.y > 265 && transform.rotation.eulerAngles.y < 270 && !switched)
            {
                swithSprite();
                switched = true;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void setSprites(Sprite front, Sprite back){
        imageFront = front;
        imageBack = back;
        GetComponent<Image>().sprite = imageFront;
    }

    private void swithSprite(){
        if (imageFront == null || imageBack == null)
            return;

        print("Switching sprite");

        if (GetComponent<Image>().sprite == imageFront)
            GetComponent<Image>().sprite = imageBack;
        else
            GetComponent<Image>().sprite = imageFront;
        
        print("Sprite switched");
    }
}
