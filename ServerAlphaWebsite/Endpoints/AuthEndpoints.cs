using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ServerAlphaWebsite.PythonEngines;

namespace ServerAlphaWebsite.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapGameAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/api/auth");

        authGroup.MapGet("/log-user", async (string? PROLIFIC_PID, HttpContext context) =>
                {
                    string assignedId = string.IsNullOrWhiteSpace(PROLIFIC_PID)
                    ? ClientHost.GenerateUserId()
                    : PROLIFIC_PID;
                    var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, assignedId),
                new Claim(ClaimTypes.Role, "user")
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await context.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                    return Results.Redirect("/");
                });

        authGroup.MapPost("/login", async (HttpContext context, [FromForm] string username, [FromForm] string password) =>
                {
                    if (username == Environment.GetEnvironmentVariable(ServerAlphaWebsite.Config.LoginUsernameEnvVariableName)
                            && password == Environment.GetEnvironmentVariable(ServerAlphaWebsite.Config.LoginPasswordEnvVariableName))
                    {
                        var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, "AdminUser"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "user")
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity));

                        return Results.Redirect("/api");
                    }
                    else
                    {
                        // Console.WriteLine("Recieved incorrect login information:");
                        // Console.WriteLine($"Request username: {username}     ({Environment.GetEnvironmentVariable(ServerAlphaWebsite.Config.LoginUsernameEnvVariableName)})");
                        // Console.WriteLine($"Request password: {password}     ({Environment.GetEnvironmentVariable(ServerAlphaWebsite.Config.LoginPasswordEnvVariableName)})");
                    }

                    return Results.Redirect("/api/login");
                }).DisableAntiforgery();

        authGroup.MapGet("/logout", async (HttpContext context) =>
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Results.Redirect("/");
                });

        return app;
    }
}
