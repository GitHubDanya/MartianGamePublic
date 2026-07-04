using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Parsers;
using ServerAlphaWebsite.ServerStorage;
using ServerAlphaWebsite.Services;

namespace ServerAlphaWebsite.Pages
{
    public partial class Finish : ComponentBase
    {
        private readonly string LOADING_ICON_PATH = Config.FilePathLoadingIcon;

        private float score = 0;
        private int averageScore = 0;
        private string code = Config.FilePathLoadingIcon;
        public string Username = "";

        private bool canRetry = false;
        private bool leavingPage = false;

        private bool imageLoaded = false;
        private bool finishButtonClicked = false;

        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] UserInfoStorage UserInfoStorage { get; set; } = default!;
        [Inject] StageValidationService StageValidationService { get; set; } = default!;

        private void SubscribeToImageGeneratedEvent()
        {
            User? user = UserInfoStorage.GetUser(Username);
            if (user == null) return;

            user.ImageURLChanged += (sender, e) => { code = user.ResultImageURL; InvokeAsync(StateHasChanged); };
        }

        public void EraseUser(string user)
        {
            UserInfoStorage.DeleteUser(user);
        }

        private void QuitButtonClicked()
        {
            if (leavingPage) return;
            leavingPage = true;

            finishButtonClicked = true;
        }

        private double GetAverageScore(List<AnswerDto> answers)
        {
            double average = 0;
            foreach (AnswerDto answer in answers)
            {
                average += answer.p_totalscore;
            }
            if (answers.Count > 0)
                average /= answers.Count;
            return average;
        }
        private async Task GoBackButtonClicked()
        {
            if (leavingPage) return;
            leavingPage = true;

            UserInfoStorage.RemoveTry(Username);
            UserInfoStorage.LoadRawScore(Username);
            await StageValidationService.SetUserStage(GameStage.Game);
            NavigationManager.NavigateTo("/game?user=" + Username);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await StageValidationService.ValidateUserStage(GameStage.Finish);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            UrlParameterParser parser = new();
            DbCommunicationProvider db = new();

            Username = parser.GetUrlParameter("user", NavigationManager);
            score = UserInfoStorage.GetScore(Username);

            SubscribeToImageGeneratedEvent();
            User? user = UserInfoStorage.GetUser(Username);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.ResultImageURL))
                    code = user.ResultImageURL;

                canRetry = (user.TriesLeft > 0);
            }

            averageScore = (int)Math.Round(GetAverageScore(db.FetchAnswerDtos()), MidpointRounding.AwayFromZero);
        }
    }
}
