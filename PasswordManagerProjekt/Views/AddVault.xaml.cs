using System.Windows;
using System.Windows.Controls;

namespace PwM_UI.Views
{
    public partial class AddVault : Window
    {
        public AddVault()
        {
            InitializeComponent();
            VaultNameTextBox.Focus();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(VaultNameTextBox.Text))
            {
                MessageBox.Show("Please enter a vault name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MasterPasswordBox.Password.Length < 8)
            {
                MessageBox.Show("Master password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MasterPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Add vault creation logic here
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}