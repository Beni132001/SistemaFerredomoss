using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.LoginAuth;
using SistemaFerredomos.src.ViewModels.LoginViewModel;
using SistemaFerredomos.src.ViewModels.Main;
using SistemaFerredomos.src.Views.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SistemaFerredomos.src.Views.Login
{
    /// <summary>
    /// Lógica de interacción para LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly IUserRepository _userRepository;
        public LoginView(IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;
            DataContext = new LoginViewModel(userRepository);
            ((LoginViewModel)DataContext).LoginSuccessful += OnLoginSuccessful;
        }

        private void OnLoginSuccessful(UserModel user)
        {
            // Aquí crearías la ventana principal según el tipo de usuario
            var mainView = new MainView(user);
            mainView.Show();
            this.Close();
            //cerrar sesion
            if (mainView.DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.LogoutRequested += (s, e) =>
                {
                    mainView.Close();
                    var loginView = new LoginView(_userRepository);
                    loginView.Show();
                };

                
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((LoginViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
