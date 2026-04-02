using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

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

            _allOrders = _ordersRepository.GetOrders();
            ApplyFilters();

            CompleteOrderCommand = new RelayCommand(CompleteOrder);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            DeleteOrderCommand = new RelayCommand(DeleteOrder);
            ViewOrderCommand = new RelayCommand(ViewOrder);
        }

        private void CompleteOrder(object orderId)
        {
            var confirm = MessageBox.Show(
                "¿Marcar esta orden como completada?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            _ordersRepository.UpdateStatus((int)orderId, "completa");
            RefreshOrders();
        }

        private void CancelOrder(object orderId)
        {
            var confirm = MessageBox.Show(
                "¿Estás seguro de cancelar esta orden? Esta acción no se puede deshacer.",
                "Confirmar cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            _ordersRepository.UpdateStatus((int)orderId, "cancelada");
            RefreshOrders();
        }

        private void DeleteOrder(object orderId)
        {
            var confirm = MessageBox.Show(
                "¿Eliminar esta orden permanentemente?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            _ordersRepository.DeleteOrder((int)orderId);
            RefreshOrders();
        }

        private List<OrdersModel> _allOrders = new();

        // Filtro por estado
        private string _selectedStatus = "Todos";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); ApplyFilters(); }
        }

        // Filtro por fecha desde
        private DateTime? _dateFrom;
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(); ApplyFilters(); }
        }

        // Filtro por fecha hasta
        private DateTime? _dateTo;
        public DateTime? DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(); ApplyFilters(); }
        }

        // Opciones del ComboBox de estado
        public List<string> StatusOptions { get; } = new List<string>{
            "Todos", "pendiente", "completa", "cancelada"
        };

        private void ApplyFilters()
        {
            var filtered = _allOrders.AsEnumerable();

            if (SelectedStatus != "Todos")
                filtered = filtered.Where(o => o.Status == SelectedStatus);

            if (DateFrom.HasValue)
                filtered = filtered.Where(o => o.Date.Date >= DateFrom.Value.Date);

            if (DateTo.HasValue)
                filtered = filtered.Where(o => o.Date.Date <= DateTo.Value.Date);

            Orders.Clear();
            foreach (var order in filtered)
                Orders.Add(order);
        }

        private void RefreshOrders()
        {
            _allOrders = _ordersRepository.GetOrders();
            ApplyFilters();
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