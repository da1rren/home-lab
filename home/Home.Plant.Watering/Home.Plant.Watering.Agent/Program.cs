using Home.Plant.Watering.Agent.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

if (OperatingSystem.IsMacOS())
{
    builder.Services.AddSingleton<IPumpControlService, FakePumpControlService>();
}
else
{
    builder.Services.AddSingleton<IPumpControlService, PumpControlService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Home.Plant.Watering.Agent.RpcServices.PumpRpcService>();

if (builder.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
