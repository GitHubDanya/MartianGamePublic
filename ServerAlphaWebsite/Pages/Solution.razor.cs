using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.GameStages.AnsweringStage;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Parsers;
using ServerAlphaWebsite.PythonEngines;
using ServerAlphaWebsite.Services;

namespace ServerAlphaWebsite.Pages
{
    public partial class Solution : GamePageBase
    {
        // --- Private Fields ---

        private string userMessageInput;
        private bool InputDisabled { get; set; }
        private bool imageGenerationDebounce;
        private bool solutionGenerating;
        private bool solutionGenerated;
        private string generatedSolution;
        private DbCommunicationProvider dbCommunicationProvider;

        // --- Injected Properties ---

        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private StageValidationService StageValidationService { get; set; } = default!;

        public Solution()
        {
            userMessageInput = "";
            InputDisabled = false;
            imageGenerationDebounce = false;
            solutionGenerating = false;
            solutionGenerated = false;
            generatedSolution = string.Empty;
            dbCommunicationProvider = new();
        }

        // --- Lifecycle Methods ---

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!imageGenerationDebounce)
            {
                imageGenerationDebounce = true;
                _ = Task.Run(() => GenerateAndLogImage(CurrentUser.Username));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            userMessageInput = CurrentUser.Solution;
        }

        private void HandleInputKeyDown()
        {

        }

        private async Task GenerateAndLogImage(string username)
        {
            string imageUrl;

            imageUrl = await Task.Run(() => ImageService.GenerateImage(username));
            CurrentUser.UpdateResultImageURL(imageUrl);
        }

        private void GenerateSolution()
        {
            if (solutionGenerating) return;

            solutionGenerating = true;
            InputDisabled = true;
            _ = InvokeAsync(StateHasChanged);

            generatedSolution = ClientHost.GetConversationSummary(CurrentUser.Username);
            userMessageInput = generatedSolution;

            InputDisabled = false;
            solutionGenerated = true;
            solutionGenerating = false;

            StateHasChanged();

            Console.WriteLine(ClientHost.GetActiveClientsReport());
        }

        private AnswerDto CreateAnswerDto(User user, AnswerCategorizationDto categorization)
        {
            AnswerDto answer = new AnswerDto()
            {
                p_userid = CurrentUser.Username,
                p_answer = userMessageInput,
                p_submittime = DateTime.Now,
                p_solutioncategory = categorization.solutioncategory,
                p_coverage = categorization.coverage,
                p_coveragescore = categorization.coveragescore,
                p_answerkind = Enum.GetName(GetAnswerKind()) ?? "unknown",
                p_triesleft = user.TriesLeft,
                p_totalscore = user.Score
            };

            return answer;
        }

        private AnswerKind GetAnswerKind()
        {
            if (userMessageInput.Trim() == generatedSolution)
                return AnswerKind.botgenerated;
            else if (solutionGenerated)
                return AnswerKind.botassisted;
            else
                return AnswerKind.regular;
        }

        private async Task GoBack()
        {
            CurrentUser.Solution = userMessageInput;
            ChangeStage(GameStage.Game);
        }

        private async void FinishButtonClick() => await FinishGame();

        private async Task FinishGame()
        {
            AnswerCategorizationDto answerCategorizationDto = ClientHost.GetAnswerCategorizationDto(CurrentUser.Username);

            CurrentUser.SolutionQuality = answerCategorizationDto.coveragescore;

            AnswerDto answer = CreateAnswerDto(CurrentUser, answerCategorizationDto);

            try
            {
                dbCommunicationProvider.InsertAnswer(answer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(Solution.razor.cs) Failed to save answer to database.\nException: {ex.Message}");
            }

            CurrentUser.Solution = userMessageInput;
            ChangeStage(GameStage.Finish);
        }
    }
}
