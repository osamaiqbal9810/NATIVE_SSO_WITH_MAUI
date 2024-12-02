namespace ChatWithDocsMobileApp.Views;
using ChatWithDocsMobileApp.Services; 


public class DashboardPage : ContentPage
{
    private readonly LoginService _loginService;
    public DashboardPage()
	{
		_loginService = new LoginService();
        var logout_btn = new Button
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Text = "Logout",
        };

        logout_btn.Clicked += async(sender, args) =>  await logOutClicked(sender, args);

            Content = new VerticalStackLayout
			{
				Children = {
					new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
                    },
                     logout_btn    
			}
		};
	}

    async Task logOutClicked(object s, EventArgs a)
    {
        Preferences.Clear();
        App.Current.MainPage = new NavigationPage(new LoginPage());

    }
}