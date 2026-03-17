using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class MainViewModel : BaseViewModel
    {
        private readonly UserModel _currentUser;
        private BaseViewModel _currentView;
        private readonly MaterialRepository _materialRepository;
        private readonly ProductsRepository _productsRepository;

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

        private void RefreshInventory()
        {
            if (CurrentView is MaterialInventoryViewModel materialVm)
            {
                materialVm.LoadMaterials(); // Método que recarga la lista en MaterialInventoryViewModel
            }
            else if (CurrentView is ProductsInventoryViewModel productVm)
            {
                productVm.LoadProducts(); // Similar, recarga productos
            }
        }

        public MainViewModel(UserModel user)
        {
            _currentUser = user;
            NavigateCommand = new RelayCommand(Navigate);

            // Inicializar repositorios
            _materialRepository = new MaterialRepository();

            // Construir menú según tipo de usuario
            BuildMenu();

            // Vista inicial
            CurrentView = new HomeViewModel();

            LogoutCommand = new RelayCommand(Logout);   
        }

        private void BuildMenu()
        {
            MenuItems.Add(new NavItem { Title = "Inicio", ViewType = NavViewType.Home });

            var OrdersMenu = new NavItem { Title ="Ordenes ", ViewType = NavViewType.Orders };
            OrdersMenu.Children.Add(new NavItem { Title = "Orden nueva", ViewType=NavViewType.NewOrders });
            OrdersMenu.Children.Add(new NavItem { Title = "Revision de ordenes", ViewType = NavViewType.OrdersReview });

            var POrdersMenu=new NavItem { Title = "Pedidos", ViewType= NavViewType.POrders };
            POrdersMenu.Children.Add(new NavItem { Title = "Pedido nuevo", ViewType = NavViewType.NewPOrders });
            POrdersMenu.Children.Add(new NavItem { Title = "Revision de pedido", ViewType = NavViewType.POrdersReview });

            var InventorysMenu = new NavItem { Title = "Inventario", ViewType = NavViewType.Inventorys };
            InventorysMenu.Children.Add(new NavItem { Title = "Inventario de materiales", ViewType = NavViewType.MaterialInventory });
            InventorysMenu.Children.Add(new NavItem { Title = "Inventario de productos", ViewType = NavViewType.ProductsInventory });

            var DesingsMenu = new NavItem { Title = "Diseños", ViewType = NavViewType.Desings };
            DesingsMenu.Children.Add(new NavItem { Title = "Vidrios", ViewType = NavViewType.Glass });
            DesingsMenu.Children.Add(new NavItem { Title = "Domos", ViewType = NavViewType.Domes });
            DesingsMenu.Children.Add(new NavItem { Title = "Ventanas", ViewType = NavViewType.Windows });
            DesingsMenu.Children.Add(new NavItem { Title = "Puertas", ViewType = NavViewType.Door });

            MenuItems.Add(OrdersMenu);
            MenuItems.Add(POrdersMenu);
            MenuItems.Add(InventorysMenu);
            MenuItems.Add(DesingsMenu);

            //solo administrador
            if (_currentUser.IsAdmin)
            {
                var AddMenu = new NavItem { Title = "Agregar", ViewType = NavViewType.Add };
                AddMenu.Children.Add(new NavItem { Title = "Productos", ViewType = NavViewType.Products });
                AddMenu.Children.Add(new NavItem { Title = "Materiales", ViewType = NavViewType.Materials });
                AddMenu.Children.Add(new NavItem { Title = "Proveedores", ViewType = NavViewType.Suppliers });
                AddMenu.Children.Add(new NavItem { Title = "Diseños", ViewType = NavViewType.Desing });

                MenuItems.Add(AddMenu); // <--- Agregar al menú principal

                MenuItems.Add(new NavItem { Title = "Actividad", ViewType = NavViewType.Activity });
                MenuItems.Add(new NavItem { Title = "Opciones", ViewType = NavViewType.Users });
            }
            

        }

        private void Navigate(object parameter)
        {
            if (parameter is NavViewType viewType)
            {
                CurrentView = viewType switch
                {
                    NavViewType.Home => new HomeViewModel(),

                    //Ordenes
                    NavViewType.NewOrders => new NewOrdersViewModel(),
                    NavViewType.OrdersReview => new OrdersReviewViewModel(),
                    //pedido proveedor
                    NavViewType.NewPOrders => new NewSupplierOrderViewModel(),
                    NavViewType.POrdersReview => new POrdersReviewViewModel(),
                    //Inventario
                    NavViewType.MaterialInventory => new MaterialInventoryViewModel(new MaterialRepository(), _currentUser.IsAdmin),
                    NavViewType.ProductsInventory => new ProductsInventoryViewModel(new ProductsRepository(), _currentUser.IsAdmin),

                    // agregar
                    NavViewType.Materials => new AddEditMaterialViewModel(
                        material: null,
                        onSave: RefreshInventory,
                        onCancel: null,
                        repository: new MaterialRepository()
                    ),

                    //Agregar null para onCancel  
                    NavViewType.Products => new AddEditProductViewModel(
                        product: null,
                        onSave: RefreshInventory,
                        repository: new ProductsRepository()
                    ),

                    //diseños
                    NavViewType.Glass => new ProductionViewModel("vidrios"),
                    NavViewType.Domes => new ProductionViewModel("domos"),
                    NavViewType.Windows => new ProductionViewModel("ventanas"),
                    NavViewType.Door => new ProductionViewModel("puertas"),
                    //otros
                    NavViewType.Suppliers => new SupplierViewModel(),
                    NavViewType.Desing => new DesingViewModel(),
                    NavViewType.Activity => new ActivityViewModel(),
                    NavViewType.Users => new UsersViewModel(),

                    _ => new HomeViewModel()
                };
            }
        }
    }

    public class NavItem
    {
        public string Title { get; set; }
        public NavViewType ViewType { get; set; }
        public ObservableCollection<NavItem> Children { get; set; }= new ObservableCollection<NavItem>();
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
       Glass,
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
      
    }

}
