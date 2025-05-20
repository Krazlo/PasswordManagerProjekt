using System.Windows.Input;
using PwM_UI.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using PwM_UI.Views;

namespace PwM_UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public bool ShowVaults { get; set; } = true;
        public bool ShowApplications { get; set; } = false;
        private string _searchText;
        //private bool _showVaults = true;
        //private bool _showApplications = false;

        //public bool ShowVaults
        //{
        //    get => _showVaults;
        //    set
        //    {
        //        _showVaults = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public bool ShowApplications
        //{
        //    get => _showApplications;
        //    set
        //    {
        //        _showApplications = value;
        //        OnPropertyChanged();
        //    }
        //}

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public VaultsViewModel VaultsViewModel { get; } = new VaultsViewModel();
        public ApplicationsViewModel ApplicationsViewModel { get; } = new ApplicationsViewModel();

        public ICommand AddNewCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainWindowViewModel()
        {
            AddNewCommand = new Commands.RelayCommand(AddNew);
            OpenSettingsCommand = new Commands.RelayCommand(OpenSettings);
            SearchCommand = new Commands.RelayCommand(Search, CanSearch);
            RefreshCommand = new Commands.RelayCommand(Refresh);
        }

        private void AddNew(object parameter)
        {
            if (ShowVaults)
            {
                var addVault = new AddVault();
                addVault.Owner = Application.Current.MainWindow;
                addVault.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addVault.ShowDialog();
            }
            else if (ShowApplications)
            {
                var addApp = new AddApplication();
                addApp.Owner = Application.Current.MainWindow;
                addApp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addApp.ShowDialog();
            }
        }

        private void OpenSettings(object parameter)
        {
            // Open settings window/dialog
        }

        private void Search(object parameter)
        {
            // Perform search
        }

        private bool CanSearch(object parameter) => !string.IsNullOrWhiteSpace(SearchText);

        private void Refresh(object parameter)
        {
            // Refresh data
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}