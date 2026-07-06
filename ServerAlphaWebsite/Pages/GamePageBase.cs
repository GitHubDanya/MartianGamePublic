using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.ServerStorage;
using System.Security.Claims;

namespace ServerAlphaWebsite;

public abstract class GamePageBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    [Inject] protected UserInfoStorage userInfoStorage { get; set; } = null!;
    [Inject] protected NavigationManager navigationManager { get; set; } = null!;

    protected User CurrentUser { get; set; } = null!;
    protected abstract GameStage CurrentStage { get; init; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        CurrentUser = userInfoStorage.LogUser(userId);
        ValidateUserStage();
    }

    private string GetUrlForStage(GameStage stage) => stage switch
    {
        GameStage.MainMenu => "/",
        GameStage.Disclaimer => "/disclaimer",
        GameStage.Questionnaire => "/questionnaire",
        GameStage.Game => "/game",
        GameStage.Solution => "/solution",
        GameStage.Finish => "/finish",
        GameStage.Thanks => "/finish",
        _ => "/"
    };

    protected void ValidateUserStage()
    {
        if (CurrentUser.CurrentStage == CurrentStage) return;
        navigationManager.NavigateTo(GetUrlForStage(CurrentStage), forceLoad: true);
    }

    protected void ChangeStage(GameStage stage)
    {
        CurrentUser.CurrentStage = stage;
        navigationManager.NavigateTo(GetUrlForStage(stage));
    }
}
