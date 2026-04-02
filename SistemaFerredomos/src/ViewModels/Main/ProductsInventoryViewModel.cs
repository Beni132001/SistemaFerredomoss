using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ProductsInventoryViewModel : BaseViewModel
    {
        private readonly ProductsRepository _repository;
        private bool _isAdmin;
        private string _searchText;

        public static decimal LowStockThreshold => 5;

        public string RecordSummary => FilteredProducts.Count == Products.Count
            ? $"{Products.Count} productos"
            : $"{FilteredProducts.Count} de {Products.Count} productos";

        public ProductsInventoryViewModel(ProductsRepository repository = null, bool isAdmin = false)
        {
            _repository = repository ?? new ProductsRepository();
            _isAdmin = isAdmin;

            Products = new ObservableCollection<ProductsModel>(_repository.GetAll());
            FilteredProducts = new ObservableCollection<ProductsModel>(Products);

            AddCommand = new RelayCommand(o => OpenAddEditProduct(null), _ => IsAdmin);
            EditCommand = new RelayCommand(o =>
            {
                if (o is ProductsModel p) OpenAddEditProduct(p);
            }, _ => IsAdmin);
            DeleteCommand = new RelayCommand(o =>
            {
                if (o is ProductsModel p) DeleteProduct(p);
            }, _ => IsAdmin);
        }

        // Propiedades
        public ObservableCollection<ProductsModel> Products { get; set; }
        public ObservableCollection<ProductsModel> FilteredProducts { get; set; }

        private ProductsModel _selectedProduct;
        public ProductsModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }

        // Comandos
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private void OpenAddEditProduct(ProductsModel product)
        {
            Window window = null;

            var vm = new AddEditProductViewModel(
                product: product,
                onSave: () =>
                {
                    LoadProducts();
                    window?.Close();
                },
                onCancel: () =>
                {
                    window?.Close();
                },
                repository: _repository
            );

            var view = new AddEditProductView { DataContext = vm };

            window = new Window
            {
                Content = view,
                Title = product == null ? "Agregar Producto" : "Editar Producto",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }

        public void LoadProducts()
        {
            Products.Clear();
            foreach (var p in _repository.GetAll())
                Products.Add(p);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredProducts = new ObservableCollection<ProductsModel>(Products);
            }
            else
            {
                var query = SearchText.ToLower();
                var filtered = Products.Where(p =>
                    (p.Name ?? "").ToLower().Contains(query) ||
                    (p.Supplier?.Name ?? "").ToLower().Contains(query)
                );
                FilteredProducts = new ObservableCollection<ProductsModel>(filtered);
            }
            OnPropertyChanged(nameof(FilteredProducts));
            OnPropertyChanged(nameof(RecordSummary));
        }

        private void DeleteProduct(ProductsModel product)
        {
            if (MessageBox.Show($"¿Deseas eliminar el producto '{product.Name}'?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_repository.Delete(product.Id))
                {
                    MessageBox.Show("✅ Producto eliminado correctamente", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar el producto", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}