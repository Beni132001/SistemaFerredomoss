using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Repositories.Commons;
using SistemaFerredomos.src.Repositories.LoginAuth;
using SistemaFerredomos.src.Views.Login;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SistemaFerredomos.src.Views.Splash
{
    public partial class SplashView : Window
    {
        public SplashView()
        {
            InitializeComponent();
            Loaded += SplashView_Loaded;
        }

        private async void SplashView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await RunStartup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar la aplicación:\n{ex.Message}",
                    "Error crítico",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private async Task RunStartup()
        {
            // Paso 1 — Inicializando
            SetStatus("Iniciando sistema...", 10);
            await Task.Delay(400);

            // Paso 2 — Verificar conexión a BD
            SetStatus("Conectando a la base de datos...", 35);
            await Task.Delay(300);

            bool connected = await Task.Run(() => TestDatabaseConnection());

            if (!connected)
            {
                MessageBox.Show(
                    "No se pudo conectar a la base de datos.\n\n" +
                    "Verifica que MySQL esté corriendo y que las credenciales en DatabaseConfig.cs sean correctas.",
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // Paso 3 — Cargando módulos
            SetStatus("Cargando módulos...", 60);
            await Task.Delay(400);

            // Paso 4 — Preparando interfaz
            SetStatus("Preparando interfaz...", 85);
            await Task.Delay(300);

            // Paso 5 — Listo
            SetStatus("¡Listo!", 100);
            await Task.Delay(300);

            // Abrir login
            var databaseService = new DatabaseService();
            IUserRepository userRepository = new UserRepository(databaseService);

            var loginView = new LoginView(userRepository);
            loginView.Show();
            Close();
        }

        private void SetStatus(string message, int progress)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = message;
                ProgressBar.Value = progress;
                PercentText.Text = $"{progress}%";
            });
        }

        private bool TestDatabaseConnection()
        {
            try
            {
                using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}