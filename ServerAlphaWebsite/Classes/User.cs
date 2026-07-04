namespace ServerAlphaWebsite.Classes
{
    public class User
    {
        public string Username { get; init; }
        public string ProlificID { get; init; } = string.Empty;
        public string Experiment { get; init; } = string.Empty;

        public GameStage CurrentStage { get; set; } = GameStage.MainMenu;

        public float Score { get; set; } = 0;
        public float SolutionQuality { get; set; } = 0;

        public int TriesLeft { get; set; } = 1;

        public bool FilledQuestionnaire = false;

        public string ResultImageURL { get; private set; } = string.Empty;
        public event EventHandler? ImageURLChanged;

        public List<Message> ChatHistory { get; set; } = new List<Message>();
        public string Solution { get; set; } = string.Empty;

        public User(string username, string prolificId = "unknown", string experimentId = "unknown")
        {
            Username = username;
            ProlificID = prolificId;
            Experiment = experimentId;
        }

        public void IncrementScore(float score) => Score += score;

        public int GetQuestionCount() => ChatHistory.Count;
        public void LogMessage(Message message) => ChatHistory.Add(message);

        public void UpdateResultImageURL(string url)
        {
            ResultImageURL = url;
            ImageURLChanged?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            // Getting the count of messages for a cleaner output
            int chatCount = ChatHistory?.Count ?? 0;

            return $@"User Details:
-------------------------
Username:            {Username}
ProlificID:          {ProlificID}
Experiment:          {Experiment}
Current Stage:       {CurrentStage}
Score:               {Score}
Solution Quality:    {SolutionQuality}
Tries Left:          {TriesLeft}
Filled Questionnaire:{FilledQuestionnaire}
Result Image URL:    {ResultImageURL}
Chat History:        {chatCount} message(s)
Solution:            {Solution}";
        }
    }
}
