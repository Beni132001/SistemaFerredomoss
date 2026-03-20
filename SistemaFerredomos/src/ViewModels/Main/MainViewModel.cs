using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class MainViewModel : BaseViewModel
    {
        private readonly UserModel _currentUser;
        private BaseViewModel _currentView;
        private readonly MaterialRepository _materialRepository;

        public ICommand LogoutCommand { get; }
        public event EventHandler LogoutRequested;

        // Nombre + rol visible en el header
        public string UserDisplayName =>
            $"{_currentUser.Name}  ({(_currentUser.IsAdmin ? "Administrador" : "Taller")})";

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

        private void RefreshInventory()
        {
            if (CurrentView is MaterialInventoryViewModel materialVm)
                materialVm.LoadMaterials();
            else if (CurrentView is ProductsInventoryViewModel productVm)
                productVm.LoadProducts();
        }

        public MainViewModel(UserModel user)
        {
            _currentUser = user;
            NavigateCommand = new RelayCommand(Navigate);
            _materialRepository = new MaterialRepository();

            BuildMenu();

            CurrentView = new HomeViewModel();
            LogoutCommand = new RelayCommand(Logout);
        }

        private void BuildMenu()
        {
            if (_currentUser.IsAdmin)
                BuildAdminMenu();
            else
                BuildTallerMenu();
        }

        // MENÚ ADMINISTRADOR — acceso completo
        private void BuildAdminMenu()
        {
            MenuItems.Add(new NavItem { Title = "Inicio", ViewType = NavViewType.Home });

            // Órdenes
            var ordersMenu = new NavItem { Title = "Órdenes", ViewType = NavViewType.Orders };
            ordersMenu.Children.Add(new NavItem { Title = "Orden nueva", ViewType = NavViewType.NewOrders });
            ordersMenu.Children.Add(new NavItem { Title = "Revisión de órdenes", ViewType = NavViewType.OrdersReview });
            MenuItems.Add(ordersMenu);

            // Pedido material
            var pOrdersMenu = new NavItem { Title = "Pedido material", ViewType = NavViewType.POrders };
            pOrdersMenu.Children.Add(new NavItem { Title = "Pedido nuevo", ViewType = NavViewType.NewPOrders });
            pOrdersMenu.Children.Add(new NavItem { Title = "Pedidos pendientes", ViewType = NavViewType.POrdersReview });
            MenuItems.Add(pOrdersMenu);

            // Inventario
            var inventoryMenu = new NavItem { Title = "Inventario", ViewType = NavViewType.Inventorys };
            inventoryMenu.Children.Add(new NavItem { Title = "Materiales", ViewType = NavViewType.MaterialInventory });
            inventoryMenu.Children.Add(new NavItem { Title = "Productos", ViewType = NavViewType.ProductsInventory });
            MenuItems.Add(inventoryMenu);

            // Catálogos
            var addMenu = new NavItem { Title = "Catálogos", ViewType = NavViewType.Add };
            addMenu.Children.Add(new NavItem { Title = "Materiales", ViewType = NavViewType.Materials });
            addMenu.Children.Add(new NavItem { Title = "Productos", ViewType = NavViewType.Products });
            addMenu.Children.Add(new NavItem { Title = "Proveedores", ViewType = NavViewType.Suppliers });
            addMenu.Children.Add(new NavItem { Title = "Diseños", ViewType = NavViewType.Desing });
            MenuItems.Add(addMenu);

            // Producción
            var designsMenu = new NavItem { Title = "Producción", ViewType = NavViewType.Desings };
            designsMenu.Children.Add(new NavItem { Title = "Domos", ViewType = NavViewType.Domes });
            designsMenu.Children.Add(new NavItem { Title = "Ventanas", ViewType = NavViewType.Windows });
            designsMenu.Children.Add(new NavItem { Title = "Puertas", ViewType = NavViewType.Door });
            MenuItems.Add(designsMenu);

            // otros
            MenuItems.Add(new NavItem { Title = "Vidrios", ViewType = NavViewType.Glass });
            MenuItems.Add(new NavItem { Title = "Perfiles", ViewType = NavViewType.Profiles });
            MenuItems.Add(new NavItem { Title = "Desgloses", ViewType = NavViewType.Breakdown });

            // Solo admin
            MenuItems.Add(new NavItem { Title = "Actividad", ViewType = NavViewType.Activity });
            MenuItems.Add(new NavItem { Title = "Usuarios", ViewType = NavViewType.Users });
        }

        // MENÚ TALLER — acceso limitado
        // Solo: Revisión órdenes, Pedido material, Desgloses, Vidrios
        private void BuildTallerMenu()
        {
            MenuItems.Add(new NavItem { Title = "Inicio", ViewType = NavViewType.Home });

            // Revisión de órdenes (solo ver)
            var ordersMenu = new NavItem { Title = "Órdenes", ViewType = NavViewType.Orders };
            ordersMenu.Children.Add(new NavItem { Title = "Revisión de órdenes", ViewType = NavViewType.OrdersReview });
            MenuItems.Add(ordersMenu);

            // Pedido material (solo ver pendientes)
            var pOrdersMenu = new NavItem { Title = "Pedido material", ViewType = NavViewType.POrders };
            pOrdersMenu.Children.Add(new NavItem { Title = "Pedidos pendientes", ViewType = NavViewType.POrdersReview });
            MenuItems.Add(pOrdersMenu);

            // Módulos de taller
            MenuItems.Add(new NavItem { Title = "Desgloses", ViewType = NavViewType.Breakdown });
            MenuItems.Add(new NavItem { Title = "Vidrios", ViewType = NavViewType.Glass });
        }

        private void Navigate(object parameter)
        {
            if (parameter is NavViewType viewType)
            {
                CurrentView = viewType switch
                {
                    NavViewType.Home => new HomeViewModel(),

                    // Órdenes
                    NavViewType.NewOrders => new NewOrdersViewModel(),
                    NavViewType.OrdersReview => new OrdersReviewViewModel(),

                    // Pedidos proveedor
                    NavViewType.NewPOrders => new NewSupplierOrderViewModel(),
                    NavViewType.POrdersReview => new POrdersReviewViewModel(),

                    // Inventario
                    NavViewType.MaterialInventory => new MaterialInventoryViewModel(new MaterialRepository(), _currentUser.IsAdmin),
                    NavViewType.ProductsInventory => new ProductsInventoryViewModel(new ProductsRepository(), _currentUser.IsAdmin),

                    // Catálogos (admin)
                    NavViewType.Materials => new AddEditMaterialViewModel(
                        material: null,
                        onSave: RefreshInventory,
                        onCancel: null,
                        repository: new MaterialRepository()
                    ),
                    NavViewType.Products => new AddEditProductViewModel(
                        product: null,
                        onSave: RefreshInventory,
                        repository: new ProductsRepository()
                    ),

                    // Producción
                    NavViewType.Domes => CreateProductionViewModel("domos"),
                    NavViewType.Windows => CreateProductionViewModel("ventanas"),
                    NavViewType.Door => CreateProductionViewModel("puertas"),

                    // Módulos nuevos
                    NavViewType.Glass => new GlassViewModel(),
                    NavViewType.Profiles => new ProfileViewModel(),
                    NavViewType.Breakdown => new BreakdownViewModel(),

                    // Otros (admin)
                    NavViewType.Suppliers => new SupplierViewModel(),
                    NavViewType.Desing => new DesignViewModel(),
                    NavViewType.Activity => new ActivityViewModel(),
                    NavViewType.Users => new UsersViewModel(),

                    _ => new HomeViewModel()
                };
            }
        }

        // Crea ProductionViewModel con ChangeView conectado
        // para que Agregar/Editar funcione correctamente
        private ProductionViewModel CreateProductionViewModel(string type)
        {
            var vm = new ProductionViewModel(type);

            vm.ChangeView = (newVm) =>
            {
                CurrentView = newVm;
            };

            return vm;
        }
    }

    public class NavItem
    {
        public string Title { get; set; }
        public NavViewType ViewType { get; set; }
        public ObservableCollection<NavItem> Children { get; set; } = new ObservableCollection<NavItem>();
        public bool HasChildren => Children?.Count() > 0;
    }

    public enum NavViewType
    {
        Home,

        Orders,
        NewOrders,
        OrdersReview,

        POrders,
        NewPOrders,
        POrdersReview,

        Inventorys,
        MaterialInventory,
        ProductsInventory,

        Desings,
        Domes,
        Windows,
        Door,

        Add,
        Products,
        Materials,
        Suppliers,
        Desing,

        Activity,
        Users,

        Glass,
        Profiles,
        Breakdown,
    }
}