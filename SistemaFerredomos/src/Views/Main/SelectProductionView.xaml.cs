using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.ViewModels.Main;
using System.Windows;

namespace SistemaFerredomos.src.Views.Main
{
    public partial class SelectProductionView : Window
    {
        public ProductionModel SelectedProduction { get; private set; }

        public SelectProductionView()
        {
            InitializeComponent();
            DataContext = new SelectProductionViewModel();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SelectProductionViewModel;

            if (vm.SelectedProduction == null)
            {
                MessageBox.Show("Seleccione una producción.");
                return;
            }

            SelectedProduction = vm.SelectedProduction;

            DialogResult = true;
        }
    }
}