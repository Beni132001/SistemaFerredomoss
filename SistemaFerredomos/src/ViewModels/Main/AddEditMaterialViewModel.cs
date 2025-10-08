using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class AddEditMaterialViewModel : BaseViewModel
    {
        private readonly MaterialRepository _repository;

        public AddEditMaterialViewModel(MaterialModel material = null, Action onSave = null, MaterialRepository repository = null)
        {
            _repository = repository ?? new MaterialRepository();

            Suppliers = new ObservableCollection<SupplierModel>(_repository.GetSuppliers());
            SaveCommand = new RelayCommand(o =>
            {
                SaveMaterial();
                onSave?.Invoke();
            }, o => CanSave());

            // Inicializa campos si es edición
            if (material != null)
            {
                MaterialId = material.Id;
                Name = material.Name;
                Stock = material.Stock;
                PurchasePrice = material.PurchasePrice;
                SalePrice = material.SalePrice;
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == material.SupplierId);
            }
        }

        // Propiedades para el binding
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

        public int MaterialId { get; set; } // Para edición

        public ICommand SaveCommand { get; set; }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name) && SelectedSupplier != null;

        private void SaveMaterial()
        {
            var material = new MaterialModel
            {
                Id = MaterialId,
                Name = Name,
                Stock = Stock,
                PurchasePrice = PurchasePrice,
                SalePrice = SalePrice,
                SupplierId = SelectedSupplier?.Id ?? 0
            };

            bool success = MaterialId == 0 ? _repository.Add(material) : _repository.Update(material);

            System.Windows.MessageBox.Show(success ? "✅ Material guardado correctamente" : "❌ Error al guardar material");
        }
    }
}
