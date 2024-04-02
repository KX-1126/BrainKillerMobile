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
    public string CreatedAt;
    public string UpdatedAt;
    public string DeletedAt; // 如果您不需要处理这个字段，可以省略
    public string username;
    public string email;
    public string password; // 注意：通常不建议在客户端处理密码
    public int flipProgress;
    public int imageDetectiveProgress;
    public int MatchCardsProgress;
    public int bpcheckProgress;
    public int rewardsGameProgress;
    public int rewardsScienceProgress;
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
        
        string result = await NetworkRequest.PostRequest(NetworkURL.SIGN_IN, json, "");
        if (result != null)
        {
            print("login result:" + result);
            SignInResponse signInResponse = JsonUtility.FromJson<SignInResponse>(result);
            Debug.Log("get token" + signInResponse.token);
            userInfoCache.setToken(signInResponse.token);
            gotoLobby.goToLobby();
        }
        
    }
    
    static string RemoveZeroWidthSpaces(string s) // handle on server side
    {
        return s.Replace("\u200B", "");
    }

}
