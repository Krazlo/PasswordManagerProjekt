using System.Windows;
using System.Windows.Controls;

namespace PwM_UI.Views
{
    public partial class AddApplication : Window
    {
        public AddApplication()
        {
            InitializeComponent();
            AppNameTextBox.Focus();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(AppNameTextBox.Text))
            {
                MessageBox.Show("Please enter an application name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text))
            {
                MessageBox.Show("Please enter an account name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AccountPasswordBox.Password.Length == 0)
            {
                MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Add application creation logic here
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