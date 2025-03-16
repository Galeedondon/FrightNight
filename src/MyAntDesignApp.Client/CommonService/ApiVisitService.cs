using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Blazored.LocalStorage;
using MyAntDesignApp.Models;
using Newtonsoft.Json;
using static MyAntDesignApp.Client.Models.Common;

namespace MyAntDesignApp.CommonService
{
    public class ApiVisitService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly string _baseUrl;
        private readonly string _token;

        public ApiVisitService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExperienceAPI:FrightNightApi:Url"];
            _token = configuration["YourTokenConfigKey"]; // 從設定中獲取 Token 
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            _localStorage = localStorage;
        }

        public async Task<Auth> GetProfile()
        {
            var auth = new Auth();
            string serializedData = await _localStorage.GetItemAsync<string>("Token");
            if (serializedData != null && serializedData != "")
            {
                 auth = JsonConvert.DeserializeObject<Auth>(serializedData);
            }

            return auth;
        }

        public async Task<ApiRes<bool>> Vlidation()
        {
            string serializedData = await _localStorage.GetItemAsync<string>("Token");
            if (serializedData != null && serializedData != "")
            {

                // 反序列化成 Auth 物件
                var tokenData = JsonConvert.DeserializeObject<Auth>(serializedData);

                // 將 Auth 物件序列化成 JSON 並發送給 API
                var httpContent = new StringContent(JsonConvert.SerializeObject(tokenData), Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync($"{_baseUrl}/Auth/ValidateToken", httpContent);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();



                return JsonConvert.DeserializeObject<ApiRes<bool>>(content);
            }
            var noToken = new ApiRes<bool>();
            noToken.Result = false;
            noToken.Data = false;
            noToken.Msg = "請乖乖登入  :>>>";
            return noToken;
        }

        public async Task<string> GetCurrentUser()
        {
            string user = string.Empty;
            string tokenInfo = await _localStorage.GetItemAsync<string>("Token");
            if (!string.IsNullOrWhiteSpace(tokenInfo))
            {
                var tokenModel = JsonConvert.DeserializeObject<Auth>(tokenInfo);
                user = tokenModel != null ? tokenModel.UserId : string.Empty;
            }
            return user;
        }


        public async Task<ApiRes<S>> QueryCommon<T, S>(string method, T queryModel)
        {
            var jsonContent = JsonConvert.SerializeObject(queryModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/{method}", httpContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiRes<S>>(content);
        }

        public async Task<ApiRes<S>> GetAsync<S>(string method)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{method}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiRes<S>>(content);
        }

        public async Task<ApiRes<S>> PutAsync<T, S>(string method, T queryModel)
        {
            var jsonContent = JsonConvert.SerializeObject(queryModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/{method}", httpContent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiRes<S>>(content);
        }

        public async Task DeleteAsync(string method)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{method}");
            response.EnsureSuccessStatusCode();
        }
    }
}
