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

            MenuItems.Add(new NavItem { Title = "Órdenes", ViewType = NavViewType.Orders });
            MenuItems.Add(new NavItem { Title = "Pedidos", ViewType = NavViewType.POrders });
            MenuItems.Add(new NavItem { Title = "Invetarios", ViewType = NavViewType.Inventorys });
            MenuItems.Add(new NavItem { Title = "Diseños", ViewType = NavViewType.Desings});

            if (_currentUser.IsAdmin)
            {
                MenuItems.Add(new NavItem { Title = "Agregar", ViewType = NavViewType.Add });
                MenuItems.Add(new NavItem { Title = "Actividad", ViewType = NavViewType.Activity });
                MenuItems.Add(new NavItem { Title = "Opciones", ViewType = NavViewType.Options });
            }

            
        }

        private void Navigate(object parameter)
        {
            if (parameter is NavViewType viewType)
            {
                CurrentView = viewType switch
                {
                    NavViewType.Home => new HomeViewModel(),
                    NavViewType.Orders => new OrdersViewModel(),
                    NavViewType.POrders => new OrdersViewModel(),
                    NavViewType.Inventorys => new OrdersViewModel(),
                    NavViewType.Desings => new OrdersViewModel(),
                    NavViewType.Add => new ProductsViewModel(),
                    NavViewType.Activity => new SupplierViewModel(),
                    NavViewType.Options => new OrdersViewModel(),

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
       Orders,
       POrders,
       Inventorys,
       Desings,
       Add,
       Activity,
       Options
       
      
    }
}
