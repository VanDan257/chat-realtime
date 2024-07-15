using Microsoft.AspNetCore.Identity;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using notip_server.ChatBot;
using notip_server.Extensions;
using notip_server.Hubs;
using notip_server.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
EnviConfig.Config(builder.Configuration);
var policy = "_anyCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(policy,
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

builder.Services.AddApplicationServices()
    .AddIdentityServices()
    .AddAwsS3Services(builder.Configuration)
    .AddEmailService(builder.Configuration);

// Thiết lập thời hạn sống token là 1 giờ
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24);
});

builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddSingleton<ChatHub>();

// Đăng ký bot
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
builder.Services.AddTransient<IBot, ChatBot>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policy);

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");

});

app.MapControllers();

app.Run();
