using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MyAntDesignApp.CommonService;
using Blazored.LocalStorage;
using static MyAntDesignApp.Client.Models.responseModel;
namespace MyAntDesignApp.Pages.Components;
public partial class Night
{
    [CascadingParameter] public SceneService Scene { get; set; } = default!;
    [Inject] private ApiVisitService Api { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ILocalStorageService Storage { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    private DateTime month = new(DateTime.Today.Year, DateTime.Today.Month, 1), selected = DateTime.Today;
    private readonly string[] weekdays = ["日", "一", "二", "三", "四", "五", "六"];
    private List<Memorys> items = []; private List<Comments> comments = [];
    private readonly Dictionary<string, string> pictures = [];
    private string user = "", search = "", message = "", dialogError = "", preview = "", detailImage = "", commentText = "";
    private bool busy = true, saving, isError, listView, dark, editorOpen, deleteMemory, themeAutomatic = true;
    private Memorys editing = new(); private Memorys? opened;
    private Comments? replyTo, editingComment; private int? pendingDeleteComment;
    private byte[]? upload; private string uploadName = "", uploadType = "";
    private IEnumerable<Memorys> Filtered => items.Where(x => string.IsNullOrWhiteSpace(search) ||
        $"{x.Title} {x.Tags} {x.Locate} {x.CreateAt}".Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));
    private IEnumerable<Memorys> SelectedItems => Filtered.Where(x => x.MemoryDate.Date == selected);
    private IEnumerable<DateTime> CalendarDays
    {
        get { var start = month.AddDays(-(int)month.DayOfWeek); for (var i = 0; i < 42; i++) yield return start.AddDays(i); }
    }
    protected override void OnParametersSet()
    {
        if (themeAutomatic) dark = Scene.IsNight;
    }
    private string Weekday(DateTime day) => "星期" + weekdays[(int)day.DayOfWeek];
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        try
        {
            var profile = await Api.GetProfile();
            if (string.IsNullOrEmpty(profile?.Token) || profile.ValidDate <= DateTime.UtcNow) { await Logout(); return; }
            user = profile.UserId;
            selected = Scene.LocalDate; month = new(selected.Year, selected.Month, 1);
            themeAutomatic = !await Storage.ContainKeyAsync("fn-dark");
            dark = themeAutomatic ? Scene.IsNight : await Storage.GetItemAsync<bool>("fn-dark");
            await LoadMonth();
        }
        catch (Exception ex) { await Handle(ex); busy = false; }
        StateHasChanged();
    }
    private async Task ToggleTheme() { themeAutomatic = false; dark = !dark; await Storage.SetItemAsync("fn-dark", dark); }
    private async Task FollowDaylight() { themeAutomatic = true; dark = Scene.IsNight; await Storage.RemoveItemAsync("fn-dark"); }
    private async Task Logout() { await Api.SignOut(); Navigation.NavigateTo("/", replace: true); }
    private async Task Handle(Exception ex)
    {
        if (ex is ApiClientException { Status: System.Net.HttpStatusCode.Unauthorized }) { await Logout(); return; }
        message = ex.Message; isError = true;
    }
    private async Task LoadMonth()
    {
        busy = true;
        try { items = await Api.Get<List<Memorys>>($"Memory/GetByMonth/{month:yyyy-MM-dd}"); }
        catch (Exception ex) { items = []; await Handle(ex); }
        finally { busy = false; }
    }
    private async Task MoveMonth(int offset) { month = month.AddMonths(offset); selected = month; items = []; await LoadMonth(); }
    private async Task Today() { selected = Scene.LocalDate; month = new(selected.Year, selected.Month, 1); await LoadMonth(); }
    private async Task SelectDate(DateTime date)
    {
        selected = date;
        if (date.Month != month.Month || date.Year != month.Year) { month = new(date.Year, date.Month, 1); items = []; await LoadMonth(); }
    }
    private async Task<string> Picture(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        if (!pictures.TryGetValue(key, out var result)) { result = await Api.Get<string>($"Memory/Picture/{key}"); pictures[key] = result; }
        return result;
    }
    private void NewMemory()
    {
        editing = new Memorys { MemoryDate = selected }; preview = dialogError = ""; upload = null; editorOpen = true;
    }
    private async Task CloseEditor() { if (saving) return; await JS.InvokeVoidAsync("fn.closeActiveDialog"); editorOpen = false; }
    private async Task ChoosePicture(InputFileChangeEventArgs e)
    {
        dialogError = "";
        try
        {
            var file = e.File;
            if (file.Size > 2097152) throw new InvalidOperationException("圖片不能超過 2 MB。");
            if (file.ContentType is not ("image/jpeg" or "image/png" or "image/webp")) throw new InvalidOperationException("請選擇 JPG、PNG 或 WebP 圖片。");
            using var stream = new MemoryStream(); await file.OpenReadStream(2097152).CopyToAsync(stream);
            upload = stream.ToArray(); uploadName = file.Name; uploadType = file.ContentType;
            preview = $"data:{uploadType};base64,{Convert.ToBase64String(upload)}";
        }
        catch (Exception ex) { dialogError = ex.Message; }
    }
    private void RemovePicture() { preview = ""; upload = null; editing.PicKey = ""; }
    private async Task SaveMemory()
    {
        if (saving) return;
        saving = true; dialogError = "";
        try
        {
            if (upload != null) { editing.PicKey = await Api.Upload(upload, uploadName, uploadType); pictures[editing.PicKey] = preview; upload = null; }
            var payload = new { editing.Title, MemoryDate = editing.MemoryDate.ToString("yyyy-MM-dd"), editing.Locate, editing.Tags, editing.PicKey };
            if (editing.Id == 0) await Api.Post<Memorys>("Memory", payload);
            else await Api.Put<Memorys>($"Memory/{editing.Id}", payload);
            await JS.InvokeVoidAsync("fn.closeActiveDialog"); editorOpen = false; selected = editing.MemoryDate.Date; month = new(selected.Year, selected.Month, 1);
            message = "已收藏這段回憶。"; isError = false; await LoadMonth();
        }
        catch (Exception ex) { dialogError = ex.Message; if (ex is ApiClientException { Status: System.Net.HttpStatusCode.Unauthorized }) await Logout(); }
        finally { saving = false; }
    }
    private async Task OpenMemory(Memorys item)
    {
        if (busy) return; busy = true; dialogError = ""; deleteMemory = false; ResetComment(); pendingDeleteComment = null;
        try
        {
            var fetched = await Api.Get<List<Comments>>($"Comment/{item.Id}"); var image = await Picture(item.PicKey);
            comments = fetched; detailImage = image; opened = item;
        }
        catch (Exception ex) { await Handle(ex); }
        finally { busy = false; }
    }
    private async Task CloseDetail() { if (saving) return; await JS.InvokeVoidAsync("fn.closeActiveDialog"); opened = null; }
    private async Task EditOpened()
    {
        var item = opened!; editing = new Memorys { Id = item.Id, Title = item.Title, MemoryDate = item.MemoryDate, Locate = item.Locate, Tags = item.Tags, PicKey = item.PicKey };
        preview = detailImage; upload = null; dialogError = "";
        await JS.InvokeVoidAsync("fn.closeActiveDialog"); opened = null; editorOpen = true;
    }
    private async Task DeleteOpened()
    {
        saving = true;
        try { await Api.Delete<Memorys>($"Memory/{opened!.Id}"); await JS.InvokeVoidAsync("fn.closeActiveDialog"); opened = null; message = "回憶已從日曆隱藏。"; isError = false; await LoadMonth(); }
        catch (Exception ex) { dialogError = ex.Message; }
        finally { saving = false; }
    }
    private void ResetComment() { replyTo = editingComment = null; commentText = ""; }
    private async Task ReplyTo(Comments item) { ResetComment(); replyTo = item; await JS.InvokeVoidAsync("fn.focusComment"); }
    private async Task EditComment(Comments item) { ResetComment(); editingComment = item; commentText = item.Comment; await JS.InvokeVoidAsync("fn.focusComment"); }
    private async Task SaveComment()
    {
        if (saving || string.IsNullOrWhiteSpace(commentText)) return;
        saving = true; dialogError = "";
        try
        {
            if (editingComment != null) await Api.Put<Comments>("Comment", new { editingComment.Id, Comment = commentText });
            else await Api.Post<Comments>("Comment", new { MemoryId = opened!.Id, ReplyId = replyTo?.Id, Comment = commentText });
            comments = await Api.Get<List<Comments>>($"Comment/{opened!.Id}"); ResetComment();
        }
        catch (Exception ex) { dialogError = ex.Message; }
        finally { saving = false; }
    }
    private async Task DeleteComment()
    {
        saving = true; dialogError = "";
        try { await Api.Delete<Comments>($"Comment/{pendingDeleteComment}"); comments = await Api.Get<List<Comments>>($"Comment/{opened!.Id}"); pendingDeleteComment = null; ResetComment(); }
        catch (Exception ex) { dialogError = ex.Message; }
        finally { saving = false; }
    }
}
