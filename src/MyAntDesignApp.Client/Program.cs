using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyAntDesignApp.CommonService;
namespace MyAntDesignApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        AddClientServices(builder.Services);
        var host = builder.Build();
        await host.Services.GetRequiredService<SceneService>().InitializeAsync();
        await host.RunAsync();
    }
    public static void AddClientServices(IServiceCollection services)
    {
        services.AddBlazoredLocalStorage(); services.AddScoped<ApiVisitService>(); services.AddScoped<SceneService>();
    }
}
