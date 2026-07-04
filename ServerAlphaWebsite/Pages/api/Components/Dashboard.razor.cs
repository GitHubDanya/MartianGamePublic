namespace ServerAlphaWebsite.Pages.api.Components;

public partial class Dashboard : GamePageBase
{
    private bool? loggedIn = null;
    private DashboardPage CurrentTab = DashboardPage.Dashboard;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authState = await AuthStateTask;
        if (!authState.User.IsInRole("admin"))
            navigationManager.NavigateTo("/", forceLoad: true);
    }

    private void DashboardNavButtonClick() => CurrentTab = DashboardPage.Dashboard;
    private void DownloadDataNavButtonClick() => CurrentTab = DashboardPage.DownloadData;
    private void DeleteDataNavButtonClick() => CurrentTab = DashboardPage.DeleteData;

    private async Task LogOut() => navigationManager.NavigateTo("/api/auth/logout", forceLoad: true);

    private void RedirectToLogin() => navigationManager.NavigateTo("/api/login");
}
