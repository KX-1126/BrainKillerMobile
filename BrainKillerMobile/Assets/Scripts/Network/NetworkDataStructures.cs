using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Network
{
    public class NetworkURL : MonoBehaviour
    {
        public static readonly string SERVER = "http://121.43.239.122:80/api";
        
        public static readonly string SIGN_IN = SERVER + "/signin/";
        public static readonly string REGISTER = SERVER + "/register/";
        public static readonly string GET_USER_PROFILE = SERVER + "/user/profile";
        
        public static readonly string GET_MODE_CONFIG = SERVER + "/getModeConfig";
        public static readonly string GET_LEVELCONFIG = SERVER + "/getLevelConfig";

        public static readonly string IMG_RES = "http://121.43.239.122:8080/static/";
    }

    public class NetworkRequest : MonoBehaviour
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public static async Task<string> GetRequest(string url, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                return content;
            }
            else
            {
                Console.WriteLine("Request failed");
                return null;
            }
        }

        public static async Task<string> PostRequest(string url, string json, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var postJson = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync(url, postJson);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                return content;
            }
            else
            {
                Console.WriteLine("Request failed");
                return null;
            }
        }

        public static async Task<Sprite> DownloadImage(string imageName, string token)
        {
            string url = NetworkURL.IMG_RES + imageName; // Construct the full URL
            Debug.Log("downloading image with path:" + url);
            // Set the authorization header with the provided token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
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