using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Services;
using SistemaFerredomos.src.Repositories.LoginAuth;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.LoginViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IUserRepository _userRepository;
        private string _username = "";
        private string _password = "";
        private string _errorMessage = null;
        private string _usernameError;
        private string _passwordError;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); ValidateUsername(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); ValidatePassword(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        // Controla visibilidad del mensaje de error
        public bool HasError => !string.IsNullOrWhiteSpace(_errorMessage);

        public string UsernameError
        {
            get => _usernameError;
            set { _usernameError = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasUsernameError)); }
        }

        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasPasswordError)); }
        }

        public bool HasUsernameError => !string.IsNullOrWhiteSpace(_usernameError);
        public bool HasPasswordError => !string.IsNullOrWhiteSpace(_passwordError);

        public ICommand LoginCommand { get; }
        public event Action<UserModel> LoginSuccessful;

        public LoginViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private void ValidateUsername()
        {
            UsernameError = ValidationHelper.GetUsernameError(Username);
            CommandManager.InvalidateRequerySuggested();
        }

        private void ValidatePassword()
        {
            PasswordError = Password.Length < 4 ? "Mínimo 4 caracteres" : "";
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanExecuteLogin(object parameter)
            => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !HasUsernameError && !HasPasswordError;

        private void ExecuteLogin(object parameter)
        {
            ValidateUsername();
            ValidatePassword();
            if (HasUsernameError || HasPasswordError) return;
            try
            {
                ErrorMessage = "";
                var user = _userRepository.Authenticate(Username.Trim(), Password);

                if (user != null)
                {
                    ErrorMessage = null;
                    LoginSuccessful?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Usuario o contraseña incorrectos";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error de conexión: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}