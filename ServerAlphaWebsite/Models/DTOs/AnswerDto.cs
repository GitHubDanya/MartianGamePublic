namespace ServerAlphaWebsite.Models.DTOs
{
	public class AnswerDto
	{
		public string p_userid { get; set; }
		public string p_answer { get; set; }
		public DateTime p_submittime { get; set; }
		public string p_solutioncategory { get; set; }
		public string p_coverage { get; set; }
		public float p_coveragescore { get; set; }
		public string p_answerkind { get; set; }
		public int p_triesleft { get; set; }
		public float p_totalscore { get; set; }
		public float? p_dsi { get; set; }
		public float? p_distancefromoptimal { get; set; }

		public AnswerDto()
		{
			p_userid = "unknown";
			p_answer = "NA";
			p_submittime = DateTime.Now;
			p_solutioncategory = "NA";
			p_coverage = "NA";
			p_coveragescore = 0;
			p_answerkind = "unknown";
			p_triesleft = 0;
			p_totalscore = 0;
			p_dsi = null;
			p_distancefromoptimal = null;
		}
	}
}