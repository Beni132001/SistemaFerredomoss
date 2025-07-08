using System.Configuration;
using System.Data;
using System.Windows;
using SistemaFerredomos.src.Views.Login;

namespace SistemaFerredomos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var login = new LoginView();
            login.Show();
        }
    }

}
