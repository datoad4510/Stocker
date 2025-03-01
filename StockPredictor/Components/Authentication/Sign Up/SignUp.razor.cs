using Microsoft.AspNetCore.Components;

namespace StockPredictor.Components.Authentication.Sign_Up;

public partial class SignUp : ComponentBase
{
    [Inject] IMyLogger MyLogger { get; set; }

    protected override void OnInitialized()
    {
        MyLogger.Log("Initialized Sign Up Component");
    }
    
}