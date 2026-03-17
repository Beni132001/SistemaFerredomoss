using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class SelectProductionViewModel : BaseViewModel
    {
        private readonly ProductionRepository _productionRepository;

        public ObservableCollection<ProductionModel> Productions { get; set; }
        public ObservableCollection<ProductionModel> FilteredProductions { get; set; }

        public ProductionModel SelectedProduction { get; set; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterProductions();
            }
        }

        public SelectProductionViewModel()
        {
            _productionRepository = new ProductionRepository();

            var list = _productionRepository.GetProductions();

            Productions = new ObservableCollection<ProductionModel>(list);
            FilteredProductions = new ObservableCollection<ProductionModel>(list);
        }

        private void FilterProductions()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredProductions = new ObservableCollection<ProductionModel>(Productions);
            }
            else
            {
                var filtered = Productions
                    .Where(p => p.Name.ToLower().Contains(SearchText.ToLower()) ||
                                p.Type.ToLower().Contains(SearchText.ToLower()))
                    .ToList();

                FilteredProductions = new ObservableCollection<ProductionModel>(filtered);
            }

            OnPropertyChanged(nameof(FilteredProductions));
        }
    }
}