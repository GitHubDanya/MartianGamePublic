
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;

namespace ServerAlphaWebsite;

public class Disclaimer : GamePageBase
{

    private string[] contentText = default!;

    private string animationClass = ""; //animate slideInUp
    private string continueButtonText = "";
    private int contentCounter = 0;

    [Inject] IStringLocalizer<Resource> localizer { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine(System.Globalization.CultureInfo.CurrentCulture.Name);

        contentText = new string[]
        {
            localizer["DisclaimerText1"],
            localizer["DisclaimerText2"],
            localizer["DisclaimerText3"]
        };

        continueButtonText = localizer["DisclaimerContinueButton"];
    }

    private async Task instructionContinueButtonClick()
    {

        if (!CurrentUser.FilledQuestionnaire && contentCounter == 0)
        {
            ChangeStage(GameStage.Questionnaire);
            return;
        }

        if (contentCounter < contentText.Length - 1)
        {
            await changeContentForwards();
            return;
        }

        await AnimateDisclaimerLeaving();
        ChangeStage(GameStage.Game);
    }

    private async Task AnimateDisclaimerLeaving()
    {
        animationClass = "animate slideOutUp";
        await Task.Delay(1000);
    }

    private async Task instructionBackButtonClick()
    {
        if (contentCounter <= 0) return;

        await changeContentBackwards();
        return;
    }

    private async Task changeContentForwards()
    {
        if (contentCounter >= contentText.Length - 1) return;

        animationClass = "animate slideOutUp";
        StateHasChanged();

        await Task.Delay(1000);

        if (contentCounter < contentText.Length - 1)
            contentCounter++;

        if (contentCounter == contentText.Length - 1 && contentCounter != 0)
            continueButtonText = localizer["DisclaimerStartButton"];

        await Task.Yield();

        animationClass = "animate slideInUp";
        StateHasChanged();

        await Task.Delay(1000);
    }

    private async Task changeContentBackwards()
    {
        if (contentCounter <= 0) return;

        animationClass = "animate slideOutUp";
        StateHasChanged();

        await Task.Delay(1000);

        continueButtonText = localizer["DisclaimerContinueButton"];
        contentCounter--;

        await Task.Yield();

        animationClass = "animate slideInUp";
        StateHasChanged();

        await Task.Delay(1000);
    }
}
