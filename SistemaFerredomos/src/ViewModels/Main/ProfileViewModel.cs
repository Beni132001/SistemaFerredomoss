using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ProfileRepository _repository;

        public ObservableCollection<ProfileModel> ProfileList { get; set; } = new ObservableCollection<ProfileModel>();

        // Perfil seleccionado en la tabla
        private ProfileModel _selectedProfile;
        public ProfileModel SelectedProfile
        {
            get => _selectedProfile;
            set => SetProperty(ref _selectedProfile, value);
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

        private double _size;
        public double Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        // Controla visibilidad del formulario lateral
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        // Título dinámico del formulario
        private bool _isEditing;
        public string FormTitle => _isEditing ? "EDITAR PERFIL" : "AGREGAR PERFIL";

        // Commands
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProfileViewModel(ProfileRepository repository = null)
        {
            _repository = repository ?? new ProfileRepository();

            LoadProfiles();

            AddCommand = new RelayCommand(o => OpenForm(null));
            EditCommand = new RelayCommand(o => OpenForm(SelectedProfile), o => SelectedProfile != null);
            DeleteCommand = new RelayCommand(o => DeleteProfile(), o => SelectedProfile != null);
            SaveCommand = new RelayCommand(o => SaveProfile(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
        }

        public void LoadProfiles()
        {
            ProfileList.Clear();
            foreach (var p in _repository.GetAll())
                ProfileList.Add(p);
        }

        private void OpenForm(ProfileModel profile)
        {
            _isEditing = profile != null;

            if (profile != null)
            {
                Code = profile.Code;
                Name = profile.Name;
                Size = profile.Size;
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
            Size = 0;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Code) &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   Size > 0;
        }

        private void SaveProfile()
        {
            var profile = new ProfileModel
            {
                Code = Code.Trim(),
                Name = Name.Trim(),
                Size = Size
            };

            bool success = _isEditing
                ? _repository.Update(profile)
                : _repository.Add(profile);

            if (success)
            {
                MessageBox.Show("✅ Perfil guardado correctamente");
                LoadProfiles();
                CloseForm();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar perfil");
            }
        }

        private void DeleteProfile()
        {
            if (SelectedProfile == null) return;

            var result = MessageBox.Show(
                $"¿Deseas eliminar el perfil '{SelectedProfile.Name}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedProfile.Code))
                {
                    MessageBox.Show("✅ Perfil eliminado correctamente");
                    LoadProfiles();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar perfil. Verifique que no esté en uso.");
                }
            }
        }
    }
}