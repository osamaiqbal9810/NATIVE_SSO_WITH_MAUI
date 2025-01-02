namespace ChatWithDocsMobileApp;
using ChatWithDocsMobileApp.Views;
using ChatWithDocsMobileApp.Services;
using ChatWithDocsMobileApp.ViewModels;
using ChatWithDocsMobileApp.Framework;

#if ANDROID
//using NewBinding = NewBindingAndroid.DotnetNewBinding;
#elif IOS
using NewBinding = NewBindingMaciOS.DotnetNewBinding;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using NewBinding = System.Object;
#endif

public partial class LoginPage : ContentPage
{
    private readonly ILoginHandlerAndroid? _loginHandlerAndroid = null;
    private readonly ILoginHandlerIOS? _loginHandlerIOS = null;
    private readonly LoginService _loginService;
    LoginViewModel _loginViewModel;

   // TODO:may be interface will be same for android and IOS

    public LoginPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        _loginService = ServiceResolver.ServiceProvider.GetRequiredService<LoginService>();
        _loginViewModel = ServiceResolver.ServiceProvider.GetRequiredService<LoginViewModel>();
        this.BindingContext = _loginViewModel;

#if ANDROID
        _loginHandlerAndroid = ServiceResolver.ServiceProvider.GetRequiredService<ILoginHandlerAndroid>();
        _loginHandlerAndroid.onGoogleLoginCompleted += onSSOLoginCompleted;
        MicrosoftFrame.IsVisible = false; // it is not implemented for maui Android for now

#elif IOS
        _loginHandlerIOS = ServiceResolver.ServiceProvider.GetRequiredService<ILoginHandlerIOS>();
        _loginHandlerIOS.onGoogleLoginCompleted += onSSOLoginCompleted;
         MicrosoftFrame.IsVisible = true; // it is not implemented for maui Android for now
#endif
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var userInfo = new UserInfo();
        userInfo.email = _loginViewModel.Email ?? "";
        userInfo.password = _loginViewModel.Password ?? "";
        var isLoggedIn = await _loginService.loginUser("auth/signIn", userInfo);
        if (isLoggedIn.Success)
        {
            App.Current?.MainPage?.Navigation.PushAsync(new DashboardPage());
        }
        else
        {
            await DisplayAlert("Login Failed", isLoggedIn.Message, "Close");
        }
    }

    // Google login click listner
    private void googleButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            _loginViewModel.IsLoading = true;
#if ANDROID
            _loginHandlerAndroid?.handleLoginAsync();
#elif IOS
            _loginHandlerIOS?.handleGogoleLogIn();
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    // Google login click listner
    private void microsoftButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            _loginViewModel.IsLoading = true;
#if ANDROID
           
            _loginHandlerAndroid?.handleLoginAsync();
#elif IOS
            _loginHandlerIOS?.handleMicrosoftLogIn();
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private async void onSSOLoginCompleted(UserInfo? userInfo)
    {
        _loginViewModel.IsLoading = false;
        if (userInfo != null)
        {
         var isLoggedIn = await _loginService.loginUser("auth/CreateSSOUser",userInfo);
            if (isLoggedIn.Success)
            {
                App.Current?.MainPage?.Navigation.PushAsync(new DashboardPage());
            }
            else
            {
                await DisplayAlert("Login Failed", isLoggedIn.Message, "Close");
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Preferences.Get("access_token", null) != null && App.Current != null)
        {
           App.Current.MainPage = new NavigationPage(new DashboardPage());

        }
    }
}   