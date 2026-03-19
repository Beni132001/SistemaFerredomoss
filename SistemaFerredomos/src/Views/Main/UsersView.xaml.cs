using SistemaFerredomos.src.ViewModels.Main;
using System.Windows.Controls;

namespace SistemaFerredomos.src.Views.Main
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        private void PasswordInput_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UsersViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}