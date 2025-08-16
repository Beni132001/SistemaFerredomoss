using SistemaFerredomos.src.Repositories.Commons;
using SistemaFerredomos.src.Repositories.LoginAuth;
using SistemaFerredomos.src.Views.Login;
using System.Configuration;
using System.Data;
using System.Windows;


namespace SistemaFerredomos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Configurar dependencias
                var databaseService = new DatabaseService();
                IUserRepository userRepository = new UserRepository(databaseService);

                // Crear y mostrar la ventana de login
                var loginView = new LoginView(userRepository);
                loginView.Show();
            }
            catch (Exception ex)
            {
                // Manejar errores de inicialización
                MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}",
                                "Error crítico",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
