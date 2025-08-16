using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class MainViewModel : BaseViewModel
    {
        private readonly UserModel _currentUser;
        private BaseViewModel _currentView;

        public ICommand LogoutCommand { get; }

        public event EventHandler LogoutRequested;

        private void Logout(object parameter)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ObservableCollection<NavItem> MenuItems { get; } = new ObservableCollection<NavItem>();

        public ICommand NavigateCommand { get; }

        public MainViewModel(UserModel user)
        {
            _currentUser = user;
            NavigateCommand = new RelayCommand(Navigate);

            // Construir menú según tipo de usuario
            BuildMenu();

            // Vista inicial
            CurrentView = new HomeViewModel();

            LogoutCommand = new RelayCommand(Logout);   
        }

        private void BuildMenu()
        {
            MenuItems.Add(new NavItem { Title = "Inicio", ViewType = NavViewType.Home });

            if (_currentUser.IsAdmin)
            {
                MenuItems.Add(new NavItem { Title = "Porductos", ViewType = NavViewType.Products });
                MenuItems.Add(new NavItem { Title = "Proveedores", ViewType = NavViewType.Supplier });
            }

            MenuItems.Add(new NavItem { Title = "Órdenes", ViewType = NavViewType.Orders });
            MenuItems.Add(new NavItem { Title = "Produccion", ViewType = NavViewType.Production });
        }

        private void Navigate(object parameter)
        {
            if (parameter is NavViewType viewType)
            {
                CurrentView = viewType switch
                {
                    NavViewType.Home => new HomeViewModel(),
                    NavViewType.Products => new ProductsViewModel(),
                    NavViewType.Supplier => new SupplierViewModel(),
                    NavViewType.Orders=> new OrdersViewModel(),
                    NavViewType.Production => new ProductionViewModel(),
                    _ => new HomeViewModel()
                };
            }
        }
    }

    public class NavItem
    {
        public string Title { get; set; }
        public NavViewType ViewType { get; set; }
    }

    public enum NavViewType
    {
       Home,
       Products,
       Supplier,
       Orders, 
       Production
        
    }
}
