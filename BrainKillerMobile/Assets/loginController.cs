using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Network;

struct loginRequestBody
{
    public string email;
    public string password;
    public string token;
}

public class User
{
    public int ID;
    public string username;
    public string email;
    public string password; 
    public int flipProgress;
    public int imageDetectiveProgress;
    public int MatchCardsProgress;
    public int bpcheckProgress;
    public int rewardsGameProgress;
    public int rewardsScienceProgress;
    
    public User DeepCopy()
    {
        return new User
        {
            ID = this.ID,
            username = this.username,
            email = this.email,
            password = this.password,
            flipProgress = this.flipProgress,
            imageDetectiveProgress = this.imageDetectiveProgress,
            MatchCardsProgress = this.MatchCardsProgress,
            bpcheckProgress = this.bpcheckProgress,
            rewardsGameProgress = this.rewardsGameProgress,
            rewardsScienceProgress = this.rewardsScienceProgress
        };
    }
}

public class SignInResponse
{
    public User user;
    public string token;
}

public class loginController : MonoBehaviour
{
    public TextMeshProUGUI email;
    public TextMeshProUGUI password;
    public gotoLobby gotoLobby;
    public UserInfoCache userInfoCache;
    
    static readonly HttpClient client = new HttpClient();
    async public void login()
    {
        Debug.Log("Login with " + email.text + " and " + password.text);
        loginRequestBody body = new loginRequestBody();
        body.email = email.text;
        body.password = password.text;
        // body struct to json
        string json = JsonUtility.ToJson(body);
        print(json);
        
        RequestResult<string> result = await NetworkRequest.PostRequest(NetworkURL.SIGN_IN, json);
        if (result.Success)
        {
            print("login result:" + result);
            SignInResponse signInResponse = JsonUtility.FromJson<SignInResponse>(result.Data);
            Debug.Log("get token" + signInResponse.token);
            NetworkRequest.setToken(signInResponse.token);
            gotoLobby.goToLobby();
        }
        else
        {
            print("login failed error message:" + result.ErrorMessage);
        }
        
    }
    
    static string RemoveZeroWidthSpaces(string s) // handle on server side
    {
        return s.Replace("\u200B", "");
    }

}
