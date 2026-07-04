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

        public int GetQuestionCount() => ChatHistory.Count;

        public void UpdateResultImageURL(string url)
        {
            ResultImageURL = url;
            ImageURLChanged?.Invoke(this, new EventArgs());
        }
    }
}
