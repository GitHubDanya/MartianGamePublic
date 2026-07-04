using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.Parsers;
using System.Globalization;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;

namespace ServerAlphaWebsite.Pages
{
    public partial class MainMenu : GamePageBase
    {
        private string[] AVAILABLE_CULTURES = { "en-US", "he-IL" };

        private string animationClass = "";
        private string backgroundAnimationClass = "";
        private int currentCultureIndex = 0;

        [Inject] private IStringLocalizer<Resource> localizer { get; set; } = default!;
        [Inject] private CultureService CultureService { get; set; } = default!;

        private async Task startClicked()
        {
            await animatePageLeaving();

            UrlParameterParser parser = new(navigationManager);
            string Prolific_PID = parser.GetUrlParameter("PROLIFIC_PID");
            string experimentId = parser.GetUrlParameter("experimentId");

            CurrentUser.CurrentStage = GameStage.Disclaimer;
            navigationManager.NavigateTo("/disclaimer");
        }

        private async Task animatePageLeaving()
        {
            animationClass = "animate slideOutUp";
            backgroundAnimationClass = "animate fadeOut ";
            await Task.Delay(1000);
        }

        private async Task apiClicked()
        {
            try { navigationManager.NavigateTo("/api"); } catch { }
        }

        private async Task changeLanguage()
        {
            currentCultureIndex = Array.IndexOf(AVAILABLE_CULTURES, CultureInfo.CurrentCulture.Name);

            if (currentCultureIndex == AVAILABLE_CULTURES.Length - 1)
                currentCultureIndex = 0;
            else
                currentCultureIndex++;

            await ChangeCulture(AVAILABLE_CULTURES[currentCultureIndex]);
        }

        private async Task ChangeCulture(string value)
        {
            if (CultureInfo.CurrentCulture.Name != value)
            {
                Console.WriteLine(CultureInfo.CurrentCulture.Name + " " + value);
                var uri = new Uri(navigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
                var cultureEscaped = Uri.EscapeDataString(value);
                var uriEscaped = Uri.EscapeDataString(uri);
                Console.WriteLine(value);

                navigationManager.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
            }
        }

        protected override void OnInitialized()
        {
            CultureService.OnChange += () => InvokeAsync(StateHasChanged);
        }
    }
}
