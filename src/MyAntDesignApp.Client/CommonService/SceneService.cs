using System.Globalization;
using Microsoft.JSInterop;
namespace MyAntDesignApp.CommonService;

/// <summary>Browser-local scenery. Original FN hours: day 05:00–16:59, night 17:00–04:59.</summary>
public sealed class SceneService(IJSRuntime js) : IAsyncDisposable
{
    public bool IsNight { get; private set; }
    public DateTime LocalDate { get; private set; } = DateTime.Today;
    public string Logo => IsNight ? "/img/logo-night.png" : "/img/logo-morning.png";
    public string Bottle => IsNight ? "/img/nightglass.png" : "/img/sunglass1.png";
    public string Label => IsNight ? "夜晚・月光與星空" : "白天・日光與雲朵";
    public event Action? Changed;
    private DotNetObjectReference<SceneService>? reference;
    private int? watcher;
    public async Task InitializeAsync()
    {
        if (reference != null) return;
        UpdateClock(await js.InvokeAsync<BrowserClock>("fn.clock"));
        reference = DotNetObjectReference.Create(this);
        watcher = await js.InvokeAsync<int>("fn.observeClock", reference);
    }
    [JSInvokable]
    public void UpdateClock(BrowserClock value)
    {
        var night = value.Hour >= 17 || value.Hour < 5;
        if (!DateTime.TryParseExact(value.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day)) return;
        if (night == IsNight && day == LocalDate) return;
        IsNight = night; LocalDate = day; Changed?.Invoke();
    }
    public async ValueTask DisposeAsync()
    {
        if (watcher is int id) await js.InvokeVoidAsync("fn.stopClock", id);
        reference?.Dispose();
    }
    public sealed class BrowserClock
    {
        public int Hour { get; set; }
        public string Date { get; set; } = "";
    }
}
