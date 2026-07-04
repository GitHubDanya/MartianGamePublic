using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.ServerStorage;
using System.Globalization;
using System.Security.Claims;

namespace ServerAlphaWebsite;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    protected NavigationManager NavManager { get; set; } = default!;

    [Inject]
    protected UserInfoStorage userInfoStorage { get; set; } = default!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private User? CurrentPlayer;

    private bool IsRtl => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        var user = authState.User;

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            var currentUri = new Uri(NavManager.Uri);
            var queryString = currentUri.Query;

            NavManager.NavigateTo($"/api/auth/log-user{queryString}", forceLoad: true);
        }
        else
        {
            var playerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            CurrentPlayer = userInfoStorage.LogUser(playerId);
        }
    }
}
