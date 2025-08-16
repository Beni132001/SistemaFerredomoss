using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.ViewModels.Main;
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

namespace SistemaFerredomos.src.Views.Main
{
    /// <summary>
    /// Lógica de interacción para MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly UserModel _currentUser;

        public MainView(UserModel user)
        {
            InitializeComponent();
            DataContext = new MainViewModel(user);

            
        }
    }
}
