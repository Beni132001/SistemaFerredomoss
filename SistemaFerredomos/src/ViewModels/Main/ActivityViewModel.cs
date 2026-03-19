using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ActivityViewModel : BaseViewModel
    {
        private readonly ActivityRepository _repository;

        public ObservableCollection<ActivityModel> Activities { get; set; } = new();

        // Filtro por usuario
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) LoadActivities(); }
        }

        public ICommand RefreshCommand { get; }

        public ActivityViewModel(ActivityRepository repository = null)
        {
            _repository = repository ?? new ActivityRepository();
            RefreshCommand = new RelayCommand(o => LoadActivities());
            LoadActivities();
        }

        public void LoadActivities()
        {
            Activities.Clear();
            var list = string.IsNullOrWhiteSpace(SearchText)
                ? _repository.GetAll()
                : _repository.GetByUser(SearchText.Trim());

            foreach (var a in list)
                Activities.Add(a);
        }
    }
}