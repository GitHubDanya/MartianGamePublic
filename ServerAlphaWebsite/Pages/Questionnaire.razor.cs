using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Services;

namespace ServerAlphaWebsite.Pages
{
    public partial class Questionnaire : GamePageBase
    {
        private int MINIMUM_COUNTRY_NAME_LENGTH = 4;
        private int MAXIMUM_AGE = 99;

        [Inject] StageValidationService StageValidationService { get; set; } = default!;

        PersonalInfoDto response = new PersonalInfoDto();

        private bool FormSubmitted = false;

        private Dictionary<string, string> inputValidityClasses = new()
    {
        {"Age", string.Empty},
        {"Gender", string.Empty },
        {"EnglishFirstLanguage", string.Empty },
        {"Country", string.Empty },
        {"Education", string.Empty },
    };

        private int? age = null;
        private string? gender = null;
        private bool? englishFirstLanguage = null;
        private string? country = null;
        private string? education = null;
        private string? customEducation = null;

        private void HandleValidSubmit()
        {
            if (FormSubmitted) return;

            bool isFormValid = true;
            Dictionary<string, string> validityClassesCopy = new Dictionary<string, string>(inputValidityClasses);
            foreach (KeyValuePair<string, string> item in validityClassesCopy)
            {
                bool pairIsValid = true;
                switch (item.Key)
                {
                    case "Age":
                        if (!IsAgeValid())
                            pairIsValid = false;
                        break;

                    case "Gender":
                        if (!IsGenderValid())
                            pairIsValid = false;
                        break;

                    case "EnglishFirstLanguage":
                        if (!IsEnglishValid())
                            pairIsValid = false;
                        break;

                    case "Country":
                        if (!IsCountryValid())
                            pairIsValid = false;
                        break;

                    case "Education":
                        if (!IsEducationValid())
                            pairIsValid = false;
                        break;
                }
                if (!pairIsValid)
                {
                    isFormValid = false;
                    InvalidateInput(item.Key);
                }
                else
                    ValidateInput(item.Key);
            }

            if (isFormValid)
            {
                _ = HandleValidForm();
            }
        }

        private async Task HandleValidForm()
        {
            FormSubmitted = true;

            populateResponse();

            DbCommunicationProvider provider = new();
            try { provider.InsertPersonalInfo(response); }
            catch (Exception ex)
            { Console.WriteLine("(Questionnaire.razor.cs) Error inserting personal info: " + ex.Message); }

            CurrentUser.FilledQuestionnaire = true;
            ChangeStage(GameStage.Disclaimer);
        }

        private void populateResponse()
        {
            try
            {
                response.p_userid = CurrentUser.Username;
                response.p_experimentid = CurrentUser.Experiment;
                response.p_age = (int?)age;
                response.p_gender = gender;
                response.p_isenglishfirstlanguage = (bool?)englishFirstLanguage;
                response.p_country = country == null
                ? null
                : new string(country.Where(c => char.IsLetter(c)).ToArray()).ToLower();
                response.p_education = education == "Other"
                ? (string?)customEducation
                : (string?)education;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InvalidateInput(string input) => inputValidityClasses[input] = "is-invalid";
        private void ValidateInput(string input) => inputValidityClasses[input] = string.Empty;

        private bool IsAgeValid()
        {
            if (age <= 0 || age > MAXIMUM_AGE)
                return false;
            return true;
        }

        private bool IsGenderValid()
        {
            //return !string.IsNullOrEmpty(gender);
            return true;
        }

        private bool IsEnglishValid()
        {
            //return englishFirstLanguage != null;
            return true;
        }

        private bool IsCountryValid()
        {
            if (!string.IsNullOrEmpty(country) && country.Length < MINIMUM_COUNTRY_NAME_LENGTH && country.ToLower() != "usa") //TODO: ADD COUNTRY CHECKING
                return false;
            return true;
        }

        private bool IsEducationValid()
        {
            if (education == "Other" && string.IsNullOrEmpty(customEducation)) return false;
            return true;
        }
    }
}
