using System;
using Microsoft.Maui.Controls;

namespace ChatWithDocsMobileApp;

public interface ILoginHandlerIOS
{
#if IOS
    event Action<GoogleUserInfo?> onGoogleLoginCompleted;
    void handleGogoleLogIn();  
    void handleMicrosoftLogIn(); 
#endif
}