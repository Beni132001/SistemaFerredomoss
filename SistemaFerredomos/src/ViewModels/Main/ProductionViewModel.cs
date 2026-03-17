using Mysqlx.Crud;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class ProductionViewModel : BaseViewModel
    {
        private readonly ProductionRepository _repository;
        private readonly MaterialRepository _materialRepository;

        public ObservableCollection<ProductionModel> Productions { get; set; }
        public ObservableCollection<ProductionMaterialModel> Materials { get; set; }

        public ObservableCollection<MaterialModel> AllMaterials { get; set; }

        public Action<BaseViewModel> ChangeView { get; set; }

        public string ProductionType { get; }
        private ProductionModel _selectedProduction;
        public ProductionModel SelectedProduction
        {
            get => _selectedProduction;
            set
            {
                _selectedProduction = value;
                OnPropertyChanged();

                LoadMaterials(null); // 🔥 esto es clave
            }
        }
        public MaterialModel SelectedMaterial { get; set; }


        public ICommand LoadMaterialsCommand { get; }
        public ICommand AddMaterialCommand { get; }
        public ICommand RemoveMaterialCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public decimal Quantity { get; set; }
        public ProductionViewModel(string type)
        {
            _repository = new ProductionRepository();
            _materialRepository = new MaterialRepository();

            ProductionType = type;

            AllMaterials = new ObservableCollection<MaterialModel>(_materialRepository.GetAll());

            LoadMaterialsCommand = new RelayCommand(LoadMaterials);
            AddMaterialCommand = new RelayCommand(AddMaterial);
            RemoveMaterialCommand = new RelayCommand(RemoveMaterial);
            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);

            LoadProductions();
        }

        private void LoadProductions()
        {
            var list = _repository
                .GetProductions()
                .Where(p => p.Type == ProductionType)
                .ToList();

            Productions = new ObservableCollection<ProductionModel>(list);

            OnPropertyChanged(nameof(Productions));
        }
        private void LoadMaterials(object obj)
        {
            if (SelectedProduction == null)
                return;

            var list = _repository.GetProductionMaterials(SelectedProduction.Id);

            Materials = new ObservableCollection<ProductionMaterialModel>(list);

            OnPropertyChanged(nameof(Materials));
        }

        //agregar material
        private void AddMaterial(object obj)
        {
            if (SelectedProduction == null || SelectedMaterial == null)
                return;

            _repository.AddMaterialToProduction(
                SelectedProduction.Id,
                SelectedMaterial.Id,
                Quantity
            );

            LoadMaterials(null);
        }

        //eliminar material
        private void RemoveMaterial(object obj)
        {
            if (obj is ProductionMaterialModel material)
            {
                _repository.DeleteMaterial(material.ProductionId, material.MaterialId);

                LoadMaterials(null);
            }
        }

        private void Add(object obj)
        {
            ChangeView?.Invoke(new AddEditProductionViewModel(
                null,
                LoadProductions,
                () => ChangeView?.Invoke(this)
            ));
        }

        private void Edit(object obj)
        {
            if (SelectedProduction == null) return;

            ChangeView?.Invoke(new AddEditProductionViewModel(
                SelectedProduction,
                LoadProductions,
                () => ChangeView?.Invoke(this)
            ));
        }

        private void Delete(object obj)
        {
            if (SelectedProduction == null) return;

            _repository.DeleteProduction(SelectedProduction.Id);

            LoadProductions();
        }
    }
}