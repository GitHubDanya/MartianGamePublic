using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.Classes;

namespace ServerAlphaWebsite.Services
{
    public class StageValidationService
    {
        private readonly NavigationManager NavigationManager;

        public StageValidationService(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
        }

        public static string GetUrlForStage(GameStage stage) => stage switch
        {
            GameStage.MainMenu => "/",
            GameStage.Disclaimer => "/disclaimer",
            GameStage.Questionnaire => "/questionnaire",
            GameStage.Game => "/game",
            GameStage.Solution => "/solution",
            GameStage.Finish => "/finish",
            GameStage.Thanks => "/finish",
            _ => "/"
        };

        public async Task ValidateUserStage(User user, GameStage stage)
        {
            if (user.CurrentStage == stage) return;

            NavigationManager.NavigateTo(GetUrlForStage(user.CurrentStage));
        }
    }
}
