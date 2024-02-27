using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateWithMouse : MonoBehaviour
{
    public float rotateSpeed = 5f;
    private float mouseX, mouseY;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);
            transform.localRotation = Quaternion.Euler(-mouseY, -mouseX, 0f);
        }
    }
}
