namespace ServerAlphaWebsite.Models.DTOs
{
	public class ScoreDto
	{
		public int ComplexityScore { get; set; }
		public string QuestionType { get; set; }
		public int QuestionTypeScore { get; set; }
		public float RelevanceScore { get; set; }
		public int ConsistencyScore { get; set; }
		public float RepresentativenessScore { get; set; }
		public float TotalScore { get; set; }
	}
}
