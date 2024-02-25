using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageGridController : MonoBehaviour
{
    public Sprite imageFront;
    public Sprite imageBack;
    public bool isRotating = false;
    public bool isFlipped = false;
    
    public event EventHandler FlipCompleted;

    [ContextMenu("StartFlip")]
    public void StartFlip(float flipTime = 0.8f, int flipCount = 1, bool notify = true)
    {
        if (isRotating)
            return;
        StopAllCoroutines();
        StartCoroutine(Flip(flipTime, flipCount, notify));
    }

    IEnumerator Flip(float flipTime, int flipCount, bool notify)
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
            StartCoroutine(Flip(flipTime, flipCount, notify));
        }

        if (notify)
        {
            FlipCompleted?.Invoke(this, EventArgs.Empty);
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

    private void Start()
    {
        StartCoroutine(setColliderSize());
    }

    IEnumerator setColliderSize() 
    {
        yield return null; // wait for a frame
        
        // Debug.Log(GetComponent<RectTransform>().rect.size);
        BoxCollider2D _collider2D = GetComponent<BoxCollider2D>();
        RectTransform _rectTransform = GetComponent<RectTransform>();
        _collider2D.size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
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
                // print(gameObject.name + " Clicked");
                GetComponent<ImageGridController>().StartFlip();
            }
        }
    }
    
    [ContextMenu("test flip multiple times")]
    public void testFlipMultipleTimes(){
        StartFlip(2.0f, 3);
    }
}
