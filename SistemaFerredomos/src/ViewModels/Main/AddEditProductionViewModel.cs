using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class AddEditProductionViewModel : BaseViewModel
    {
        private readonly ProductionRepository _repository;

        public ProductionModel Production { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action OnSave { get; set; }
        public Action OnCancel { get; set; }

        public AddEditProductionViewModel(
            ProductionModel production,
            Action onSave,
            Action onCancel)
        {
            _repository = new ProductionRepository();

            Production = production ?? new ProductionModel();

            OnSave = onSave;
            OnCancel = onCancel;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object obj)
        {
            if (string.IsNullOrWhiteSpace(Production.Name))
            {
                MessageBox.Show("El nombre es obligatorio");
                return;
            }

            if (Production.Id == 0)
                _repository.AddProduction(Production);
            else
                _repository.UpdateProduction(Production);

            OnSave?.Invoke();
        }

        private void Cancel(object obj)
        {
            OnCancel?.Invoke();
        }
    }
}