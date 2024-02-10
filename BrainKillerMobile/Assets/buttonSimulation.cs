using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonSimulation : MonoBehaviour
{
    private BoxCollider2D _collider2D;
    
    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            print("mouse pos: " + Input.mousePosition);
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            print("click pos: " + clickPosition);
            print("collider pos: " + _collider2D.bounds);
            if (_collider2D.OverlapPoint(clickPosition))
            {
                print("Button Clicked");
                GetComponent<ImageGridController>().StartFlip();
            }
        }
    }
}
