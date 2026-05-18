using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.Forms;

namespace ServerAlphaWebsite.Pages.api.Components
{
    public partial class DataRemoval : ComponentBase
    {
        bool PopupConfirmationShown = false;
        ApiSearchForm apiForm = new();

        private void ShowPopupConfirmation()
        {
            PopupConfirmationShown = true;
            StateHasChanged();
        }


    }
}
