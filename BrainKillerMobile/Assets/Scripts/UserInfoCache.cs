using System;
using Network;
using UnityEngine;

namespace DefaultNamespace
{
    public class UserInfoCache : MonoBehaviour
    {
        public static UserInfoCache instance;

        private static string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MTI2NDM1NTYsImd1ZXN0IjpmYWxzZSwidXNlckVtYWlsIjoidGVzdEBleGFtcGxlLmNvbSJ9.qWcCl-I_7m1bzXZ6ZMW5TMjGotr1bqfbd887n7hcUv8";
        private static User userProfile;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            
        }

        private void Start()
        {
            queryForUserProfile();
        }

        public void setToken(string tokenString)
        {
            Debug.Log("set token " + token);
            token = tokenString;
            queryForUserProfile();
        }
        
        public static string getToken()
        {
            return token;
        }

        private async void queryForUserProfile()
        {
            string jsonData = await NetworkRequest.GetRequest(NetworkURL.GET_USER_PROFILE, token);
            // Debug.Log(jsonData);
            if (jsonData != null)
            {
                userProfile = JsonUtility.FromJson<User>(jsonData);
            }
        }

        public static User getUserProfile()
        {
            return userProfile;
        }
    }
}