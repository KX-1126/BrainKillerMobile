using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
    public int Id { get; set; } // Gorm.Model 的简化表示，仅包含 ID
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public long FlipProgress { get; set; }
    public long ImageDetectiveProgress { get; set; }
    public long MatchCardsProgress { get; set; }

    public long BPCheckProgress { get; set; }

    public long RewardsGameProgress { get; set; }
    public long RewardsScienceProgress { get; set; }

    // 假设 BPAnnotation 已经定义为 C# 类
    public string BPAnnotations { get; set; }
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
        
        string result = await NetworkRequest.PostRequest(NetworkURL.SIGN_IN, json);
        if (result != null)
        {
            print("login result:" + result);
            SignInResponse signInResponse = JsonUtility.FromJson<SignInResponse>(result);
            Debug.Log("get token" + signInResponse.token);
            gotoLobby.goToLobby();
        }
        
    }
    
    static string RemoveZeroWidthSpaces(string s) // handle on server side
    {
        return s.Replace("\u200B", "");
    }

}
