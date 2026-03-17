using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class POrdersReviewViewModel : BaseViewModel
    {
        private readonly SupplierOrdersRepository _repository;
        private readonly MaterialRepository _materialRepository;
        private readonly ProductsRepository _productsRepository;

        public ObservableCollection<SupplierOrderModel> Orders { get; set; }

        public ICommand ReceiveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public POrdersReviewViewModel()
        {
            _repository = new SupplierOrdersRepository();
            _materialRepository = new MaterialRepository();
            _productsRepository = new ProductsRepository();

            Orders = new ObservableCollection<SupplierOrderModel>(_repository.GetOrders());

            ReceiveOrderCommand = new RelayCommand(ReceiveOrder);
            DeleteOrderCommand = new RelayCommand(DeleteOrder);
        }

        private void ReceiveOrder(object orderId)
        {
            int id = (int)orderId;

            var materials = _repository.GetOrderMaterials(id);
            var products = _repository.GetOrderProducts(id);

            foreach (var material in materials)
            {
                _materialRepository.IncreaseStock(material.MaterialId, material.Quantity);
            }

            foreach (var product in products)
            {
                _productsRepository.IncreaseStock(product.ProductId, product.Quantity);
            }

            _repository.DeleteOrder(id);

            RefreshOrders();
        }

        private void DeleteOrder(object orderId)
        {
            int id = (int)orderId;

            _repository.DeleteOrder(id);

            RefreshOrders();
        }

        private void RefreshOrders()
        {
            Orders.Clear();

            foreach (var order in _repository.GetOrders())
                Orders.Add(order);
        }
    }
}