using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using orders;
using orders.Services;
using RestaurantOrderManager.Backend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ILocalStorage, LocalStorageJs>();

// gRPC-Web client using configurable backend URL from appsettings.json: Backend:BaseAddress
var backendBase = builder.Configuration["Backend:BaseAddress"] ?? "https://localhost:5001";

builder.Services
    .AddGrpcClient<Menu.MenuClient>(o =>
    {
        o.Address = new Uri(backendBase);
    })
    .AddHttpMessageHandler(() => new GrpcWebHandler(GrpcWebMode.GrpcWebText));

// Register fallback JSON service and primary gRPC service implementing IMenuService
builder.Services.AddScoped<MenuServiceWwwroot>();
builder.Services.AddScoped<IMenuService, MenuServiceGrpc>();

await builder.Build().RunAsync();