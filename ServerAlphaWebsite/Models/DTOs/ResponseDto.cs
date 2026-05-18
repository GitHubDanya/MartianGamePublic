using ServerAlphaWebsite.Classes;

namespace ServerAlphaWebsite.Models.DTOs
{
	public class ResponseDto
	{
		private int MAX_COMPLEXITY_LEVEL_SCORE = 6;
		private int MAX_QUESTION_TYPE_SCORE = 5;
		private float[] ACCEPTABLE_RELEVANCY_LEVELS = { 0.25f, 0.5f, 1.25f, 2f};
		private float[] ACCEPTABLE_REPRESENTATIVENESS_LEVELS= { 0.25f, 0.5f, 1.25f, 2f };

		public string response { get; set; }
		public string info { get; set; }
		public string complexitylevel { get; set; }
		public int complexitylevelscore { get; set; }
		public string questiontype { get; set; }
		public int questiontypescore { get; set; }
		public float relevance { get; set; }
		public int consistency { get; set; }
		public float representativeness { get; set; }

		public ResponseDto()
		{
			response = string.Empty;
			info = string.Empty;
			complexitylevel = string.Empty;
			complexitylevelscore = 0;
			questiontype = string.Empty;
			questiontypescore = 0;
			relevance = 0;
			consistency = 0;
			representativeness = 0;
		}

		public float GetTotalScore()
		{
			return (relevance + consistency + representativeness) * (complexitylevelscore + questiontypescore);
		}

		public bool ValidScores(bool verbose = false)
		{
			if (response == string.Empty)
			{
				if (verbose) Console.WriteLine("Response is empty");
				return false;
			}

			if (complexitylevelscore == 0 && questiontypescore == 0 && relevance == 0 && consistency == 0 && representativeness == 0)
			{
				if (verbose) Console.WriteLine("Score is empty");
				return true;
			}

			bool complexitygood = complexitylevelscore <= MAX_COMPLEXITY_LEVEL_SCORE && complexitylevelscore >= 0;
			bool questiontypegood = questiontypescore >= 0 && questiontypescore <= MAX_QUESTION_TYPE_SCORE;
			bool relevancegood = ACCEPTABLE_RELEVANCY_LEVELS.Contains(relevance);
			bool consistencygood = consistency == 1 || consistency == 0;
			bool representativenessgood = ACCEPTABLE_REPRESENTATIVENESS_LEVELS.Contains(representativeness);

			//bool relevancegood = relevance == 0.25 || relevance == 0.5 || relevance == 1.25 || relevance == 2;
			//bool representativenessgood = representativeness == 0.25 || representativeness == 0.5 || representativeness == 1.25 || representativeness == 2;

			string rawQuestionType = questiontype.ToLower();
			bool questioncategorizationfitsscore = (
				questiontypescore == 1 ? (rawQuestionType == "directive") :
				questiontypescore == 2 ? (rawQuestionType == "clarifying" || rawQuestionType == "follow-up") :
				questiontypescore == 3 ? (rawQuestionType == "analytical") :
				questiontypescore == 5 ? (rawQuestionType == "exploratory" || rawQuestionType == "creative") :
				false
			);


			if (verbose)
			{
				Console.Write("Checking score margins..\n");

				Console.Write($"Complexity Level: " + (complexitygood ? "Pass .. " : $"Fail! {complexitylevelscore}\n"));
				Console.Write($"Question type: " + (questiontypegood ? "Pass .. " : $"Fail! {questiontypescore}\n"));
				Console.Write($"Relevance: " + (relevancegood ? "Pass .. " : $"Fail! {relevance}\n"));
				Console.Write($"Consistency: " + (consistencygood ? "Pass .. " : $"Fail! {consistency}\n"));
				Console.Write($"Representativeness: " + (representativenessgood ? "Pass .. " : $"Fail! {representativeness}\n"));
				Console.Write($"Question categorization fits score: " + (questioncategorizationfitsscore ? "Pass\n" : $"Fail! {rawQuestionType} for score {questiontypescore}\n"));

				Console.WriteLine("\nTest completed.");
			}

			return (complexitygood && questiontypegood && relevancegood && consistencygood && representativenessgood && questioncategorizationfitsscore);
		}

		public static explicit operator Message(ResponseDto response)
		{
			return new Message
			{
				Time = DateTime.Now,
				Content = response.response,
				Sender = MessageSender.GPT
			};
		}
	}
}
