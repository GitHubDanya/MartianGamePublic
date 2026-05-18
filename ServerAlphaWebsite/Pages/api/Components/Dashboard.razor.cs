using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.Fetchers;

namespace ServerAlphaWebsite.Pages.api.Components
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] NavigationManager NavManager { get; set; } = default!;

        private bool? loggedIn = null;
        private CookieFetcher cookieFetcher;
        private string Page = "dashboard";

        private async Task<bool> LoggedIn()
        {
            if (cookieFetcher == null)
                throw new Exception("CookieFetcher is null");

            string cookieResult = await cookieFetcher.GetCookie(CookieEnum.loggedIn);

            if (cookieResult == "true") return true;
            return false;
        }

        private void DashboardNavButtonClick()
        {
            Page = "dashboard";
        }

        private void DownloadDataNavButtonClick()
        {
            Page = "downloaddata";
        }

        private void DeleteDataNavButtonClick()
        {
            Page = "deletedata";
        }

        private async Task LogOut()
        {
            await cookieFetcher.ClearCookies();
            RedirectToLogin();
        }

        private void RedirectToLogin()
        {
            NavManager.NavigateTo("/api/login");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            cookieFetcher = new CookieFetcher(JS);
            loggedIn = await LoggedIn();
            StateHasChanged();
        }
    }
}
