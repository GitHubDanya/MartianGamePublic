using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.Fetchers;

namespace ServerAlphaWebsite.Services
{
    public class StageValidationService
    {
        private readonly IJSRuntime JS;
        private readonly NavigationManager NavigationManager;
        private readonly CookieFetcher CookieFetcher;
        private readonly NavigationHistoryService NavigationHistoryService;

        public StageValidationService(
            IJSRuntime jsRuntime,
            NavigationManager navigationManager,
            NavigationHistoryService navigationHistoryService)
        {
            JS = jsRuntime;
            NavigationManager = navigationManager;
            NavigationHistoryService = navigationHistoryService;
            CookieFetcher = new CookieFetcher(JS);
        }

        public async Task ValidateUserStage(GameStage stage)
        {
            //return;
            string UserStage = await GetUserStage();
            if (UserStage != Enum.GetName(stage))
            {
                string? referrer = NavigationHistoryService.PreviousPage;
                if (string.IsNullOrEmpty(referrer) || referrer == "null")
                    referrer = NavigationHistoryService.PreviousPage ?? "";
                NavigationManager.NavigateTo(referrer);
            }
        }

        public async Task SetUserStage(GameStage stage)
        {
            await CookieFetcher.SetCookie(CookieEnum.stage, Enum.GetName(stage) ?? GameStage.MainMenu.ToString());
        }

        public async Task<string> GetUserStage()
        {
            return await CookieFetcher.GetCookie(CookieEnum.stage);
        }
    }
}
