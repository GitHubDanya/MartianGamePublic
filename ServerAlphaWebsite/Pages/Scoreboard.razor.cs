
using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.Models.DTO;
using ServerAlphaWebsite.Models.DTOs;

namespace ServerAlphaWebsite.Pages
{
    public partial class Scoreboard : ComponentBase
    {
        [Parameter] public int? Key { get; set; }
        [Parameter] public int? Rows { get; set; }
        [Parameter] public string? PersonalizedFor { get; set; }
        [Parameter] public bool? ShowTopPlayer { get; set; }
        [Parameter] public bool? ShowBottomPlayer { get; set; }
        [Parameter] public int? RelativeForQuestion { get; set; }

        [Inject] NavigationManager NavigationManager { get; set; } = default!;

        public List<string> ScoreboardProperties = new()
        {
            "Place",
            "Username",
            "Score"
        };

        public List<ScoreTableEntry> ScoreboardData = new();

        private string username = "";
        private double averageScore = 0;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(PersonalizedFor) && Rows != null)
                Rows += 1 - Rows % 2;

            ScoreboardData = GetTableData();
            averageScore = GetAverageScore(ScoreboardData);

            //If amount of rows is even and scoreboard displays relatively, make amount of rows odd

            StateHasChanged();
        }

        private double GetAverageScore(List<ScoreTableEntry> entries)
        {
            double average = 0;
            int i;

            for (i = 0; i < entries.Count; i++)
            {
                average += entries[i].score;
            }

            average /= i;
            return average;
        }

        private List<ScoreTableEntry> GetTableData(bool verbose = false)
        {
            DbCommunicationProvider communicationProvider = new();
            List<ScoreTableEntry> scoreTableEntries = new();

            if (verbose)
            {
                Console.WriteLine($"PersonalizedFor: {PersonalizedFor}");
                Console.WriteLine($"Rows (original): {Rows}");
                Console.WriteLine($"RelativeForQuestion: {RelativeForQuestion}");
                Console.WriteLine($"ShowTopPlayer: {ShowTopPlayer}");
                Console.WriteLine($"ShowBottomPlayer: {ShowBottomPlayer}");
            }

            if (RelativeForQuestion != null)
            {
                List<ConversationDto>? conversationDtos = communicationProvider.FetchConversationDtos();
                if (conversationDtos == null) return new();
                conversationDtos = conversationDtos.Where(conversationDto => conversationDto.p_user_entry_order <= RelativeForQuestion).ToList();

                if (conversationDtos == null || conversationDtos.Count == 0)
                    return scoreTableEntries;

                scoreTableEntries = conversationDtos.GroupBy(c => c.p_userid)
                .Select(g => new ScoreTableEntry
                {
                    username = g.Key,
                    score = g.Sum(c => c.p_totalscore)
                }).ToList();
            }
            else
            {
                List<AnswerDto>? answerDtos = communicationProvider.FetchAnswerDtos();
                if (answerDtos == null || answerDtos.Count == 0) return scoreTableEntries;

                scoreTableEntries = answerDtos.GroupBy(c => c.p_userid)
                    .Select(g => new ScoreTableEntry
                    {
                        username = g.Key,
                        score = g.Sum(c => c.p_totalscore)
                    }).ToList();
            }

            scoreTableEntries.Sort((a, b) => b.score.CompareTo(a.score));

            foreach (ScoreTableEntry entry in scoreTableEntries) entry.id = scoreTableEntries.IndexOf(entry);

            ScoreTableEntry? topPlayer = scoreTableEntries.FirstOrDefault();
            ScoreTableEntry? bottomPlayer = scoreTableEntries.LastOrDefault();

            if (Rows == null || Rows > scoreTableEntries.Count)
            {
                if (verbose) Console.WriteLine("\n\nInvalid rows, rows are " + Rows);
                Rows = scoreTableEntries.Count;
            }

            if (!string.IsNullOrEmpty(PersonalizedFor))
            {
                var currentEntry = scoreTableEntries
                    .Where(entry => entry.username == PersonalizedFor)
                    .FirstOrDefault();

                if (currentEntry == null) return scoreTableEntries;

                int objectIndex = scoreTableEntries.IndexOf(currentEntry);
                int half = (int)Rows / 2;
                int start = objectIndex - half;

                if (start < 0)
                    start = 0;

                if (start + Rows > scoreTableEntries.Count)
                    start = scoreTableEntries.Count - (int)Rows;

                scoreTableEntries = scoreTableEntries.GetRange(start, (int)Rows);
            }
            else if (Rows != null)
            {
                scoreTableEntries = scoreTableEntries.Take((int)Rows).ToList();
            }

            if (scoreTableEntries.Any(x => x.score != 0))
                scoreTableEntries = scoreTableEntries.Where(entry => entry.score != 0).ToList();

            try
            {
                if (ShowTopPlayer == true) scoreTableEntries[0] = topPlayer;
                if (ShowBottomPlayer == true) scoreTableEntries[^1] = bottomPlayer;
            }

            catch (Exception ex) { }

            return scoreTableEntries;
        }

        //private List<ScoreTableEntry> GetTableData()
        //{
        //	DbCommunicationProvider communicationProvider = new();
        //	List<AnswerDto?> answerDtos = communicationProvider.FetchAnswerDtos();
        //	List<ScoreTableEntry> scoreTableEntries = new();
        //	answerDtos.Sort((a, b) => b.p_totalscore.CompareTo(a.p_totalscore));

        //	foreach (AnswerDto? answerDto in answerDtos)
        //	{
        //		if (answerDto == null) continue;
        //		ScoreTableEntry entry = new()
        //		{
        //			username = answerDto.p_userid,
        //			score = answerDto.p_totalscore
        //		};
        //		scoreTableEntries.Add(entry);
        //	}

        //	if (Rows == null || Rows > scoreTableEntries.Count)
        //		Rows = scoreTableEntries.Count;

        //	if (!string.IsNullOrEmpty(PersonalizedFor))
        //	{
        //		var currentEntry = scoreTableEntries
        //			.Where(entry => entry.username == PersonalizedFor)
        //			.OrderByDescending(entry => entry.score)
        //			.FirstOrDefault();

        //		if (currentEntry == null) return scoreTableEntries;

        //		int objectIndex = scoreTableEntries.IndexOf(currentEntry);
        //		int half = (int)Rows / 2;
        //		int start = objectIndex - half;

        //		if (start < 0)
        //			start = 0;

        //		if (start + Rows > scoreTableEntries.Count)
        //			start = scoreTableEntries.Count - (int)Rows;

        //		scoreTableEntries = scoreTableEntries.GetRange(start, (int)Rows);
        //	}

        //	return scoreTableEntries;
        //}

        //public float GetScoreForUser(string user, bool truncate = true)
        //{
        //	float score = 0;


        //}

        //public Dictionary<string, string> GetScoreboard()
        //{
        //	List<object> entries = GetTableData().ToList();

        //	foreach (object entry in entries)
        //	{
        //		if (entry[])
        //	}
        //}
    }

    public class ScoreTableEntry
    {
        public int id { get; set; } = 0;
        public string username { get; set; } = string.Empty;
        public double score { get; set; } = 0;
    }
}
