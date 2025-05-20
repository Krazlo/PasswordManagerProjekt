// 📁 ViewModels/ApplicationsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

using PwM_UI.Models;

namespace PwM_UI.ViewModels
{
    public partial class ApplicationsViewModel : ObservableObject
    {
        public ObservableCollection<Credential> Credentials { get; set; } = new();

        [ObservableProperty]
        private Credential? selectedCredential;

        public ApplicationsViewModel()
        {
            Credentials.Add(new Credential { Application = "Facebook", Account = "bruger@email.dk", Password = "********" });
        }

        [RelayCommand]
        private void AddCredential()
        {
            Credentials.Add(new Credential { Application = "Ny app", Account = "", Password = "" });
        }

        [RelayCommand]
        private void DeleteCredential()
        {
            if (SelectedCredential != null)
                Credentials.Remove(SelectedCredential);
        }
    }
}
