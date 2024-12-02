using System;
using Microsoft.Maui.Controls;

namespace ChatWithDocsMobileApp;

public interface ILoginHandlerIOS
{
#if IOS
    event Action<GoogleUserInfo?> onGoogleLoginCompleted;
    void handleGogoleLogIn();  // Return object to avoid platform-specific types in shared code
#endif
}