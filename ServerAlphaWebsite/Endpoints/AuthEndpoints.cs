using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ServerAlphaWebsite.PythonEngines;

namespace ServerAlphaWebsite.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapGameAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/api/auth");

        authGroup.MapGet("/log-user/{customId}", async (string PROLIFIC_PID, HttpContext context) =>
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

        authGroup.MapPost("/login", async (HttpContext context, string username, string password) =>
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

                        return Results.Redirect("/");
                    }

                    return Results.Unauthorized();
                });

        return app;
    }
}
