using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PwM_UI.ViewModels
{
    public partial class VaultsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<VaultItem> _vaults;

        public ObservableCollection<VaultItem> Vaults
        {
            get => _vaults;
            set
            {
                _vaults = value;
                OnPropertyChanged();
            }
        }

        public VaultsViewModel()
        {
            // Initialize with sample data
            Vaults = new ObservableCollection<VaultItem>
            {
                new VaultItem { Name = "Personal", ItemCount = 24, LastModified = "2023-05-15" },
                new VaultItem { Name = "Work", ItemCount = 18, LastModified = "2023-05-10" },
                new VaultItem { Name = "Financial", ItemCount = 12, LastModified = "2023-05-01" }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VaultItem
    {
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public string LastModified { get; set; }
    }
}