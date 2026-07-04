using Microsoft.AspNetCore.HttpOverrides;
using ServerAlphaWebsite.ServerStorage;
using ServerAlphaWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<UserInfoStorage>();
builder.Services.AddScoped<CultureService>();
builder.Services.AddScoped<StageValidationService>();
builder.Services.AddScoped<NavigationHistoryService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddLocalization();

var app = builder.Build();

string[] supportedCultures = { "en-US", "he-IL" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseWebSockets();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
