using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.Services;
using SistemaFerredomos.src.ViewModels.Commons;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class DesingViewModel : BaseViewModel
    {
        private readonly DesignsRepository _repository;
        private readonly ImageService _imageService;
        private string _imageFileName;

        public ObservableCollection<DesignsModel> Designs { get; set; } = new();

        private DesignsModel _selectedDesign;
        public DesignsModel SelectedDesign
        {
            get => _selectedDesign;
            set
            {
                if (SetProperty(ref _selectedDesign, value))
                    LoadPreview(value);
            }
        }

        // Campos del formulario
        private string _name;
        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _color;
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        // Imagen
        private BitmapImage _designImage;
        public BitmapImage DesignImage
        {
            get => _designImage;
            set => SetProperty(ref _designImage, value);
        }

        // Vista previa del diseño seleccionado
        private BitmapImage _previewImage;
        public BitmapImage PreviewImage
        {
            get => _previewImage;
            set => SetProperty(ref _previewImage, value);
        }

        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        private bool _isEditing;
        public string FormTitle => _isEditing ? "EDITAR DISEÑO" : "AGREGAR DISEÑO";

        // Commands
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public DesingViewModel(DesignsRepository repository = null)
        {
            _repository = repository ?? new DesignsRepository();
            _imageService = new ImageService();

            LoadDesigns();

            AddCommand = new RelayCommand(o => OpenForm(null));
            EditCommand = new RelayCommand(o => OpenForm(SelectedDesign), o => SelectedDesign != null);
            DeleteCommand = new RelayCommand(o => DeleteDesign(), o => SelectedDesign != null);
            SaveCommand = new RelayCommand(o => SaveDesign(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
            SelectImageCommand = new RelayCommand(o => SelectImage());
            RemoveImageCommand = new RelayCommand(o => RemoveImage());
        }

        private void LoadDesigns()
        {
            Designs.Clear();
            foreach (var d in _repository.GetAll())
                Designs.Add(d);
        }

        private void LoadPreview(DesignsModel design)
        {
            if (design == null)
            {
                PreviewImage = null;
                return;
            }
            PreviewImage = string.IsNullOrEmpty(design.Image)
                ? _imageService.GetDefaultImage()
                : _imageService.LoadImage(design.Image);
        }

        private void OpenForm(DesignsModel design)
        {
            _isEditing = design != null;

            if (design != null)
            {
                Name = design.Name;
                Description = design.Description;
                Color = design.Color;
                _imageFileName = design.Image;
                DesignImage = string.IsNullOrEmpty(design.Image)
                    ? _imageService.GetDefaultImage()
                    : _imageService.LoadImage(design.Image);
            }
            else
            {
                ClearForm();
                DesignImage = _imageService.GetDefaultImage();
            }

            IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void CloseForm()
        {
            IsFormVisible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
            Color = string.Empty;
            _imageFileName = null;
            DesignImage = null;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        private void SelectImage()
        {
            try
            {
                var newFile = _imageService.SelectAndSaveImage();
                if (!string.IsNullOrEmpty(newFile))
                {
                    if (!string.IsNullOrEmpty(_imageFileName))
                        _imageService.DeleteImage(_imageFileName);

                    _imageFileName = newFile;
                    DesignImage = _imageService.LoadImage(_imageFileName);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al seleccionar imagen: {ex.Message}");
            }
        }

        private void RemoveImage()
        {
            if (!string.IsNullOrEmpty(_imageFileName))
                _imageService.DeleteImage(_imageFileName);

            _imageFileName = null;
            DesignImage = _imageService.GetDefaultImage();
        }

        private void SaveDesign()
        {
            var design = new DesignsModel
            {
                Id = _isEditing ? SelectedDesign.Id : 0,
                Name = Name.Trim(),
                Description = Description?.Trim(),
                Color = Color?.Trim(),
                Image = _imageFileName
            };

            bool success = _isEditing
                ? _repository.Update(design)
                : _repository.Add(design);

            if (success)
            {
                MessageBox.Show("✅ Diseño guardado correctamente");
                LoadDesigns();
                CloseForm();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar diseño");
            }
        }

        private void DeleteDesign()
        {
            if (SelectedDesign == null) return;

            var result = MessageBox.Show(
                $"¿Eliminar el diseño '{SelectedDesign.Name}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedDesign.Id))
                {
                    if (!string.IsNullOrEmpty(SelectedDesign.Image))
                        _imageService.DeleteImage(SelectedDesign.Image);

                    MessageBox.Show("✅ Diseño eliminado correctamente");
                    PreviewImage = null;
                    LoadDesigns();
                }
                else
                {
                    MessageBox.Show("❌ No se puede eliminar. El diseño está en uso en producción.");
                }
            }
        }
    }
}