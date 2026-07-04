namespace ServerAlphaWebsite.Models.DTO;

public class ConversationDto
{
    public string p_userid { get; set; }
    public DateTime p_requesttime { get; set; }
    public string p_request { get; set; }
    public string p_response { get; set; }
    public string p_complexitylevel { get; set; }
    public int p_complexitylevelscore { get; set; }
    public string p_questiontype { get; set; }
    public int p_questiontypescore { get; set; }
    public float p_relevance { get; set; }
    public int p_consistency { get; set; }
    public float p_representativeness { get; set; }
    public float p_totalscore { get; set; }
    public int? p_user_entry_order { get; set; }
}
