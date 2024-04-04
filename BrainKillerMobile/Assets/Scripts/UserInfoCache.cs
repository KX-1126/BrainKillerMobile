using System;
using Network;
using UnityEngine;

namespace DefaultNamespace
{
    public class UserInfoCache : MonoBehaviour
    {
        public static UserInfoCache instance;

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

        private async void queryForUserProfile()
        {
            RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.USER_PROFILE);
            // Debug.Log(jsonData);
            if (result.Success)
            {
                userProfile = JsonUtility.FromJson<User>(result.Data);
            }
            else
            {
                Debug.LogError("fail to get user profile");
                print("error message:" + result.ErrorMessage);
            }
        }
        
        public static void setUserProfile(User user)
        {
            userProfile = user;
        }

        public static User getUserProfile()
        {
            return userProfile;
        }
    }
}