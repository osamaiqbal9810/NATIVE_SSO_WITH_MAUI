using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatWithDocsMobileApp.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        [ObservableProperty]
        private bool isLoading;
    }
}

