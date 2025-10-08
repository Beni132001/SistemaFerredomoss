using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class MaterialInventoryViewModel : BaseViewModel
    {
        private readonly MaterialRepository _repository;
        private string _searchText;
        private bool _isAdmin;

        public MaterialInventoryViewModel(MaterialRepository repository = null, bool isAdmin = false)
        {
            _repository = repository ?? new MaterialRepository();
            _isAdmin = isAdmin;
            Materials = new ObservableCollection<MaterialModel>(_repository.GetAll());
            FilteredMaterials = new ObservableCollection<MaterialModel>(Materials);

            AddCommand = new RelayCommand(o => OpenAddEditMaterial(null), _ => IsAdmin);
            EditCommand = new RelayCommand(o =>
            {
                if (o is MaterialModel m) OpenAddEditMaterial(m);
            }, _ => IsAdmin);

            DeleteCommand = new RelayCommand(o =>
            {
                if (o is MaterialModel m) DeleteMaterial(m);
            }, _ => IsAdmin);
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }

        // Lista de materiales
        public ObservableCollection<MaterialModel> Materials { get; set; }
        public ObservableCollection<MaterialModel> FilteredMaterials { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredMaterials = new ObservableCollection<MaterialModel>(Materials);
            }
            else
            {
                var filtered = Materials.Where(m => m.Name.ToLower().Contains(SearchText.ToLower()));
                FilteredMaterials = new ObservableCollection<MaterialModel>(filtered);
            }
            OnPropertyChanged(nameof(FilteredMaterials));
        }

        private MaterialModel _selectedMaterial;
        public MaterialModel SelectedMaterial
        {
            get => _selectedMaterial;
            set => SetProperty(ref _selectedMaterial, value);
        }

        // Comandos
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        // Abrir ventana de agregar/editar
        private void OpenAddEditMaterial(MaterialModel material)
        {
            var vm = new AddEditMaterialViewModel(material, LoadMaterials, _repository);
            var view = new AddEditMaterialView { DataContext = vm };

            var window = new System.Windows.Window
            {
                Content = view,
                Title = material == null ? "Agregar Material" : "Editar Material",
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        // Recargar lista
        public void LoadMaterials()
        {
            Materials.Clear();
            foreach (var m in _repository.GetAll())
                Materials.Add(m);
            ApplyFilter();
        }

        // Eliminar material
        private void DeleteMaterial(MaterialModel material)
        {
            if (System.Windows.MessageBox.Show($"¿Deseas eliminar el material {material.Name}?",
                "Confirmar eliminación", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                if (_repository.Delete(material.Id))
                {
                    System.Windows.MessageBox.Show("✅ Material eliminado correctamente");
                    LoadMaterials();
                }
                else
                    System.Windows.MessageBox.Show("❌ Error al eliminar material");
            }
        }
    }
}
