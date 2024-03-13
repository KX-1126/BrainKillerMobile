using System;
using UnityEngine;
using UnityEngine.EventSystems; // 使用事件系统
using UnityEngine.Events;
using UnityEngine.UI; // 使用Unity事件

public enum BPCheckResultType
{
    Correct,
    Wrong
}

public class LongPressEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Image backImage;
    public BPCheckResultType buttonType;
    public float holdTime = 2.0f; // 长按需要的时间，2秒
    public UnityEvent<BPCheckResultType> onLongPress = new UnityEvent<BPCheckResultType>();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            backImage.color = new Color(backImage.color.r, backImage.color.g, backImage.color.b, 0.5f + 0.5f * (Time.time - timePressStarted) / holdTime);
            // print(backImage.color.a);
            if (Time.time - timePressStarted > holdTime)
            {
                longPressTriggered = true;
                onLongPress.Invoke(buttonType); 
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // print("OnPointerDown");
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // print("OnPointerUp");
        isPointerDown = false;
        backImage.color = new Color(backImage.color.r, backImage.color.g, backImage.color.b, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("OnPointerExit");
        isPointerDown = false;
    }
}

