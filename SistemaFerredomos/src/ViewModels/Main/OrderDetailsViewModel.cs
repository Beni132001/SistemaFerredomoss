using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class OrderDetailsViewModel : BaseViewModel
    {
        private readonly OrdersRepository _ordersRepository;

        public ObservableCollection<OrderDetailsProductsModel> Products { get; set; }
        public ObservableCollection<OrderDetailsProductionModel> Productions { get; set; }

        public int OrderId { get; }

        public OrderDetailsViewModel(int orderId)
        {
            _ordersRepository = new OrdersRepository();

            OrderId = orderId;

            Products = new ObservableCollection<OrderDetailsProductsModel>(
                _ordersRepository.GetOrderProducts(orderId)
            );

            Productions = new ObservableCollection<OrderDetailsProductionModel>(
            _ordersRepository.GetOrderProductions(orderId)
            );
        }
    }
}