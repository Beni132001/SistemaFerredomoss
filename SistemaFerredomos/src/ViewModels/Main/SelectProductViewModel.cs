using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class SelectProductViewModel : BaseViewModel
    {
        private readonly ProductsRepository _productsRepository;

        public ObservableCollection<ProductsModel> Products { get; set; }
        public ObservableCollection<ProductsModel> FilteredProducts { get; set; }

        public ProductsModel SelectedProduct { get; set; }

        public ICommand SelectProductCommand { get; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterProducts();
            }
        }

        public SelectProductViewModel()
        {
            _productsRepository = new ProductsRepository();

            var products = _productsRepository.GetAll();

            Products = new ObservableCollection<ProductsModel>(products);
            FilteredProducts = new ObservableCollection<ProductsModel>(products);
        }

        private void SelectProduct(object obj)
        {
            // aquí luego regresaremos el producto seleccionado
        }
        

        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredProducts = new ObservableCollection<ProductsModel>(Products);
            }
            else
            {
                var filtered = Products
                    .Where(p => p.Name.ToLower().Contains(SearchText.ToLower()))
                    .ToList();

                FilteredProducts = new ObservableCollection<ProductsModel>(filtered);
            }

            OnPropertyChanged(nameof(FilteredProducts));
        }
    }
}