using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class HomeViewModel : BaseViewModel
    {
        public string WelcomeMessage => "Bienvenido al sistema Ferredomos";

        private readonly OrdersRepository _ordersRepository;
        private readonly MaterialRepository _materialRepository;

        public ObservableCollection<MaterialModel> LowStockMaterials { get; set; }

        public DashboardStatsModel Stats { get; set; }

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
        }
    }
}
