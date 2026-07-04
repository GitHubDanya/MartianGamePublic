using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ServerAlphaWebsite.Pages.api.Components;

public partial class Dashboard : ComponentBase
{
    [Inject]
    protected NavigationManager navigationManager { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    private DashboardPage CurrentTab = DashboardPage.Dashboard;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        if (!authState.User.IsInRole("admin"))
        {
            navigationManager.NavigateTo("/");
        }
    }

    private void DashboardNavButtonClick() => CurrentTab = DashboardPage.Dashboard;
    private void DownloadDataNavButtonClick() => CurrentTab = DashboardPage.DownloadData;
    private void DeleteDataNavButtonClick() => CurrentTab = DashboardPage.DeleteData;

    private async Task LogOut() => navigationManager.NavigateTo("/api/auth/logout", forceLoad: true);
    private void RedirectToLogin() => navigationManager.NavigateTo("/api/login");
}
