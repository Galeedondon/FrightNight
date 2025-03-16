using System;
using System.Net.Http;
using System.Threading.Tasks;
using AntDesign.ProLayout;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyAntDesignApp.CommonService;

namespace MyAntDesignApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            AddClientServices(builder.Services);

            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

            await builder.Build().RunAsync();
        }

        public static void AddClientServices(IServiceCollection services)
        {
            services.AddAntDesign();
            services.AddScoped<ApiVisitService>(); // 註冊 ApiVisitService
            services.AddBlazoredLocalStorage();
        }
    }
}