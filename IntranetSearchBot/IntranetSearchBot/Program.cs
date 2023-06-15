using IntranetSearchBot;
using IntranetSearchBot.Commands;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.TeamsFx.Conversation;
using Microsoft.Bot.Builder;
using IntranetSearchBot.Interfaces;
using IntranetSearchBot.Services;
using IntranetSearchBot.Helpers;
using IntranetSearchBot.Models.App;
using Microsoft.Graph.ExternalConnectors;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();

// Prepare Configuration for ConfigurationBotFrameworkAuthentication
builder.Configuration["MicrosoftAppType"] = "MultiTenant";
builder.Configuration["MicrosoftAppId"] = builder.Configuration.GetSection("BOT_ID")?.Value;
builder.Configuration["MicrosoftAppPassword"] = builder.Configuration.GetSection("BOT_PASSWORD")?.Value;

builder.Configuration["TenantId"] = builder.Configuration.GetSection("TenantId")?.Value;
builder.Configuration["ConnectionName"] = builder.Configuration.GetSection("ConnectionName")?.Value;
builder.Configuration["SearchSizeThreshold"] = builder.Configuration.GetSection("SearchSizeThreshold")?.Value;
builder.Configuration["SearchPageSize"] = builder.Configuration.GetSection("SearchPageSize")?.Value;

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

// Create the Cloud Adapter with error handling enabled.
// Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
// register the same adapter instance for both types.
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
builder.Services.AddSingleton<BotAdapter>(sp => sp.GetService<CloudAdapter>());


//var configuration = new ConfigurationBuilder()
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("appsettings.json")
//    .Build();
//builder.Services.AddSingleton<IConfiguration>(configuration);

//builder.Services.Configure<AppConfigOptions>(configuration);
builder.Services.AddSingleton<IGraphHelper, GraphHelper>();
builder.Services.AddSingleton<IGraphService, GraphService>();
builder.Services.AddSingleton<IFileService, IntranetSearchBot.Services.FileService>();
builder.Services.AddSingleton<IAdaptiveCardService, AdaptiveCardService>();

// Create command handlers and the Conversation with command-response feature enabled.
builder.Services.AddSingleton<HelloWorldCommandHandler>();
builder.Services.AddSingleton<SearchCommandHandler>();

builder.Services.AddSingleton(sp =>
{
    var options = new ConversationOptions()
    {
        Adapter = sp.GetService<CloudAdapter>(),
        Command = new CommandOptions()
        {
            Commands = new List<ITeamsCommandHandler>
            {
                sp.GetService<HelloWorldCommandHandler>(),
                sp.GetService<SearchCommandHandler>(),
            }
        }
    };

    return new ConversationBot(options);
});

// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, TeamsBot>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();