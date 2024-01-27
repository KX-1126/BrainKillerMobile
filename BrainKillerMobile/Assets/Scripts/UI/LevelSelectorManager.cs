using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelSelectorManager : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public GameObject lockedLevelButtonPrefab;
    public GameObject content;
    public Image backImage;
    
    public bool showed = false;
    
    public void InitLevelSelector(int userRecord, int numOfLevels)
    {
        for (int i = 1; i <= numOfLevels; i++)
        {
            GameObject levelButton = null;
            if (i <= userRecord)
            {
                levelButton = Instantiate(levelButtonPrefab, content.transform);
                ItemData data = levelButton.GetComponent<ItemData>();
                if (data != null)
                {
                    data.setItemLevelNumber(i);
                }
            }
            else
            {
                levelButton = Instantiate(lockedLevelButtonPrefab, content.transform);
            }
            
            levelButton.transform.SetParent(content.transform);
            
            //calculate content height
            RectTransform rectTransform = content.GetComponent<RectTransform>();
            float height = 200 * (numOfLevels / 5.0f + 1) + 50 * (numOfLevels / 5.0f);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
    }
    
    private void Start()
    {
        print(transform.Find("LevelScrollView").transform.localPosition);
        print(transform.Find("LevelScrollView").transform.position);
    }

    [ContextMenu("show")]
    public void animateShow()
    {
        backImage.enabled = true;
        showed = true;
        Transform scrollView = transform.Find("LevelScrollView");
        StartCoroutine(MoveToRelativePosition(0.8f, new Vector3(0.0f,scrollView.localPosition.y - Screen.height/2.0f - 200.0f, 0.0f), scrollView));
    }
    
    [ContextMenu("hide")]
    public void animateHide()
    {
        showed = false;
        Transform scrollView = transform.Find("LevelScrollView");
        float parentHeight = scrollView.parent.GetComponent<RectTransform>().rect.height;
        StartCoroutine(
            MoveToRelativePosition(0.5f,
                new Vector3(0.0f, scrollView.localPosition.y + Screen.height / 2.0f + 200.0f, 0.0f), scrollView, () =>
            {
                backImage.enabled = false;
            }));
    }

    public void toggleShow()
    {
        if (showed)
        {
            animateHide();
        }
        else
        {
            animateShow();
        }
    }
    
    IEnumerator MoveToRelativePosition(float duration, Vector3 targetPosition, Transform targetTransform, Action callback = null)
    {
        float timeElapsed = 0;
        Vector3 startingPosition = targetTransform.localPosition;
        
        while (timeElapsed < duration)
        {
            targetTransform.localPosition = Vector3.Lerp(startingPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        targetTransform.localPosition = targetPosition; // 确保最终位置准确
        callback?.Invoke();
    }
}
