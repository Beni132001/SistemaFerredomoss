using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class NewSupplierOrderViewModel : BaseViewModel
    {

        private readonly SupplierOrdersRepository _repository;
        private readonly MaterialRepository _materialRepository;
        private readonly ProductsRepository _productsRepository;

        // Catálogos
        public ObservableCollection<MaterialModel> MaterialsCatalog { get; set; }
        public ObservableCollection<ProductsModel> ProductsCatalog { get; set; }

        // Pedido actual
        public ObservableCollection<SupplierOrderMaterialModel> OrderMaterials { get; set; }
        public ObservableCollection<SupplierOrderProductsModel> OrderProducts { get; set; }

        public ObservableCollection<SupplierModel> Suppliers { get; set; }

        public MaterialModel SelectedMaterial { get; set; }
        public ProductsModel SelectedProduct { get; set; }
        public SupplierModel SelectedSupplier { get; set; }


        public ICommand AddMaterialCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand SaveOrderCommand { get; }

        public decimal TotalPrice => OrderMaterials.Sum(x => x.Subtotal) + OrderProducts.Sum(x => x.Subtotal);

        public NewSupplierOrderViewModel()
        {
            _repository = new SupplierOrdersRepository();
            _materialRepository = new MaterialRepository();
            _productsRepository = new ProductsRepository();

            MaterialsCatalog = new ObservableCollection<MaterialModel>(_materialRepository.GetAll());
            ProductsCatalog = new ObservableCollection<ProductsModel>(_productsRepository.GetAll());

            Suppliers = new ObservableCollection<SupplierModel>(_productsRepository.GetSuppliers());


            OrderMaterials = new ObservableCollection<SupplierOrderMaterialModel>();
            OrderProducts = new ObservableCollection<SupplierOrderProductsModel>();

            AddMaterialCommand = new RelayCommand(AddMaterial);
            AddProductCommand = new RelayCommand(AddProduct);
            SaveOrderCommand = new RelayCommand(SaveOrder);
        }

        private void AddMaterial(object obj)
        {
            if (SelectedMaterial == null)
                return;

            OrderMaterials.Add(new SupplierOrderMaterialModel
            {
                MaterialId = SelectedMaterial.Id,
                Material = SelectedMaterial,
                Quantity = 1,
                UnitPrice = SelectedMaterial.PurchasePrice
            });

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void AddProduct(object obj)
        {
            if (SelectedProduct == null)
                return;

            OrderProducts.Add(new SupplierOrderProductsModel
            {
                ProductId = SelectedProduct.Id,
                Products = SelectedProduct,
                Quantity = 1,
                UnitPrice = SelectedProduct.PurchasePrice
            });

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void SaveOrder(object obj)
        {
            if (SelectedSupplier == null)
            {
                System.Windows.MessageBox.Show("Seleccione un proveedor");
                return;
            }

            if (OrderMaterials.Count == 0 && OrderProducts.Count == 0)
            {
                System.Windows.MessageBox.Show("El pedido está vacío");
                return;
            }

            SupplierOrderModel order = new SupplierOrderModel
            {
                UserId = 1,
                SupplierId = SelectedSupplier.Id,
                TotalPrice = TotalPrice
            };

            int orderId = _repository.CreateSupplierOrder(order);

            _repository.SaveOrderMaterials(orderId, OrderMaterials.ToList());
            _repository.SaveOrderProducts(orderId, OrderProducts.ToList());

            System.Windows.MessageBox.Show("Pedido guardado correctamente");

            OrderMaterials.Clear();
            OrderProducts.Clear();

            OnPropertyChanged(nameof(TotalPrice));
        }
    }
}