using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.Forms;
using ServerAlphaWebsite.Fetchers;
using Microsoft.JSInterop;

namespace ServerAlphaWebsite.Pages.api.Components
{
    public partial class Login : ComponentBase
    {
        [Inject] private NavigationManager NavManager { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private LoginForm loginForm = new LoginForm() { Password = "", Username = "" };

        private Dictionary<string, string> users = new();
        private Dictionary<string, string> inputValidityClasses = new()
        {
            {"Username", string.Empty},
            {"Password", string.Empty}
        };

        protected override void OnInitialized()
        {
            users.Add(
                Environment.GetEnvironmentVariable("MARTIAN_DB_ADMIN_USER"), Environment.GetEnvironmentVariable("MARTIAN_DB_ADMIN_PASSWORD")
            );
        }

        private bool UsernameCorrect(string user)
        {
            return users.ContainsKey(user);
        }

        private bool PasswordCorrect(string user, string pass)
        {
            if (users.ContainsKey(user))
            {
                return users[user] == pass;
            }
            return false;
        }

        private bool FormValid()
        {
            return (UsernameCorrect(loginForm.Username) && PasswordCorrect(loginForm.Username, loginForm.Password));
        }

        public async Task Submit()
        {
            inputValidityClasses["Username"] = UsernameCorrect(loginForm.Username) ? string.Empty : "is-invalid";
            inputValidityClasses["Password"] = PasswordCorrect(loginForm.Username, loginForm.Password) ? string.Empty : "is-invalid";

            if (FormValid())
            {
                CookieFetcher cookieFetcher = new CookieFetcher(JS);
                await cookieFetcher.SetCookie(CookieEnum.loggedIn, "true");
                string loggedin = await cookieFetcher.GetCookie(CookieEnum.loggedIn);

                NavManager.NavigateTo("/api");
            }
        }

        public void HomeButtonClick()
        {
            NavManager.NavigateTo("/#");
        }

    }
}
