using SistemaFerredomos.src.Views.Splash;
using System;
using System.Windows;

namespace SistemaFerredomos
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var splash = new SplashView();
                splash.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}",
                                "Error crítico",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}