using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.PythonEngines;
using ServerAlphaWebsite.Parsers;
using ServerAlphaWebsite.GameStages.QuestionAskingStage;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Models.DTO;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.ServerStorage;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using ServerAlphaWebsite.Services;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;
using BlazorBootstrap;

namespace ServerAlphaWebsite.Pages
{
    public partial class Game : ComponentBase
    {

        // private readonly string CHATBOT_NAME = Config.ChatbotName;
        // private readonly string[] CHATBOT_THINKING_SEQUENCE = Config.ChatbotThinkingSequence;
        // private readonly int MIN_GPT_PARSED_RESPONSE_LENGTH = Config.MinGptParsedResponseLength;
        // private readonly string REGEX_MARKUP_PATTERN = Config.RegexMarkupPattern;
        // private readonly string REGEX_MARKUP_REPLACEMENT = Config.RegexMarkupReplacement;
        // private readonly string ANIMATION_SLIDE_IN_UP = Config.AnimationSlideInUp;
        // private readonly string ANIMATION_SLIDE_OUT_UP = Config.AnimationSlideOutUp;


        private string _messageBoxContent = "";
        private string _generalInformationBoxContent = "";

        private string scoreBoxContent = "";
        private string animationClass = Config.AnimationSlideInUp;
        private Modal quitConfirmationModal = default!;
        private Modal instructionModal = default!;
        private ElementReference InputRef;

        private bool inputDisabled = false;
        private bool leavingPage = false;
        private bool chatThinking = false;
        private bool chatEmpty = true;
        private int scoreboardRefreshKey = 0;
        private TaskCompletionSource UserCancelResponseTcs = new();
        private ChatManager chatManager;

        private DbCommunicationProvider dbCommProvider = new();


        public string GeneralInformationBoxContent
        {
            get => _generalInformationBoxContent;
            set
            { _generalInformationBoxContent = Regex.Replace(value, Config.RegexMarkupPattern, Config.RegexMarkupReplacement); }
        }
        public string MessageBoxContent
        {
            get => _messageBoxContent;
            set
            { _messageBoxContent = Regex.Replace(value, Config.RegexMarkupPattern, Config.RegexMarkupReplacement).Replace("\n", "<br />"); }
        }


        [Inject] private StageValidationService StageValidationService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] public IStringLocalizer<Resource> localizer { get; set; } = default!;

        public Game()
        {
            chatManager = new(this);
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();

            UrlParameterParser parser = new(navigationManager);
            Username = parser.GetUrlParameter("user", NavigationManager);
            user = UserInfoStorage.GetUser()

            chatManager = new ChatManager(this);

            _messageBoxContent = chatManager.GetChatString(localizer);
            if (string.IsNullOrEmpty(_messageBoxContent)) _messageBoxContent = localizer["GameChatTextPlaceholder"];
            scoreBoxContent = $"{localizer["GameScore"]}: {user.Score}";
            chatEmpty = user.GetQuestionCount() == 0 ? true : false;

            UpdateChatWindow();
        }

        /* This function handles user input. It is an asynchronous function
		 * purposed with handling the main logic after a message is sent,
		 * along with relevant UI updates.
		 *
		 * It does the following:
		 *
		 * 1. Log the user's input into a Message class
		 * 2. Await a response from the user's OpenAIClient
		 * 3. Create the ConversationDto for the request-response pair
		 * 4. Await a successful save of the dto to the database.
		 * 
		 * Returns a task representing the async operation.
		 */

        private async Task HandleInputKeyDown(KeyboardEventArgs e)
        {
            if (e.Key != "Enter") return;

            StateHasChanged();

            if (string.IsNullOrEmpty(UserMessageInput) || chatThinking)
                return;

            Message msg = LogStateIntoMessage();

            chatManager.SendMessage(msg);

            UpdateChatWindow();
            DisableInput();
            ClearInput();

            _ = Think();

            ResponseDto response = await GetResponseDto(msg);
            bool responseCancelled = response.response == "Cancelled";


            if (!responseCancelled)
            {
                ConversationDto conversation = ConvertToConversationDto(msg, response);
                dbCommProvider.InsertConversation(conversation);
                chatManager.SendMessage((Message)response);
                PopulateSecondaryWindows(response);
                UserInfoStorage.IncrementScore(Username, response.GetTotalScore());
                UserInfoStorage.IncrementQuestionNum(Username);
                currentQuestion = UserInfoStorage.GetQuestionNum(Username);
            }
            else
            {
                chatManager.DeleteLastMessage();
            }

            response.ValidScores();

            StopThinking();
            EnableInput();
            await InvokeAsync(StateHasChanged);
            UpdateChatWindow();
            scoreboardRefreshKey++;
        }

        // --- UI Methods ---

        private Message LogStateIntoMessage()
        {
            Message msg = new Message
            {
                Content = UserMessageInput,
                Sender = MessageSender.User,
                Time = DateTime.Now
            };

            return msg;
        }

