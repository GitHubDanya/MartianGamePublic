using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.GameStages.AnsweringStage;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Parsers;
using ServerAlphaWebsite.PythonEngines;
using ServerAlphaWebsite.ServerStorage;
using ServerAlphaWebsite.Services;



namespace ServerAlphaWebsite.Pages
{
    public partial class Solution : ComponentBase
    {
        // --- Private Fields ---

        private string userMessageInput;
        private bool InputDisabled { get; set; }
        private string username;
        private bool imageGenDb;
        private bool solutionGeneratedDb;
        private bool solutionGenerated;
        private string generatedSolution;
        private DbCommunicationProvider dbCommunicationProvider;

        // --- Injected Properties ---

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private UserInfoStorage UserInfoStorage { get; set; } = default!;
        [Inject] private StageValidationService StageValidationService { get; set; } = default!;

        public Solution()
        {
            userMessageInput = "";
            InputDisabled = false;
            username = "unknown";
            imageGenDb = false;
            solutionGeneratedDb = false;
            solutionGenerated = false;
            generatedSolution = string.Empty;
            dbCommunicationProvider = new();
        }

        // --- Lifecycle Methods ---

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await StageValidationService.ValidateUserStage(GameStage.Solution);
            }

            if (!imageGenDb)
            {
                imageGenDb = true;
                _ = Task.Run(() => GenerateAndLogImage(username));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            UrlParameterParser parser = new();
            username = parser.GetUrlParameter("user", NavigationManager);
            userMessageInput = UserInfoStorage.GetSolution(username);
        }

        // --- Private Methods ---

        private void HandleInputKeyDown()
        {

        }

        private async Task GenerateAndLogImage(string username)
        {
            string imageUrl;
            imageUrl = await Task.Run(() => ImageService.GenerateImage(username));
            //string imageUrl = ImageService.GenerateImage(username);
            UserInfoStorage.LogImageUrl(username, imageUrl);
        }

        private void GenerateSolution()
        {
            if (solutionGeneratedDb) return;

            solutionGeneratedDb = true;
            InputDisabled = true;

            _ = InvokeAsync(StateHasChanged);
            generatedSolution = ClientHost.GetConversationSummary(username);
            userMessageInput = generatedSolution;

            InputDisabled = false;
            solutionGenerated = true;

            StateHasChanged();
        }

        private AnswerDto CreateAnswerDto(User user, AnswerCategorizationDto categorization)
        {
            AnswerDto answer = new AnswerDto()
            {
                p_userid = username,
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
            UserInfoStorage.LogSolution(userMessageInput, username);
            await StageValidationService.SetUserStage(GameStage.Game);
            NavigationManager.NavigateTo("/game?user=" + username);
        }

        private async void FinishButtonClick()
        {
            await FinishGame();
        }

        private async Task FinishGame()
        {
            AnswerCategorizationDto answerCategorizationDto = ClientHost.GetAnswerCategorizationDto(username);

            UserInfoStorage.SetScore(username, UserInfoStorage.GetScore(username) * answerCategorizationDto.coveragescore);

            User? user = UserInfoStorage.GetUser(username);

            if (user == null) { return; }

            AnswerDto answer = CreateAnswerDto(user, answerCategorizationDto);

            try
            {
                dbCommunicationProvider.InsertAnswer(answer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(Solution.razor.cs) Failed to save answer to database.\nException: {ex.Message}");
            }

            UserInfoStorage.LogSolution(userMessageInput, username);

            await StageValidationService.SetUserStage(GameStage.Finish);
            NavigationManager.NavigateTo("/finish?user=" + username);
        }
    }
}
