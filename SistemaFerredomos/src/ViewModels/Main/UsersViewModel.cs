using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly UsersRepository _repository;

        public ObservableCollection<UserModel> Users { get; set; } = new();

        private UserModel _selectedUser;
        public UserModel SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        // Campos del formulario
        private string _name;
        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { if (SetProperty(ref _username, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { if (SetProperty(ref _password, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _selectedType;
        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        public ObservableCollection<string> UserTypes { get; } = new() { "admin", "user" };

        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        private bool _isEditing;
        public string FormTitle => _isEditing ? "EDITAR USUARIO" : "AGREGAR USUARIO";

        // Commands
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UsersViewModel(UsersRepository repository = null)
        {
            _repository = repository ?? new UsersRepository();

            LoadUsers();

            AddCommand = new RelayCommand(o => OpenForm(null));
            EditCommand = new RelayCommand(o => OpenForm(SelectedUser), o => SelectedUser != null);
            DeleteCommand = new RelayCommand(o => DeleteUser(), o => SelectedUser != null);
            SaveCommand = new RelayCommand(o => SaveUser(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
        }

        private void LoadUsers()
        {
            Users.Clear();
            foreach (var u in _repository.GetAll())
                Users.Add(u);
        }

        private void OpenForm(UserModel user)
        {
            _isEditing = user != null;

            if (user != null)
            {
                Name = user.Name;
                Username = user.UserName;
                Password = "";
                SelectedType = user.Type.ToString();
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
            Username = string.Empty;
            Password = string.Empty;
            SelectedType = "user";
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   (!_isEditing || !string.IsNullOrWhiteSpace(Password));
        }

        private void SaveUser()
        {
            var user = new UserModel
            {
                Id = _isEditing ? SelectedUser.Id : 0,
                Name = Name.Trim(),
                UserName = Username.Trim(),
                Password = Password?.Trim(),
                Type = SelectedType == "admin" ? UserType.admin : UserType.user
            };

            bool success = _isEditing
                ? _repository.Update(user)
                : _repository.Add(user);

            if (success)
            {
                MessageBox.Show("✅ Usuario guardado correctamente");
                LoadUsers();
                CloseForm();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar usuario. Verifica que el nombre de usuario no exista.");
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"¿Eliminar al usuario '{SelectedUser.Name}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedUser.Id))
                {
                    MessageBox.Show("✅ Usuario eliminado correctamente");
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("❌ No se puede eliminar. El usuario tiene actividad registrada.");
                }
            }
        }
    }
}