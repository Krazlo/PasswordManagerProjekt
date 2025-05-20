// 📁 ViewModels/SettingsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace PwM_UI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private int sessionTimeoutMinutes = 10;

        [ObservableProperty]
        private bool isBiometricEnabled;

        [ObservableProperty]
        private string language = "da-DK";
    }
}
