namespace ServerAlphaWebsite.Models.DTOs
{
    public class PersonalInfoDto
    {
        public string p_userid { get; set; }
        public string? p_experimentid { get; set; }
        public int? p_age { get; set; }
        public string? p_gender { get; set; }
        public bool? p_isenglishfirstlanguage { get; set; }
        public string? p_country { get; set; }
        public string? p_education { get; set; }

        public override string ToString()
        {
            return $"Age: {p_age}\nGender: {p_gender}\nIsEnglishFirstLanguage: {p_isenglishfirstlanguage}\nCountry: {p_country}\nEducation: {p_education}";
        }
    }
}


