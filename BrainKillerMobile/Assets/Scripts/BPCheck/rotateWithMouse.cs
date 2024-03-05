using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateWithMouse : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public GameObject rotateArea;
    private float mouseX, mouseY;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // check for mouse movement within transform
            Vector3 mousePos = Input.mousePosition;
            Rect rect = rotateArea.GetComponent<RectTransform>().rect;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rotateArea.GetComponent<RectTransform>().position);
            if (Math.Abs(screenPos.x - mousePos.x) < rect.width/2.0f && Math.Abs(screenPos.y - mousePos.y) < rect.height/2.0f)
            {
                mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
                mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed;
                mouseY = Mathf.Clamp(mouseY, -90f, 90f);
                transform.localRotation = Quaternion.Euler(-mouseY, -mouseX, 0f);
            }
        }
    }
}
