using System.Text.RegularExpressions;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.Models.DTO;
using ServerAlphaWebsite.Models.DTOs;

namespace ServerAlphaWebsite.Parsers
{
	public static class ScoreParser
	{
        /* !!!!!!!!!!!!
		 *  DEPRECATED
		 * !!!!!!!!!!!!
		 

        public static float CalculateTotalScore(string[] parsedScore)
		{
			float sum = 0;
			sum = (float.Parse(parsedScore[4]) + float.Parse(parsedScore[5]) + float.Parse(parsedScore[6])) * (float.Parse(parsedScore[1]) + float.Parse(parsedScore[3]));
			return sum;
		}
		public static ScoreDto? Parse(string message)
		{
			const int MIN_NEEDED_PARSED_SCORE_ELEMENTS = 7;
			
			string[] parsedMessage;

			if (message == null || string.IsNullOrEmpty(message))
				return null;

			if (message == "0")
				parsedMessage = Enumerable.Repeat("0", MIN_NEEDED_PARSED_SCORE_ELEMENTS).ToArray();
			else
				parsedMessage = Regex.Replace(message, @"[^a-zA-Z0-9.]", "").Split("..");

			if (parsedMessage.Length < MIN_NEEDED_PARSED_SCORE_ELEMENTS)
				return null;

			try
			{
				ScoreDto score = new ScoreDto()
				{
					ComplexityScore = (int)float.Parse(parsedMessage[1]),
					QuestionType = parsedMessage[2],
					QuestionTypeScore = (int)float.Parse(parsedMessage[3]),
					RelevanceScore = float.Parse(parsedMessage[4]),
					ConsistencyScore = (int)float.Parse(parsedMessage[5]),
					RepresentativenessScore = float.Parse(parsedMessage[6]),
					TotalScore = CalculateTotalScore(parsedMessage)
				};

				return score;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public static string ToString(ScoreDto? score)
		{
			if (score == null)
				return string.Empty;

			return ($"TotalScore = {score.TotalScore}\n" +
			$"ComplexityScore = {score.ComplexityScore}\n" +
			$"QuestionType = {score.QuestionType}\n" +
			$"QuestionTypeScore = {score.QuestionTypeScore}\n" +
			$"RelevanceScore = {score.RelevanceScore}\n" +
			$"ConsistencyScore = {score.ConsistencyScore}\n" +
			$"RepresentativenessScore = {score.RepresentativenessScore}");
		}
		*/
    }
}
