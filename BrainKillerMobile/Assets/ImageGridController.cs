using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGridController : MonoBehaviour
{
    public Sprite imageFront;
    public Sprite imageBack;
    public bool isRotating = false;
    public bool isFlipped = false;

    [ContextMenu("StartFlip")]
    public void StartFlip(float flipTime = 0.8f, int flipCount = 1, bool judge = true)
    {
        if (isRotating)
            return;
        StopAllCoroutines();
        StartCoroutine(Flip(flipTime, flipCount, judge));
    }
    
    

    IEnumerator Flip(float flipTime, int flipCount, bool judge)
    {
        // print(gameObject.name + " flipping");
        flipCount -= 1;
        isRotating = true;
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
        isRotating = false;
        isFlipped = !isFlipped;

        if (flipCount > 0)
        {
            StartCoroutine(Flip(flipTime, flipCount, judge));
        }

        if (judge)
        {
            FlipLevelController.Instance.addTryCount();
            FlipLevelController.Instance.JudgeLevel();
        }
        
    }

    public void setSprites(Sprite front, Sprite back){
        imageFront = front;
        imageBack = back;
        GetComponent<Image>().sprite = imageFront;
    }

    private void swithSprite(){
        if (imageFront == null || imageBack == null)
            return;

        // print("Switching sprite");

        if (GetComponent<Image>().sprite == imageFront)
            GetComponent<Image>().sprite = imageBack;
        else
            GetComponent<Image>().sprite = imageFront;
        
        // print("Sprite switched");
    }
    
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            BoxCollider2D _collider2D = GetComponent<BoxCollider2D>();
            // print("mouse pos: " + Input.mousePosition);
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // print("click pos: " + clickPosition);
            // print("collider pos: " + _collider2D.bounds);
            if (_collider2D.OverlapPoint(clickPosition))
            {
                print(gameObject.name + " Clicked");
                GetComponent<ImageGridController>().StartFlip();
            }
        }
    }
    
    [ContextMenu("test flip multiple times")]
    public void testFlipMultipleTimes(){
        StartFlip(2.0f, 3);
    }
}
