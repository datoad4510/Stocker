using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using StockPredictor;
using StockPredictor.Auth.Handlers;
using StockPredictor.Auth.PolicyProviders;
using StockPredictor.Auth.Requirements;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockPredictor.Areas.Identity.Data;
using StockPredictor.Data;

var builder = WebApplication.CreateBuilder(args);
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'StockPredictorContextConnection' not found.");;
builder.Services.AddScoped<IMyLogger, MyLogger>();


// Add authentication with cookie-based authentication handler
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(identityConnectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddServerSideBlazor();  // Needed for Blazor integration

builder.Services.AddBlazorBootstrap();

builder.Services.AddRazorPages();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
});

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();



// your fault
builder.Services.AddSingleton<IAuthorizationPolicyProvider, MinimumAgePolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MinimumAge18",
        policy => policy.AddRequirements(new MinimumAgeRequirement(18)));
    
    // options.FallbackPolicy = new AuthorizationPolicyBuilder()
    //     .RequireAuthenticatedUser()
    //     .Build();
});

builder.Services.AddDefaultIdentity<StockPredictorUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapStaticAssets();
app.MapRazorPages();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapBlazorHub();


app.Run();