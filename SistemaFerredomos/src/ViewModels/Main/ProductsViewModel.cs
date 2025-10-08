using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly ProductsRepository _repository;

        public ObservableCollection<ProductsModel> Products { get; set; } = new ObservableCollection<ProductsModel>();

        private ProductsModel _selectedProduct;
        public ProductsModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ProductsViewModel(ProductsRepository repository = null)
        {
            _repository = repository ?? new ProductsRepository();

            LoadProducts();

            AddCommand = new RelayCommand(o => OpenAddEditView(null));
            EditCommand = new RelayCommand(o => OpenAddEditView(SelectedProduct), o => SelectedProduct != null);
            DeleteCommand = new RelayCommand(o => DeleteProduct(), o => SelectedProduct != null);
        }

        private void LoadProducts()
        {
            Products.Clear();
            var list = _repository.GetAll();
            foreach (var p in list)
                Products.Add(p);
        }

        private void OpenAddEditView(ProductsModel product)
        {
            var addEditVM = new AddEditProductViewModel(product, RefreshProducts, _repository);

            var addEditView = new Views.Main.AddEditProductView
            {
                DataContext = addEditVM
            };

            var window = new Window
            {
                Title = product == null ? "Agregar Producto" : "Editar Producto",
                Content = addEditView,
                Height = 400,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }

        private void RefreshProducts()
        {
            LoadProducts();
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show($"¿Deseas eliminar el producto '{SelectedProduct.Name}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedProduct.Id))
                {
                    MessageBox.Show("✅ Producto eliminado correctamente");
                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar producto");
                }
            }
        }
    }
}
