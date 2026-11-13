using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using MyAntDesignApp.Models;
using static MyAntDesignApp.Client.Models.Common;
namespace MyAntDesignApp.CommonService;
public sealed class ApiClientException(string message, HttpStatusCode status) : Exception(message)
{
    public HttpStatusCode Status { get; } = status;
}
public class ApiVisitService(HttpClient http, IConfiguration config, ILocalStorageService storage)
{
    private readonly string baseUrl = (config["ExperienceAPI:FrightNightApi:Url"] ?? "http://localhost:5189").TrimEnd('/');
    public ValueTask<Auth?> GetProfile() => storage.GetItemAsync<Auth>("Token");
    public ValueTask SaveProfile(Auth auth) => storage.SetItemAsync("Token", auth);
    public ValueTask SignOut() => storage.RemoveItemAsync("Token");
    private async Task<T> Send<T>(HttpMethod method, string path, HttpContent? content = null)
    {
        using var req = new HttpRequestMessage(method, $"{baseUrl}/{path}") { Content = content };
        var profile = await GetProfile();
        if (!string.IsNullOrEmpty(profile?.Token)) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profile.Token);
        HttpResponseMessage response;
        try { response = await http.SendAsync(req); }
        catch (HttpRequestException) { throw new ApiClientException("無法連線，請確認 FN API 已啟動後重試。", HttpStatusCode.ServiceUnavailable); }
        using (response)
        {
            ApiRes<T>? result;
            try { result = await response.Content.ReadFromJsonAsync<ApiRes<T>>(); }
            catch (JsonException) { throw new ApiClientException("伺服器回應格式異常，請稍後重試。", response.StatusCode); }
            if (!response.IsSuccessStatusCode || result?.Result != true)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized && path != "Auth/Login") await SignOut();
                throw new ApiClientException(result?.Msg ?? "操作失敗，請稍後重試。", response.StatusCode);
            }
            return result.Data!;
        }
    }
    public Task<T> Get<T>(string path) => Send<T>(HttpMethod.Get, path);
    public Task<T> Post<T>(string path, object value) => Send<T>(HttpMethod.Post, path, JsonContent.Create(value));
    public Task<T> Put<T>(string path, object value) => Send<T>(HttpMethod.Put, path, JsonContent.Create(value));
    public Task<T> Delete<T>(string path) => Send<T>(HttpMethod.Delete, path);
    public Task<string> Upload(byte[] bytes, string name, string type)
    {
        var form = new MultipartFormDataContent(); var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue(type); form.Add(file, "avatar", name);
        return Send<string>(HttpMethod.Post, "Memory/Picture", form);
    }
}
