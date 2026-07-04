using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ServerAlphaWebsite.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapGameAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/api/auth");

        authGroup.MapGet("/log-user/{customId}", async (string customId, HttpContext context) =>
                {
                    var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, customId),
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
                    if (username == Environment.GetEnvironmentVariable(Config.LoginUsernameEnvVariableName)
                            && password == Environment.GetEnvironmentVariable(Config.LoginPasswordEnvVariableName))
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
