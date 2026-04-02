using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class SupplierViewModel : BaseViewModel
    {
        private readonly SupplierRepository _repository;

        public ObservableCollection<SupplierModel> Suppliers { get; set; } = new();

        private SupplierModel _selectedSupplier;
        public SupplierModel SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (SetProperty(ref _search, value))
                    FilterSuppliers();
            }
        }

        private void FilterSuppliers()
        {
            var list = _repository.GetAll()
                .Where(s => s.Name.ToLower().Contains(Search?.ToLower() ?? ""));

            Suppliers.Clear();
            foreach (var s in list)
                Suppliers.Add(s);
        }

        // Campos del formulario
        private string _name;
        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        private bool _isEditing;
        public string FormTitle => _isEditing ? "EDITAR PROVEEDOR" : "AGREGAR PROVEEDOR";

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public SupplierViewModel(SupplierRepository repository = null)
        {
            _repository = repository ?? new SupplierRepository();

            LoadSuppliers();

            AddCommand = new RelayCommand(o => OpenForm(null));
            EditCommand = new RelayCommand(o => OpenForm(SelectedSupplier), o => SelectedSupplier != null);
            DeleteCommand = new RelayCommand(o => DeleteSupplier(), o => SelectedSupplier != null);
            SaveCommand = new RelayCommand(o => SaveSupplier(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
        }

        private void LoadSuppliers()
        {
            Suppliers.Clear();
            foreach (var s in _repository.GetAll())
                Suppliers.Add(s);
        }

        private void OpenForm(SupplierModel supplier)
        {
            _isEditing = supplier != null;

            if (supplier != null)
            {
                Name = supplier.Name;
                Phone = supplier.Phone;
                Address = supplier.Address;
            }
            else
            {
                ClearForm();
            }

            IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void CloseForm()
        {
            IsFormVisible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && Name.Length >= 3;
        }

        private void SaveSupplier()
        {
            var supplier = new SupplierModel
            {
                Id = _isEditing ? SelectedSupplier.Id : 0,
                Name = Name.Trim(),
                Phone = Phone?.Trim(),
                Address = Address?.Trim()
            };

            bool success = _isEditing
                ? _repository.Update(supplier)
                : _repository.Add(supplier);

            if (success)
            {
                MessageBox.Show("✅ Proveedor guardado correctamente");
                LoadSuppliers();
                CloseForm();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar proveedor");
            }
        }

        private void DeleteSupplier()
        {
            if (SelectedSupplier == null) return;

            var result = MessageBox.Show(
                $"¿Eliminar al proveedor '{SelectedSupplier.Name}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.HasDependencies(SelectedSupplier.Id))
                {
                    MessageBox.Show("No puedes eliminar este proveedor porque tiene materiales asociados.");
                    return;
                }
                if (_repository.Delete(SelectedSupplier.Id))
                {
                    MessageBox.Show("✅ Proveedor eliminado correctamente");
                    LoadSuppliers();
                }
                else
                {
                    MessageBox.Show("❌ No se puede eliminar. El proveedor tiene materiales o pedidos asociados.");
                }
            }
        }
    }
}