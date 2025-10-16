using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.Services;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class AddEditProductViewModel : BaseViewModel
    {
        private readonly ProductsRepository _repository;
        private readonly ImageService _imageService;
        private readonly Action _onSave;
        private readonly Action _onCancel;

        private BitmapImage _productImage;
        private string _imageFileName;
        private string _name;
        private int _stock;
        private decimal _purchasePrice;
        private decimal _salePrice;
        private SupplierModel _selectedSupplier;

        public AddEditProductViewModel(ProductsModel product = null, Action onSave = null, Action onCancel = null, ProductsRepository repository = null)
        {
            _repository = repository ?? new ProductsRepository();
            _imageService = new ImageService();
            _onSave = onSave;
            _onCancel = onCancel;

            Suppliers = new ObservableCollection<SupplierModel>(_repository.GetSuppliers());

            // Inicializar comandos
            SaveCommand = new RelayCommand(o => SaveProduct(), o => CanSave());
            CancelCommand = new RelayCommand(o => _onCancel?.Invoke());
            SelectImageCommand = new RelayCommand(o => SelectImage());
            RemoveImageCommand = new RelayCommand(o => RemoveImage());

            // Establecer imagen por defecto inmediatamente
            ProductImage = _imageService.GetDefaultImage();

            // Si es edición, cargar datos existentes
            if (product != null)
            {
                ProductId = product.Id;
                Name = product.Name;
                Stock = product.Stock;
                PurchasePrice = product.PurchasePrice;
                SalePrice = product.SalePrice;
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == product.SupplierId);
                _imageFileName = product.Image;
                ImageFileName = product.Image;

                // Cargar imagen si existe
                if (!string.IsNullOrEmpty(product.Image))
                {
                    ProductImage = _imageService.LoadImage(product.Image);
                }
            }
        }

        public int ProductId { get; set; }
        public ObservableCollection<SupplierModel> Suppliers { get; set; }

        // Propiedades para binding
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int Stock
        {
            get => _stock;
            set
            {
                if (SetProperty(ref _stock, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set
            {
                if (SetProperty(ref _purchasePrice, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal SalePrice
        {
            get => _salePrice;
            set
            {
                if (SetProperty(ref _salePrice, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public SupplierModel SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (SetProperty(ref _selectedSupplier, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public BitmapImage ProductImage
        {
            get => _productImage;
            set => SetProperty(ref _productImage, value);
        }

        public string ImageFileName
        {
            get => _imageFileName;
            set => SetProperty(ref _imageFileName, value);
        }

        public string Title => ProductId == 0 ? "AGREGAR NUEVO PRODUCTO" : "EDITAR PRODUCTO";

        // Comandos
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand RemoveImageCommand { get; set; }

        private void SelectImage()
        {
            try
            {
                var newImageFileName = _imageService.SelectAndSaveImage();
                if (!string.IsNullOrEmpty(newImageFileName))
                {
                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(_imageFileName))
                    {
                        _imageService.DeleteImage(_imageFileName);
                    }

                    _imageFileName = newImageFileName;
                    ImageFileName = _imageFileName;
                    ProductImage = _imageService.LoadImage(_imageFileName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al seleccionar imagen: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void RemoveImage()
        {
            if (!string.IsNullOrEmpty(_imageFileName))
            {
                _imageService.DeleteImage(_imageFileName);
                _imageFileName = null;
                ImageFileName = null;
            }
            ProductImage = _imageService.GetDefaultImage();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   SelectedSupplier != null &&
                   Stock >= 0 &&
                   PurchasePrice >= 0 &&
                   SalePrice >= 0;
        }

        private void SaveProduct()
        {
            try
            {
                var product = new ProductsModel
                {
                    Id = ProductId,
                    Name = Name.Trim(),
                    Stock = Stock,
                    PurchasePrice = PurchasePrice,
                    SalePrice = SalePrice,
                    SupplierId = SelectedSupplier?.Id ?? 0,
                    Image = _imageFileName
                };

                bool success = ProductId == 0 ?
                    _repository.Add(product) :
                    _repository.Update(product);

                if (success)
                {
                    System.Windows.MessageBox.Show("✅ Producto guardado correctamente", "Éxito",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    _onSave?.Invoke();
                }
                else
                {
                    System.Windows.MessageBox.Show("❌ Error al guardar producto", "Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"❌ Error: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}