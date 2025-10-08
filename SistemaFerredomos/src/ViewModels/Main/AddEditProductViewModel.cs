using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class AddEditProductViewModel : BaseViewModel
    {
        private readonly ProductsRepository _repository;

        public AddEditProductViewModel(ProductsModel product, Action onSave, ProductsRepository repository = null)
        {
            _repository = repository ?? new ProductsRepository();

            Suppliers = new ObservableCollection<SupplierModel>(_repository.GetSuppliers());

            SaveCommand = new RelayCommand(o =>
            {
                SaveProduct();
                onSave?.Invoke();
            }, o => CanSave());

            // Inicializar campos si es edición
            if (product != null)
            {
                ProductId = product.Id;
                Name = product.Name;
                Stock = product.Stock;
                PurchasePrice = product.PurchasePrice;
                SalePrice = product.SalePrice;
                Image = product.Image;
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
            }
        }

        // Propiedades para binding
        public ObservableCollection<SupplierModel> Suppliers { get; set; }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private decimal _stock;
        public decimal Stock { get => _stock; set => SetProperty(ref _stock, value); }

        private decimal _purchasePrice;
        public decimal PurchasePrice { get => _purchasePrice; set => SetProperty(ref _purchasePrice, value); }

        private decimal _salePrice;
        public decimal SalePrice { get => _salePrice; set => SetProperty(ref _salePrice, value); }

        private SupplierModel _selectedSupplier;
        public SupplierModel SelectedSupplier { get => _selectedSupplier; set => SetProperty(ref _selectedSupplier, value); }

        private string _image;
        public string Image { get => _image; set => SetProperty(ref _image, value); }

        public int ProductId { get; set; } // Para edición

        public ICommand SaveCommand { get; set; }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name) && SelectedSupplier != null;

        private void SaveProduct()
        {
            var product = new ProductsModel
            {
                Id = ProductId,
                Name = Name,
                Stock = Stock,
                PurchasePrice = PurchasePrice,
                SalePrice = SalePrice,
                SupplierId = SelectedSupplier?.Id ?? 0,
                Image = Image
            };

            bool success;
            if (ProductId == 0)
                success = _repository.Add(product);
            else
                success = _repository.Update(product);

            if (success)
                System.Windows.MessageBox.Show("✅ Producto guardado correctamente");
            else
                System.Windows.MessageBox.Show("❌ Error al guardar producto");
        }
    }
}
