using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ProductsInventoryViewModel : BaseViewModel
    {
        private readonly ProductsRepository _repository;
        private bool _isAdmin;

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

        // Lista de productos
        public ObservableCollection<ProductsModel> Products { get; set; }
        public ObservableCollection<ProductsModel> FilteredProducts { get; set; }

        private ProductsModel _selectedProduct;
        public ProductsModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        // buscador
        private string _searchText;
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
            var vm = new AddEditProductViewModel(product, LoadProducts);
            var view = new AddEditProductView { DataContext = vm };

            // Mostrar como ventana flotante o overlay
            var window = new System.Windows.Window
            {
                Content = view,
                Title = product == null ? "Agregar Producto" : "Editar Producto",
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
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
                var q = SearchText.ToLowerInvariant();
                var filtered = Products.Where(p => (p.Name ?? string.Empty).ToLowerInvariant().Contains(q));
                FilteredProducts = new ObservableCollection<ProductsModel>(filtered);
            }
            OnPropertyChanged(nameof(FilteredProducts));
        }

        private void DeleteProduct(ProductsModel product)
        {
            if (System.Windows.MessageBox.Show($"¿Deseas eliminar el producto {product.Name}?",
                "Confirmar eliminación", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                if (_repository.Delete(product.Id))
                {
                    System.Windows.MessageBox.Show("✅ Producto eliminado correctamente");
                    LoadProducts();
                }
                else
                    System.Windows.MessageBox.Show("❌ Error al eliminar el producto");
            }
        }
    }
}
