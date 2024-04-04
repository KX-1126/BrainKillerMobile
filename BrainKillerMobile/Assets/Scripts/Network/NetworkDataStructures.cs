using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DefaultNamespace;

namespace Network
{
    public class RequestResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
    
    public class NetworkURL : MonoBehaviour
    {
        public static readonly string SERVER = "http://121.43.239.122:80/api";
        
        public static readonly string SIGN_IN = SERVER + "/signin/";
        public static readonly string REGISTER = SERVER + "/register/";
        public static readonly string USER_PROFILE = SERVER + "/user/profile";
        
        public static readonly string GET_MODE_CONFIG = SERVER + "/getModeConfig";
        public static readonly string GET_LEVELCONFIG = SERVER + "/getLevelConfig";

        public static readonly string IMG_RES = "http://121.43.239.122:8080/static/";
    }

    public class NetworkRequest : MonoBehaviour
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string userAuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MTI4MDQ1NDksImd1ZXN0IjpmYWxzZSwidXNlckVtYWlsIjoidGVzdEBleGFtcGxlLmNvbSJ9.XuEWqqjgwKjDlkgM3o_Qs-bJ681np_HU3I6GSl53Fak";
        
        public static void setToken(string token)
        {
            userAuthToken = token;
        }
        
        public static async Task<RequestResult<string>> GetRequest(string url)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthToken);
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                RequestResult<string> result = new RequestResult<string>()
                {
                    Success = true,
                    Data = content,
                    ErrorMessage = ""
                };
                
                return result;
            }
            else
            {
                RequestResult<string> result = new RequestResult<string>()
                {
                    Success = false,
                    Data = "",
                    ErrorMessage = content
                };
                return result;
            }
        }

        public static async Task<RequestResult<string>> PostRequest(string url, string json)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthToken);
            var postJson = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync(url, postJson);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                RequestResult<string> result = new RequestResult<string>()
                {
                    Success = true,
                    Data = content,
                    ErrorMessage = ""
                };
                return result;
            }
            else
            {
                RequestResult<string> result = new RequestResult<string>()
                {
                    Success = false,
                    Data = "",
                    ErrorMessage = content
                };
                return result;
            }
        }

        public static async Task<Sprite> DownloadImage(string imageName)
        {
            string url = NetworkURL.IMG_RES + imageName; // Construct the full URL
            Debug.Log("downloading image with path:" + url);
            // Set the authorization header with the provided token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAuthToken);
    
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            
                    // Create a texture. Note that textures in Unity are 2D arrays of pixels
                    Texture2D texture = new Texture2D(2, 2); // Width and height values are overwritten by LoadImage
                    bool isLoaded = texture.LoadImage(imageBytes); // Load the image data into the texture
            
                    if (isLoaded)
                    {
                        // Create a sprite from the texture
                        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        return sprite;
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture from image bytes.");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"Failed to download image. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred while downloading image: {ex.Message}");
                return null;
            }
        }
    }
    
}