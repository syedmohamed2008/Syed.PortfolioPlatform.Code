using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Serilog;
using Syed.PortfolioPlatform.Web.Configurations;
using Syed.PortfolioPlatform.Application;
using Syed.PortfolioPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);



// Purpose: Binds "PortfolioSettings" section from appsettings.json to strongly typed class.
builder.Services
    .Configure<PortfolioSettings>(
        builder.Configuration.GetSection("PortfolioSettings"));

builder.Services
    .Configure<ApiSettings>(
        builder.Configuration.GetSection("ApiSettings"));

// Register PortfolioSettings as Singleton
// Purpose: Allows injecting PortfolioSettings directly instead of using IOptions everywhere.
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<PortfolioSettings>>().Value);

// Register ApiSettings as Singleton
// Purpose: Used to store API BaseUrl and other API config.
// Injected into HttpClient configuration.
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<ApiSettings>>().Value);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// HttpClient Configuration
// Purpose: Used to call backend API from MVC project.
builder.Services.AddHttpClient("api", (sp, client) =>
{
    var settings = sp.GetRequiredService<ApiSettings>();
    

    if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        throw new Exception("ApiSettings.BaseUrl is missing");

    client.BaseAddress = new Uri(settings.BaseUrl);

    Console.WriteLine("BASE URL:");
    Console.WriteLine(settings.BaseUrl);

});

// MVC Controllers & Views
// Purpose: Enables MVC pattern (Controllers + Razor Views).
builder.Services.AddControllersWithViews();


// Purpose: Enables server-side session storage.
// Used for: Storing user login tokens, temporary data, authentication state etc.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Serilog Logging
// Purpose: Advanced logging provider for file/DB/console logging.
// Reads configuration from appsettings.json.
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Dev Branch changes
// Global Exception Handling
// Purpose: Custom middleware for centralized error handling.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Response Output Caching
// Purpose: Adds caching capability.
// Base policy = Cache for 60 seconds globally.
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy =>
        policy.Expire(TimeSpan.FromSeconds(60)));
});

// Rate Limiting
// Purpose: Limits number of requests per user/IP.
// Protects API from abuse & DDoS-like behavior.
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);  // Time window
        opt.PermitLimit = 10;                  // Max 10 requests
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;                    // Extra queued requests
    });
});

// Health Checks
// Purpose: Provides /health endpoint to monitor application + database status.
builder.Services.AddHealthChecks();

var app = builder.Build();




// Environment Based Error Handling
// Purpose: Shows detailed errors in Development, Friendly error page in Production.
// Detailed errors (Dev only)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  
}
else
{
    // Friendly error page (Prod)
    app.UseExceptionHandler("/Home/Error");
    // Enforce HTTPS in production
    app.UseHsts(); 
}

//////////// Core Middleware Pipeline  //////////// 

// Redirect HTTP → HTTPS
app.UseHttpsRedirection(); 
app.UseStaticFiles();      
app.UseRouting();          
app.UseSession();           

///////////////////  IMPORTANT CACHING /////////////////
// Response Caching
//app.UseOutputCache();
////////////  DO NOT USE app.UseOutputCache(); BECUASE ITS Enabled global caching policy
//Middleware executes for every request
//Cache system adds overhead
//First request feels slow
///////////////////  IMPORTANT CACHING /////////////////

app.UseAuthorization(); 

// Status Code Handling
// Purpose: Redirects errors like 404/500 to custom page.
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

// Exception Handler Middleware
// Purpose: Handles unhandled runtime exceptions.
app.UseExceptionHandler();

// Enables rate limiting middleware
app.UseRateLimiter(); 

// Health Check Endpoint
// Accessible at: /health
app.MapHealthChecks("/health");

// Route Configuration
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();