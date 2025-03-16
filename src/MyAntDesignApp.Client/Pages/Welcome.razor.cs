using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MyAntDesignApp.CommonService;
using MyAntDesignApp.Models;
using static MyAntDesignApp.Client.Models.Common;
using Blazored.LocalStorage;

namespace MyAntDesignApp.Pages.Components
{
    /// <summary>
    /// Adds custom Request and Response examples.
    /// You should install it using the .AddSwaggerExamples() extension method
    /// </summary>
    public partial class Welcome
    {
        [Inject] ApiVisitService ApiVisitService { get; set; }
        [Inject] IMessageService Message { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        private AuthRequest model = new AuthRequest();
        bool loading = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                loading = false;
                StateHasChanged();
            }

        }
        private async void OnFinish()
        {
            loading = true;
            try
            {
                var res = await ApiVisitService.QueryCommon<AuthRequest, Auth>("Auth/Login", model);
                if (res.Result == true)
                {
                    await LocalStorage.SetItemAsync("Token", res.Data);
                    await Message.Success(res.Msg);
                    Navigation.NavigateTo("/FrightDay");
                    await JS.InvokeVoidAsync("location.reload"); // 刷新頁面
                }
                else
                {
                    await Message.Error(res.Msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                await Message.Error("登入失敗，稍後再試 QQ");
            }
            finally
            {
                loading = false;
                StateHasChanged();
            }
        }

        private async void Register()
        {
            loading = true;
            try
            {
                if(model.Account != model.CheckPassword)
                {
                    await Message.Error("兩次密碼不一樣！");
                    return;
                }
                var res = await ApiVisitService.QueryCommon<AuthRequest, Auth>("Auth/Register", model);
                if (res.Result == true)
                {
                    await Message.Success("註冊成功！");
                }
                else
                {
                    await Message.Error(res.Msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                await Message.Error("登入失敗，稍後再試 QQ");
            }
            finally
            {
                loading = false;
                StateHasChanged();
            }
        }


        private void OnFinishFailed(EditContext editContext)
        {
            Console.WriteLine($"Failed:{JsonSerializer.Serialize(model)}");
        }
    }
}