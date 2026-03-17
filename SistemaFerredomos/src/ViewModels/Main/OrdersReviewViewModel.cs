using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class OrdersReviewViewModel : BaseViewModel
    {
        private readonly OrdersRepository _ordersRepository;

        public ObservableCollection<OrdersModel> Orders { get; set; }

        public ICommand CompleteOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand ViewOrderCommand { get; }

        public OrdersReviewViewModel()
        {
            _ordersRepository = new OrdersRepository();

            Orders = new ObservableCollection<OrdersModel>(_ordersRepository.GetOrders());

            CompleteOrderCommand = new RelayCommand(CompleteOrder);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            DeleteOrderCommand = new RelayCommand(DeleteOrder);
            ViewOrderCommand = new RelayCommand(ViewOrder);
        }

        private void CompleteOrder(object orderId)
        {
            _ordersRepository.UpdateStatus((int)orderId, "completa");
            RefreshOrders();
        }

        private void CancelOrder(object orderId)
        {
            _ordersRepository.UpdateStatus((int)orderId, "cancelada");
            RefreshOrders();
        }

        private void DeleteOrder(object orderId)
        {
            _ordersRepository.DeleteOrder((int)orderId);
            RefreshOrders();
        }


        private void RefreshOrders()
        {
            Orders.Clear();

            foreach (var order in _ordersRepository.GetOrders())
                Orders.Add(order);
        }

        private void ViewOrder(object orderId)
        {
            int id = (int)orderId;

            var detailsView = new OrderDetailsView
            {
                DataContext = new OrderDetailsViewModel(id)
            };

            Window window = new Window
            {
                Title = "Detalle de Orden",
                Content = detailsView,
                Width = 600,
                Height = 400
            };

            window.ShowDialog();
        }
    }
}