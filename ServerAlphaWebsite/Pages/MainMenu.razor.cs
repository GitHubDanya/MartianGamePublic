using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.PythonEngines;
using ServerAlphaWebsite.Services;
using ServerAlphaWebsite.ServerStorage;
using ServerAlphaWebsite.Parsers;
using System.Globalization;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;
using ServerAlphaWebsite.Classes;

namespace ServerAlphaWebsite.Pages
{
    public partial class MainMenu : ComponentBase
    {
        private string[] AVAILABLE_CULTURES = { "en-US", "he-IL" };

        private string username = ClientHost.GenerateUsername();
        private string animationClass = "";
        private string backgroundAnimationClass = "";
        private int currentCultureIndex = 0;

        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private StageValidationService StageValidationService { get; set; } = default!;
        [Inject] private UserInfoStorage UserInfoStorage { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IStringLocalizer<Resource> localizer { get; set; } = default!;
        [Inject] private NavigationManager navigationManager { get; set; } = default!;
        [Inject] private CultureService CultureService { get; set; } = default!;

        private async Task startClicked()
        {
            animationClass = "animate slideOutUp";
            backgroundAnimationClass = "animate fadeOut ";
            await Task.Delay(1000);

            UrlParameterParser parser = new();
            string Prolific_PID = parser.GetUrlParameter("PROLIFIC_PID", NavigationManager);
            string experimentId = parser.GetUrlParameter("experimentId", NavigationManager);

            if (!string.IsNullOrEmpty(Prolific_PID)) username = Prolific_PID;

            User user = UserInfoStorage.LogUser(username, experimentId);
            await StageValidationService.SetUserStage(GameStage.Disclaimer);

            try { NavigationManager.NavigateTo("/disclaimer?user=" + username); } catch { }
        }

        private async Task apiClicked()
        {
            try { NavigationManager.NavigateTo("/api"); } catch { }
        }

        private async Task changeLanguage()
        {
            currentCultureIndex = Array.IndexOf(AVAILABLE_CULTURES, CultureInfo.CurrentCulture.Name);

            if (currentCultureIndex == AVAILABLE_CULTURES.Length - 1)
                currentCultureIndex = 0;
            else
                currentCultureIndex++;

            ChangeCulture(AVAILABLE_CULTURES[currentCultureIndex]);
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await StageValidationService.SetUserStage(GameStage.MainMenu);
                //await ChangeCulture(AVAILABLE_CULTURES[currentCultureIndex]);
            }
        }
    }
}
