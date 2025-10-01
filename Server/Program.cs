using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR(); // Add SignalR
builder.Services.AddSingleton<IGameServerManager, GameServerManager>(); // Add GameServerManager
builder.Services.AddHostedService<StaleServerMonitor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
app.MapHub<GameServerHub>("/hubs/gameserver");

app.Run();