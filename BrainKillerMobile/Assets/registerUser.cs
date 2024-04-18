using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Network;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class registerUser : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI email;
    public TextMeshProUGUI password;

    public GameObject hintText;
    
    async public void register()
    {
        Debug.Log("Register with " + username.text + " and " + email.text + " and " + password.text);
        User newUser = new User()
        {
            username = removeZWSP.RemoveZeroWidthSpaces(username.text),
            email = removeZWSP.RemoveZeroWidthSpaces(email.text),
            password = removeZWSP.RemoveZeroWidthSpaces(password.text),
            flipProgress = 1,
            imageDetectiveProgress = 1,
            MatchCardsProgress = 1,
            bpcheckProgress = 1,
            rewardsGameProgress = 1,
            rewardsScienceProgress = 1
        };
        
        string json = JsonUtility.ToJson(newUser);
        RequestResult<string> result = await NetworkRequest.PostRequest(NetworkURL.REGISTER, json);
        if (result.Success)
        {
            Debug.Log("Register success");
            setHintText("Register Success",Color.green);
            //go back to login
            Invoke(nameof(goBackToLogin), 2.0f);
        }
        else
        {
            setHintText("Register Failed\n" +result.ErrorMessage,Color.red);
            Debug.Log("Register failed");
        }
    }

    void goBackToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    void setHintText(string text,Color backColor)
    {
        hintText.GetComponent<Image>().color = backColor;
        hintText.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
        hintText.SetActive(true);
        // hide after 2 seconds
        
        Invoke(nameof(hideHintText), 2.0f);
    }
    
    void hideHintText()
    {
        hintText.SetActive(false);
    }
}
