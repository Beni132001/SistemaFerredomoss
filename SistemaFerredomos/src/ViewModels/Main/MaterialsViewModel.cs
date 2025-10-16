using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class MaterialsViewModel : BaseViewModel
    {
        private readonly MaterialRepository _repository;

        public ObservableCollection<MaterialModel> Materials { get; set; } = new ObservableCollection<MaterialModel>();

        private MaterialModel _selectedMaterial;
        public MaterialModel SelectedMaterial
        {
            get => _selectedMaterial;
            set => SetProperty(ref _selectedMaterial, value);
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public MaterialsViewModel(MaterialRepository repository = null)
        {
            _repository = repository ?? new MaterialRepository();

            LoadMaterials();

            AddCommand = new RelayCommand(o => OpenAddEditView(null));
            EditCommand = new RelayCommand(o => OpenAddEditView(SelectedMaterial), o => SelectedMaterial != null);
            DeleteCommand = new RelayCommand(o => DeleteMaterial(), o => SelectedMaterial != null);
        }

        private void LoadMaterials()
        {
            Materials.Clear();
            var list = _repository.GetAll();
            foreach (var m in list)
                Materials.Add(m);
        }

        private void OpenAddEditView(MaterialModel material)
        {
            Window window = null;

            // Crear VM del Add/Edit con ambos callbacks
            var addEditVM = new AddEditMaterialViewModel(
                material: material,
                onSave: () =>
                {
                    RefreshMaterials();
                    window?.Close(); // Cerrar ventana al guardar
                },
                onCancel: () =>
                {
                    window?.Close(); // Cerrar ventana al cancelar
                },
                repository: _repository
            );

            // Mostrar control de usuario dentro de la vista
            var addEditView = new Views.Main.AddEditMaterialView
            {
                DataContext = addEditVM
            };

            // Abrir como ventana modal
            window = new Window
            {
                Title = material == null ? "Agregar Material" : "Editar Material",
                Content = addEditView,
                Height = 400,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }

        private void RefreshMaterials()
        {
            LoadMaterials();
        }

        private void DeleteMaterial()
        {
            if (SelectedMaterial == null) return;

            var result = MessageBox.Show($"¿Deseas eliminar el material '{SelectedMaterial.Name}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedMaterial.Id))
                {
                    MessageBox.Show("✅ Material eliminado correctamente");
                    LoadMaterials();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar material");
                }
            }
        }
    }
}
