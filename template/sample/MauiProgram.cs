using ChatWithDocsMobileApp.Framework;
using ChatWithDocsMobileApp.Services;
using ChatWithDocsMobileApp;
using Microsoft.Extensions.Logging;
using ChatWithDocsMobileApp.ViewModels;

namespace ChatWithDocsMobileApp;

public static class MauiProgram
{
    public static void UseResolver(this IServiceProvider sp)
    {
        ServiceResolver.RegisterServiceProvider(sp);
    }

	public static MauiApp CreateMauiApp()
	{
        //App.Current.UserAppTheme = AppTheme.Light;
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		builder.Services.AddSingleton<LoginService>();
		builder.Services.AddSingleton<LoginViewModel>();
#if ANDROID
        builder.Services.AddSingleton<ILoginHandlerAndroid, ChatWithDocsMobileApp.Platforms.Android.LoginHandlerAndroid>();
#elif IOS
		builder.Services.AddSingleton<ILoginHandlerIOS, ChatWithDocsMobileApp.Platforms.iOS.LoginHandleriOS>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif
		MauiApp app = builder.Build();
		app.Services.UseResolver();
		return app;
	}
}
