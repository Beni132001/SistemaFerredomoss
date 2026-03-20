using ClosedXML.Excel;
using Microsoft.Win32;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.ViewModels.Commons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    public class BreakdownViewModel : BaseViewModel
    {
        private readonly BreakdownRepository _repository;
        private readonly ProfileRepository _profileRepository;

        public ObservableCollection<BreakdownModel> BreakdownList { get; set; } = new ObservableCollection<BreakdownModel>();
        public ObservableCollection<ProfileModel> ProfileList { get; set; } = new ObservableCollection<ProfileModel>();

        // Ítem seleccionado en la tabla
        private BreakdownModel _selectedBreakdown;
        public BreakdownModel SelectedBreakdown
        {
            get => _selectedBreakdown;
            set => SetProperty(ref _selectedBreakdown, value);
        }

        // Filtro de búsqueda por número de orden
        private string _searchOrder;
        public string SearchOrder
        {
            get => _searchOrder;
            set
            {
                if (SetProperty(ref _searchOrder, value))
                    LoadBreakdowns();
            }
        }

        // Campos del formulario
        private string _orderNumber;
        public string OrderNumber
        {
            get => _orderNumber;
            set { if (SetProperty(ref _orderNumber, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private ProfileModel _selectedProfile;
        public ProfileModel SelectedProfile
        {
            get => _selectedProfile;
            set { if (SetProperty(ref _selectedProfile, value)) CommandManager.InvalidateRequerySuggested(); }
        }

        private double _size;
        public double Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string _color;
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private decimal _quantity;
        public decimal Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        // Visibilidad del formulario
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ExportCommand { get; }

        public BreakdownViewModel(BreakdownRepository repository = null, ProfileRepository profileRepository = null)
        {
            _repository = repository ?? new BreakdownRepository();
            _profileRepository = profileRepository ?? new ProfileRepository();

            LoadBreakdowns();
            LoadProfiles();

            AddCommand = new RelayCommand(o => OpenForm());
            DeleteCommand = new RelayCommand(o => DeleteBreakdown(), o => SelectedBreakdown != null);
            SaveCommand = new RelayCommand(o => SaveBreakdown(), o => CanSave());
            CancelCommand = new RelayCommand(o => CloseForm());
            ExportCommand = new RelayCommand(o => ExportToExcel(), o => BreakdownList.Any());
        }

        public void LoadBreakdowns()
        {
            BreakdownList.Clear();

            var list = string.IsNullOrWhiteSpace(SearchOrder)
                ? _repository.GetAll()
                : _repository.GetByOrderNumber(SearchOrder.Trim());

            foreach (var b in list)
                BreakdownList.Add(b);
        }

        private void LoadProfiles()
        {
            try
            {
                ProfileList.Clear();
                foreach (var p in _profileRepository.GetAll())
                    ProfileList.Add(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar perfiles: {ex.Message}");
            }
        }

        private void OpenForm()
        {
            ClearForm();
            IsFormVisible = true;
        }

        private void CloseForm()
        {
            IsFormVisible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            OrderNumber = string.Empty;
            SelectedProfile = null;
            Size = 0;
            Color = string.Empty;
            Quantity = 1;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(OrderNumber) &&
                   SelectedProfile != null &&
                   Quantity > 0 && 
                   (Size > 0 || SelectedProfile?.Size > 0);
        }

        private void SaveBreakdown()
        {
            var breakdown = new BreakdownModel
            {
                OrderNumber = OrderNumber.Trim(),
                ProfileCode = SelectedProfile?.Code,
                ProfileName = SelectedProfile?.Name,
                Size = Size > 0 ? Size : SelectedProfile?.Size ?? 0,
                Color = Color?.Trim(),
                Quantity = Quantity
            };

            if (_repository.Add(breakdown))
            {
                MessageBox.Show("✅ Desglose guardado correctamente");
                LoadBreakdowns();
                CloseForm();
            }
            else
            {
                MessageBox.Show("No se pudo guardar el desglose. Verifica los datos.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void DeleteBreakdown()
        {
            if (SelectedBreakdown == null) return;

            var result = MessageBox.Show(
                $"¿Deseas eliminar este desglose de la orden '{SelectedBreakdown.OrderNumber}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (_repository.Delete(SelectedBreakdown.Id))
                {
                    MessageBox.Show("✅ Desglose eliminado correctamente");
                    LoadBreakdowns();
                }
                else
                {
                    MessageBox.Show("❌ Error al eliminar desglose");
                }
            }
        }

        private void ExportToExcel()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Exportar Desgloses",
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = $"Desgloses_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
                };

                if (dialog.ShowDialog() != true) return;

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Desgloses");

                // Encabezados
                worksheet.Cell(1, 1).Value = "# Orden";
                worksheet.Cell(1, 2).Value = "Código Perfil";
                worksheet.Cell(1, 3).Value = "Nombre Perfil";
                worksheet.Cell(1, 4).Value = "Medida";
                worksheet.Cell(1, 5).Value = "Color";
                worksheet.Cell(1, 6).Value = "Cantidad";
                worksheet.Cell(1, 7).Value = "Fecha";

                // Estilo encabezados
                var headerRow = worksheet.Range("A1:G1");
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2c3e50");
                headerRow.Style.Font.FontColor = XLColor.White;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Datos
                int row = 2;
                foreach (var b in BreakdownList)
                {
                    worksheet.Cell(row, 1).Value = b.OrderNumber;
                    worksheet.Cell(row, 2).Value = b.ProfileCode;
                    worksheet.Cell(row, 3).Value = b.ProfileName;
                    worksheet.Cell(row, 4).Value = b.Size;
                    worksheet.Cell(row, 5).Value = b.Color;
                    worksheet.Cell(row, 6).Value = (double)b.Quantity;
                    worksheet.Cell(row, 7).Value = b.CreatedAt.ToString("dd/MM/yyyy HH:mm");

                    // Filas alternadas
                    if (row % 2 == 0)
                        worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f2f2f2");

                    row++;
                }

                // Ajustar ancho de columnas automáticamente
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(dialog.FileName);
                MessageBox.Show($"✅ Exportado correctamente:\n{dialog.FileName}", "Exportar Excel");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al exportar: {ex.Message}");
            }
        }
    }
}