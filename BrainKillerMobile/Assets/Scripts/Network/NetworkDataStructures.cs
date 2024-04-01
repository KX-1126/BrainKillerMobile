using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;

namespace Network
{
    public class NetworkURL : MonoBehaviour
    {
        public static readonly string SERVER = "http://121.43.239.122:80/api";
        
        public static readonly string SIGN_IN = SERVER + "/signin/";
        public static readonly string REGISTER = SERVER + "/register/";
    }

    public class NetworkRequest : MonoBehaviour
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public static async Task<string> GetRequest(string url)
        {
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

        public static async Task<string> PostRequest(string url, string json)
        {
            var response = await httpClient.PostAsync(url, new StringContent(json));
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
        
    }
    
}