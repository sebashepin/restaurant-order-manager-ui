using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestaurantOrderManager.Backend.Grpc;
using RestaurantOrderManager.UI.Servers;
using RestaurantOrderManager.UI.Servers.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// gRPC-Web client using configurable backend URL from appsettings.json: Backend:BaseAddress
var backendBase = builder.Configuration["Backend:BaseAddress"] ?? "http://192.168.0.206:5134";
Console.WriteLine($"Backend base address (Servers): {backendBase}");
builder.Services
    .AddGrpcClient<Menu.MenuClient>(o =>
    {
        o.Address = new Uri(backendBase);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()));

// App services
builder.Services.AddScoped<IOrdersAdminService, OrdersAdminServiceGrpc>();

await builder.Build().RunAsync();