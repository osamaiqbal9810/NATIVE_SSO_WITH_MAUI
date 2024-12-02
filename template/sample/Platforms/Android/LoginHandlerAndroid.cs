// File: LoginHandler.cs in Android project

#if ANDROID
using Android.App;
using Android.Content;
using Java.Interop;
using Activity = Android.App.Activity;
using Com.Ps19.Mykotlinlibrary;


namespace ChatWithDocsMobileApp.Platforms.Android
{
    public class LoginHandlerAndroid : ILoginHandlerAndroid
    {
        public event Action<GoogleUserInfo?>? onGoogleLoginCompleted = null;

        public void handleLoginAsync()
        {
            var loginClass = Java.Lang.Class.ForName("com.ps19.mykotlinlibrary.LoginHelper");
            // TODO: server client Id needs to be stored in config.
            var serverClientId = "246654251044-j00qrbdr1t3tu5crinqjr8rnes4cntoi.apps.googleusercontent.com";

            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity ?? Platform.AppContext;

            // Define a callback as a Java.Lang.Object
            var callback = new KotlinCallback(result =>
            {
                // Trigger the event with the GoogleUserInfo
                  onGoogleLoginCompleted?.Invoke(result);
            });
            var javaClientId = new Java.Lang.String(serverClientId);
            var invokeHandleLogin = loginClass.GetMethod("invokeHandleLogin", Java.Lang.Class.FromType(typeof(Context)), Java.Lang.Class.ForName("java.lang.String"), Java.Lang.Class.FromType(typeof(ICallback)));
            invokeHandleLogin.Invoke(null, new Java.Lang.Object[] { context, javaClientId, callback });
            
        }
    }
    public class KotlinCallback : Java.Lang.Object, ICallback
    {
        private event Action<GoogleUserInfo> _onGoogleLoginCompleted;

        public KotlinCallback(Action<GoogleUserInfo> onGoogleLoginCompleted)
        {
            _onGoogleLoginCompleted = onGoogleLoginCompleted;
        }
        public void Invoke(Com.Ps19.Mykotlinlibrary.GoogleSignInResult? result)
        {
            if (result == null)
            {
                _onGoogleLoginCompleted.Invoke(null);
            }  else {
                Console.WriteLine($"Received result from Kotlin: {result.Email}");
                var userInfo = new GoogleUserInfo();
                userInfo.provider = "Google";
                userInfo.providerId = "google";
                userInfo.accessToken = result.IdToken ?? "";
                userInfo.name = result.DisplayName ?? "";
                userInfo.email = result.Email ?? "";
                userInfo.picture = result?.ProfilePictureUri?.ToString() ?? "";
                userInfo.idToken = result?.IdToken ?? "";
                _onGoogleLoginCompleted.Invoke(userInfo);

            }
        }
    }
}

#endif