        private void UpdateChatWindow()
        {
            MessageBoxContent = chatManager.GetChatString(localizer);

            StateHasChanged();

            _ = Task.Run(() => JS.InvokeVoidAsync("scrollToBottom", "chat-div"));
        }

        private void DisableInput()
        {
            inputDisabled = true;
        }

        private void EnableInput()
        {
            inputDisabled = false;
            _ = Task.Run(() => InputRef.FocusAsync());
        }

        private void ClearInput()
        {
            UserMessageInput = string.Empty;
        }

        private void PopulateSecondaryWindows(ResponseDto response)
        {
            GeneralInformationBoxContent = response.info;
            scoreBoxContent = $"{localizer["GameScore"]}: {(UserInfoStorage.GetScore(Username) + response.GetTotalScore()):0.##}";
        }

        private async void ShowQuitConfirmationModal()
        {
            await quitConfirmationModal.ShowAsync();
        }

        private async void ShowInstructionModal()
        {
            await instructionModal.ShowAsync();
        }

        private async void OnHideInstructionModalClick()
        {
            await instructionModal.HideAsync();
        }

        private async void OnHideQuitConfirmationModalClick()
        {
            await quitConfirmationModal.HideAsync();
        }

        private async Task Think()
        {
            int state = 0;
            string originalChatContent = MessageBoxContent;

            chatThinking = true;

            //await Task.Delay(100);

            while (chatThinking)
            {

                MessageBoxContent = $"{originalChatContent}\n\n<b>{localizer["GameUsernameMark"]}:</b> {CHATBOT_THINKING_SEQUENCE[state]}";

                state++;

                if (state >= CHATBOT_THINKING_SEQUENCE.Length)
                    state = 0;

                StateHasChanged();

                await Task.Delay(1000);
            }
        }

        private void StopThinking()
        {
            chatThinking = false;
        }

        // --- Private Methods ---

        private async Task<ResponseDto> GetResponseDto(Message rawRequest)
        {
            Message request = rawRequest.Clone();
            ResponseDto response = new ResponseDto();

            const int MAX_RETRIES = 5;
            int retryCount = 0;
            bool isValid = false;

            while (!isValid && retryCount < MAX_RETRIES)
            {
                request.Content += $" {System.Globalization.CultureInfo.CurrentCulture.Name}";
                Task GenerationTask = Task.Run(() => response = ClientHost.GetQuestionAnswer(Username, request));
                Task FlagTask = UserCancelResponseTcs.Task;

                Task CompletedTask = await Task.WhenAny(GenerationTask, FlagTask);

                if (CompletedTask != GenerationTask) // Response cancelled
                {
                    ClientHost.ForgetLastMessage(Username);
                    response.response = "Cancelled";
                    isValid = true;
                }
                else
                {
                    isValid = response.ValidScores(verbose: true);
                }

                ResetTcs(ref UserCancelResponseTcs);
                retryCount++;
            }

            return response;
        }

        private ConversationDto ConvertToConversationDto(Message request, ResponseDto response)
        {
            ConversationDto conversationDto = new ConversationDto()
            {
                p_userid = Username,
                p_requesttime = request.Time,
                p_request = request.Content ?? "",
                p_response = response.response,
                p_complexitylevel = response.complexitylevel,
                p_complexitylevelscore = response.complexitylevelscore,
                p_questiontype = response.questiontype,
                p_questiontypescore = response.questiontypescore,
                p_relevance = response.relevance,
                p_consistency = response.consistency,
                p_representativeness = response.representativeness,
                p_totalscore = response.GetTotalScore()
            };

            return conversationDto;
        }

        private void SaveResponseToDatabase(Message request, ResponseDto response)
        {
            if (response == null) return;

            ConversationDto conversation = new ConversationDto()
            {
                p_userid = Username,
                p_requesttime = request.Time,
                p_request = request.Content,
                p_response = response.response,
                p_complexitylevel = response.complexitylevel,
                p_complexitylevelscore = response.complexitylevelscore,
                p_questiontype = response.questiontype,
                p_questiontypescore = response.questiontypescore,
                p_relevance = response.relevance,
                p_consistency = response.consistency,
                p_totalscore = response.GetTotalScore()
            };

            try
            {
                dbCommProvider.InsertConversation(conversation);
            }
            catch
            {
                Console.WriteLine($"(Game.razor.cs) Failed to save response to database - DbCommunicationProvider threw an error.");
            }
        }

        private void ResetTcs(ref TaskCompletionSource tcs)
        {
            tcs = new TaskCompletionSource();
        }

        private void StopResponseButtonClick()
        {
            UserCancelResponseTcs.TrySetResult();
        }

        private async void FinishGame()
        {
            leavingPage = true;

            animationClass = ANIMATION_SLIDE_OUT_UP;
            //await Task.Delay(1000);
            leavingPage = false;
            await StageValidationService.SetUserStage(GameStage.Solution);
            try { NavigationManager.NavigateTo("/solution?user=" + Username); } catch { }
        }
    }
}
