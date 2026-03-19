using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly OrdersRepository _ordersRepository;
        private readonly MaterialRepository _materialRepository;

        public ObservableCollection<MaterialModel> LowStockMaterials { get; set; }
        public DashboardStatsModel Stats { get; set; }

        // Fecha actual formateada
        public string TodayDate => DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy",
            new System.Globalization.CultureInfo("es-MX"));

        // True si hay materiales con stock bajo
        public bool HasLowStock => Stats?.LowMaterials > 0;

        public HomeViewModel()
        {
            _ordersRepository = new OrdersRepository();
            _materialRepository = new MaterialRepository();
            LoadStats();
        }

        private void LoadStats()
        {
            Stats = _ordersRepository.GetDashboardStats() ?? new DashboardStatsModel();
            Stats.LowMaterials = _materialRepository.GetLowStockCount();

            LowStockMaterials = new ObservableCollection<MaterialModel>(
                _materialRepository.GetLowStockMaterials()
            );

            OnPropertyChanged(nameof(Stats));
            OnPropertyChanged(nameof(LowStockMaterials));
            OnPropertyChanged(nameof(HasLowStock));
        }
    }
}