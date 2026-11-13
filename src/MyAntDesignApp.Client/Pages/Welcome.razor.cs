using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MyAntDesignApp.CommonService;
using MyAntDesignApp.Models;
namespace MyAntDesignApp.Pages.Components;
public partial class Welcome
{
    [CascadingParameter] public SceneService Scene { get; set; } = default!;
    [Inject] private ApiVisitService Api { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private LoginForm form = new(); private string confirmation = "", error = "", success = "";
    private bool register, busy;
    private void Switch(bool value) { register = value; error = success = ""; confirmation = ""; }
    private async Task Submit()
    {
        error = success = "";
        if (register && form.Password != confirmation) { error = "兩次輸入的密碼不一致。"; return; }
        busy = true;
        try
        {
            if (register) { await Api.Post<object>("Auth/Register", form); register = false; success = "帳號建立成功！請登入，開始記錄。"; form.Password = ""; confirmation = ""; }
            else { var auth = await Api.Post<Auth>("Auth/Login", form); await Api.SaveProfile(auth); Navigation.NavigateTo("/FrightDay"); }
        }
        catch (Exception ex) { error = ex.Message; }
        finally { busy = false; }
    }
    private class LoginForm
    {
        [Required(ErrorMessage = "請輸入帳號。"), RegularExpression(@"^[A-Za-z0-9_-]{3,40}$", ErrorMessage = "使用 3–40 個英文字母、數字、底線或連字號。")]
        public string Account { get; set; } = "";
        [Required(ErrorMessage = "請輸入密碼。"), StringLength(128, MinimumLength = 8, ErrorMessage = "密碼需為 8–128 個字元。")]
        public string Password { get; set; } = "";
    }
}
