using Microsoft.AspNetCore.Components;

namespace ServerAlphaWebsite.Pages.api.Components
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private NavigationManager NavManager { get; set; } = default!;

        public void HomeButtonClick()
        {
            NavManager.NavigateTo("/");
        }
    }
}
