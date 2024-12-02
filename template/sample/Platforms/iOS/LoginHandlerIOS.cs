using UIKit;
using Foundation;
using ChatWithDocsMobileApp.Platforms.iOS;
using Microsoft.Maui.Controls;
#if IOS
using NewBinding = NewBindingMaciOS.DotnetNewBinding;


namespace ChatWithDocsMobileApp.Platforms.iOS
{
    public class LoginHandleriOS : ILoginHandlerIOS
    {
        public event Action<GoogleUserInfo?>? onGoogleLoginCompleted = null;
        public void handleGogoleLogIn()
        {
            // Ensure you're accessing the correct iOS RootViewController
            var rootViewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;

            NewBinding.PerformGoogleSignIn(rootViewController, (userDetails, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Error signing in: {error.LocalizedDescription}");
                    onGoogleLoginCompleted?.Invoke(null);
                }
                else if (userDetails != null)
                {
                    // Convert NSDictionary to GoogleUserInfoiOS
                    var userDict = userDetails.ToDictionary();
                    var googleUserInfo = new GoogleUserInfo
                    {
                        accessToken = userDict.TryGetValue("idToken", out var idToken) ? idToken : null,
                        provider = "Google",
                        providerId = "google",
                        name = userDict.TryGetValue("fullName", out var fullName) ? fullName : null,
                        email = userDict.TryGetValue("email", out var email) ? email : null,
                        picture = userDict.TryGetValue("profilePicture", out var profilePicture) ? profilePicture : null
                    };

                    onGoogleLoginCompleted?.Invoke(googleUserInfo);
                    Console.WriteLine($"User details retrieved: {string.Join(", ", userDict)}");
                }
            });
        }
    }

    public static class NSDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NSDictionary nsDict)
        {
            var dict = new Dictionary<string, string>();
            foreach (var key in nsDict.Keys)
            {
                if (key is NSString nsKey && nsDict[key] is NSString nsValue)
                {
                    dict[nsKey.ToString()] = nsValue.ToString();
                }
            }
            return dict;
        }
    }
}


#endif