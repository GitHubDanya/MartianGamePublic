using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ServerAlphaWebsite.Fetchers
{
    public class CookieFetcher
    {
        /* Class that handles cookies.
         * 
         * Cookies in use:
         * loggedIn: Is the user logged into the api? true/false
         * stage: The game stage. disclaimer/questionnaire/game/solution/finish
         * culture: The culture for localization. en-US/he-IL
         * 
         */
        private IJSRuntime JS;
        public CookieFetcher(IJSRuntime js)
        {
            JS = js ?? throw new ArgumentNullException(nameof(js));
        }
        public async Task<string> GetCookie(CookieEnum cookieEnum)
        {
            string cookie = cookieEnum.ToString();

            if (string.IsNullOrEmpty(cookie))
                throw new ArgumentException("Cookie name cannot be null or empty", nameof(cookie));

            string? cookieValue = await JS.InvokeAsync<string>("cookieHelper.getCookie", cookie);

            string value = cookieValue ?? "null";

            return value;
        }

        public async Task SetCookie(CookieEnum cookie, string value)
        {
            await JS.InvokeVoidAsync("cookieHelper.setCookie", cookie.ToString(), value);
        }

        public async Task ClearCookies()
        {
            await JS.InvokeVoidAsync("cookieHelper.deleteAllCookies");
        }
    }
}
