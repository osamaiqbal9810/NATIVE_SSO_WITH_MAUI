using System;
using Foundation;
using UIKit;
namespace NewBindingMaciOS
{
    // @interface DotnetNewBinding : NSObject
    [BaseType(typeof(NSObject))]
    interface DotnetNewBinding
    {
        // +(NSString * _Nonnull)getStringWithMyString:(NSString * _Nonnull)myString __attribute__((warn_unused_result("")));
        [Static]
        [Export("getStringWithMyString:")]
        string GetString(string myString);

        [Static]
        [Export("performGoogleSignInWithPresentingViewController:completion:")]
        void PerformGoogleSignIn(UIViewController presentingViewController, Action<NSDictionary, NSError> completion);

        [Static]
        [Export("performMicrosoftSignInWithPresentingViewController:completion:")]
        void PerformMicrosoftSignIn(UIViewController presentingViewController, Action<NSDictionary, NSError> completion);

    }
}
