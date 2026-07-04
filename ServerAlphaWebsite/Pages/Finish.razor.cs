using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Services;

namespace ServerAlphaWebsite.Pages
{
    public partial class Finish : GamePageBase
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

        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject] StageValidationService StageValidationService { get; set; } = default!;

        private void SubscribeToImageGeneratedEvent()
        {
            CurrentUser.ImageURLChanged += (sender, e) =>
            {
                code = CurrentUser.ResultImageURL; InvokeAsync(StateHasChanged);
            };
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

            CurrentUser.TriesLeft--;
            ChangeStage(GameStage.Game);
        }

        protected override async Task OnInitializedAsync()
        {
            DbCommunicationProvider db = new();
            SubscribeToImageGeneratedEvent();
            if (!string.IsNullOrEmpty(CurrentUser.ResultImageURL))
                code = CurrentUser.ResultImageURL;

            canRetry = (CurrentUser.TriesLeft > 0);

            List<AnswerDto>? fetchedAnswerDtos = db.FetchAnswerDtos();
            if (fetchedAnswerDtos == null) averageScore = 0;
            else
                averageScore = (int)Math.Round(GetAverageScore(fetchedAnswerDtos), MidpointRounding.AwayFromZero);
        }
    }
}
