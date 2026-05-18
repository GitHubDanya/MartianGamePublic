namespace ServerAlphaWebsite.Classes
{
    public class User
    {
        /*
		 * User
		 * Represents a user.
		 * 
		 * --- Values ---
		 * Username: Username of the user.
		 * Score: Score of the user. (default 0)
		 * TriesLeft: Number of tries left for the user. (default 1)
		 * ResultImageUrl: URL of the generated image summarizing the user's conversation with the user. Used in solution.razor and finish.razor.
		 *					Setter for this value calls the ImageURLChanged event.
		 * ImageUrlChanged: Event that fires when the ResultImageUrl is set.
		 * ChatHistory: List of messages sent by the user and the chatbot.
		 * Soluton: Last saved solution of the user.
		 */

        public string Username { get; set; }
        public string ProlificID { get; set; } = string.Empty;
        public string Experiment { get; set; } = string.Empty;
        public float Score { get; set; } = 0;
        public float RawScore { get; set; } = 0;
        public int TriesLeft { get; set; } = 1;
        public int QuestionNum { get; set; } = 0;

        private string _resultImageURL = string.Empty;
        public string ResultImageURL
        {
            get => _resultImageURL;
            set
            {
                _resultImageURL = value;
                ImageURLChanged?.Invoke(this, new EventArgs());
            }

        }

        public event EventHandler? ImageURLChanged;
        public List<Message> ChatHistory { get; set; } = new List<Message>();
        public string Solution { get; set; } = string.Empty;
    }
}
