using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class GlassViewModel : BaseViewModel
    {
        private readonly GlassRepository _repository;

        public ObservableCollection<GlassModel> GlassList { get; set; } = new ObservableCollection<GlassModel>();

        // Vidrio seleccionado en la tabla
        private GlassModel _selectedGlass;
        public GlassModel SelectedGlass
        {
            get => _selectedGlass;
            set => SetProperty(ref _selectedGlass, value);
        }

        // Campos del formulario
        private string _code;
        public string Code
        {
            get => _code;
            set { if (SetProperty(ref _code, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private double _thickness;
        public double Thickness
        {
            get => _thickness;
            set => SetProperty(ref _thickness, value);
        }

        // Controla si el formulario está visible
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        // Título dinámico del formulario
        private bool _isEditing;
        public string FormTitle => _isEditing ? "EDITAR VIDRIO" : "AGREGAR VIDRIO";

        // Commands
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public GlassViewModel(GlassRepository repository = null)
        {
            _repository = repository ?? new GlassRepository();

            LoadGlass();

            AddCommand = new RelayCommand(o => OpenForm(null));
            EditCommand = new RelayCommand(o => OpenForm(SelectedGlass), o => SelectedGlass != null);
            DeleteCommand = new RelayCommand(o => DeleteGlass(), o => SelectedGlass != null);
            SaveCommand = new RelayCommand(o => SaveGlass(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
        }

        private void LoadGlass()
        {
            GlassList.Clear();
            foreach (var g in _repository.GetAll())
                GlassList.Add(g);
        }

        private void OpenForm(GlassModel glass)
        {
            _isEditing = glass != null;

            if (glass != null)
            {
                Code = glass.Code;
                Name = glass.Name;
                Width = glass.Width;
                Height = glass.Height;
                Thickness = glass.Thickness;
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
            Code = string.Empty;
            Name = string.Empty;
            Width = 0;
            Height = 0;
            Thickness = 0;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Code) &&
                   !string.IsNullOrWhiteSpace(Name);
        }

        private void SaveGlass()
        {
            var glass = new GlassModel
            {
                Code = Code.Trim(),
                Name = Name.Trim(),
                Width = Width,
                Height = Height,
                Thickness = Thickness
            };

            bool success = _isEditing
                ? _repository.Update(glass)
                : _repository.Add(glass);

            if (success)
            {
                MessageBox.Show("✅ Vidrio guardado correctamente");
                LoadGlass();
                CloseForm();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar vidrio");
            }
        }

        private void DeleteGlass()
        {
            if (SelectedGlass == null) return;

            var result = MessageBox.Show(
                $"¿Deseas eliminar el vidrio '{SelectedGlass.Name}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedGlass.Code))
                {
                    MessageBox.Show("✅ Vidrio eliminado correctamente");
                    LoadGlass();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar vidrio");
                }
            }
        }
    }
}