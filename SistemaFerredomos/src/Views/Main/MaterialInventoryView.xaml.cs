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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaFerredomos.src.Views.Main
{
    /// <summary>
    /// Lógica de interacción para MaterialInventoryView.xaml
    /// </summary>
    public partial class MaterialInventoryView : UserControl
    {
        public MaterialInventoryView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MaterialInventoryViewModel vm)
            {
                // Oculta la columna de acciones, el botón de agregar y precio compra si el usuario NO es admin
                if (!vm.IsAdmin)
                {
                    colAcciones.Visibility = Visibility.Collapsed;
                    btnAgregarMaterial.Visibility = Visibility.Collapsed;

                    // Oculta la columna "Precio Compra" - necesitas darle un x:Name en XAML
                    var precioCompraColumn = dgMaterials.Columns
                        .OfType<DataGridTextColumn>()
                        .FirstOrDefault(c => c.Header?.ToString() == "Precio Compra");

                    if (precioCompraColumn != null)
                    {
                        precioCompraColumn.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}