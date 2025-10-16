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
    public class AddEditMaterialViewModel : BaseViewModel
    {
        private readonly MaterialRepository _repository;
        private readonly ImageService _imageService;
        private readonly Action _onSave;
        private readonly Action _onCancel;

        private BitmapImage _materialImage;
        private string _imageFileName;

        public string ImageFileName
        {
            get => _imageFileName;
            set => SetProperty(ref _imageFileName, value);
        }

        public AddEditMaterialViewModel(MaterialModel material = null, Action onSave = null, Action onCancel = null, MaterialRepository repository = null)
        {
            _repository = repository ?? new MaterialRepository();
            _imageService = new ImageService();
            _onSave = onSave;
            _onCancel = onCancel;

            Suppliers = new ObservableCollection<SupplierModel>(_repository.GetSuppliers());

            SaveCommand = new RelayCommand(
                o => SaveMaterial(),
                o => CanSave()
            );

            CancelCommand = new RelayCommand(o => _onCancel?.Invoke());
            SelectImageCommand = new RelayCommand(o => SelectImage());
            RemoveImageCommand = new RelayCommand(o => RemoveImage());

            // Inicializa campos si es edición
            if (material != null)
            {
                MaterialId = material.Id;
                Name = material.Name;
                Stock = material.Stock;
                PurchasePrice = material.PurchasePrice;
                SalePrice = material.SalePrice;
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == material.SupplierId);
                _imageFileName = material.Image;

                // Cargar imagen si existe
                if (!string.IsNullOrEmpty(material.Image))
                {
                    MaterialImage = _imageService.LoadImage(material.Image);
                }
                else
                {
                    MaterialImage = _imageService.GetDefaultImage();
                }
            }
            // Imagen por defecto si no hay imagen
            if (MaterialImage == null)
            {
                MaterialImage = _imageService.GetDefaultImage();
            }
        }

        public string Title => MaterialId == 0 ? "AGREGAR NUEVO MATERIAL" : "EDITAR MATERIAL";
        public int MaterialId { get; set; }
        public ObservableCollection<SupplierModel> Suppliers { get; set; }

        // Propiedades para el binding
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    // Forzar reevaluación de CanExecute
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private decimal _stock;
        public decimal Stock
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

        private decimal _purchasePrice;
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

        private decimal _salePrice;
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

        private SupplierModel _selectedSupplier;
        public SupplierModel SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (SetProperty(ref _selectedSupplier, value))
                {
                    System.Diagnostics.Debug.WriteLine($"Proveedor seleccionado: {value?.Name}");
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public BitmapImage MaterialImage
        {
            get => _materialImage;
            set => SetProperty(ref _materialImage, value);
        }

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
                    MaterialImage = _imageService.LoadImage(_imageFileName);
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
            MaterialImage = _imageService.GetDefaultImage();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   SelectedSupplier != null &&
                   Stock >= 0 &&
                   PurchasePrice >= 0 &&
                   SalePrice >= 0;
        }

        private void SaveMaterial()
        {
            try
            {
                var material = new MaterialModel
                {
                    Id = MaterialId,
                    Name = Name.Trim(),
                    Stock = Stock,
                    PurchasePrice = PurchasePrice,
                    SalePrice = SalePrice,
                    SupplierId = SelectedSupplier?.Id ?? 0, 
                    Image = _imageFileName
                };

                bool success = MaterialId == 0 ?
                    _repository.Add(material) :
                    _repository.Update(material);

                if (success)
                {
                    System.Windows.MessageBox.Show("✅ Material guardado correctamente");
                    _onSave?.Invoke();
                }
                else
                {
                    System.Windows.MessageBox.Show("❌ Error al guardar material");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"❌ Error: {ex.Message}");
            }
        }
    }
}