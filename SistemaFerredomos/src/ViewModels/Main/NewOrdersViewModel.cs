using System.Collections.Generic;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.Services;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SistemaFerredomos.src.ViewModels.Main
{
    class NewOrdersViewModel : BaseViewModel
    {
        private readonly OrdersRepository _ordersRepository;
        private readonly ProductsRepository _productsRepository;
        private readonly MaterialRepository _materialRepository;
        private readonly ProductionRepository _productionRepository;
        private readonly ProfileRepository _profileRepository;
        private readonly OrderProcessingService _orderService;
        private readonly UserModel _currentUser;

        // Colecciones de la orden 
        public ObservableCollection<OrderDetailsProductsModel> OrderProducts { get; set; } = new();
        public ObservableCollection<OrderDetailsProductionModel> OrderProductions { get; set; } = new();
        public ObservableCollection<RequiredMaterialModel> RequiredMaterials { get; set; } = new();
        public ObservableCollection<RequiredMaterialModel> MissingMaterials { get; set; } = new();

        // Perfiles seleccionados en la orden
        public ObservableCollection<OrderProfileItemModel> OrderProfiles { get; set; } = new();

        // Catálogos
        public ObservableCollection<ProductsModel> CatalogProducts { get; set; }
        public ObservableCollection<ProductsModel> FilteredCatalogProducts { get; set; }
        public ObservableCollection<ProfileModel> CatalogProfiles { get; set; }

        // Selección actual 
        public ProductsModel SelectedCatalogProduct { get; set; }

        private ProfileModel _selectedCatalogProfile;
        public ProfileModel SelectedCatalogProfile
        {
            get => _selectedCatalogProfile;
            set => SetProperty(ref _selectedCatalogProfile, value);
        }

        // Cantidad y color del perfil a agregar
        private decimal _profileQuantity = 1;
        public decimal ProfileQuantity
        {
            get => _profileQuantity;
            set => SetProperty(ref _profileQuantity, value);
        }

        private string _profileColor;
        public string ProfileColor
        {
            get => _profileColor;
            set => SetProperty(ref _profileColor, value);
        }

        // Datos del cliente
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        // Búsqueda
        private string _searchProductText;
        public string SearchProductText
        {
            get => _searchProductText;
            set { _searchProductText = value; OnPropertyChanged(); FilterCatalogProducts(); }
        }

        // Commands
        public ICommand SaveOrderCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveProductCommand { get; }
        public ICommand AddCatalogProductCommand { get; }
        public ICommand AddProductionCommand { get; }
        public ICommand RemoveProductionCommand { get; }
        public ICommand AddProfileCommand { get; }   
        public ICommand RemoveProfileCommand { get; }   

        public NewOrdersViewModel(UserModel currentUser)
        {
            _currentUser = currentUser;

            _ordersRepository = new OrdersRepository();
            _productsRepository = new ProductsRepository();
            _productionRepository = new ProductionRepository();
            _materialRepository = new MaterialRepository();
            _profileRepository = new ProfileRepository();  
            _orderService = new OrderProcessingService();

            // Cargar catálogos
            CatalogProducts = new ObservableCollection<ProductsModel>(_productsRepository.GetAll());
            FilteredCatalogProducts = new ObservableCollection<ProductsModel>(CatalogProducts);
            CatalogProfiles = new ObservableCollection<ProfileModel>(_profileRepository.GetAll()); 

            // Commands
            SaveOrderCommand = new RelayCommand(SaveOrder);
            AddProductCommand = new RelayCommand(AddProduct);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            AddCatalogProductCommand = new RelayCommand(AddCatalogProduct);
            AddProductionCommand = new RelayCommand(AddProduction);
            RemoveProductionCommand = new RelayCommand(RemoveProduction);

            //Commands nuevos de perfiles
            AddProfileCommand = new RelayCommand(AddProfile, o => SelectedCatalogProfile != null);
            RemoveProfileCommand = new RelayCommand(RemoveProfile);

            // Recalcular total al cambiar colecciones
            OrderProducts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            OrderProductions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            OrderProfiles.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
        }

        // Total 
        public decimal TotalPrice
        {
            get
            {
                decimal products = OrderProducts.Sum(p => p.Quantity * p.UnitPrice);
                decimal production = OrderProductions.Sum(p => p.Quantity * p.UnitPrice);
                return products + production;
            }
        }

        // Guardar orden
        private void SaveOrder(object obj)
        {
            try
            {
                if (!OrderProducts.Any() && !OrderProductions.Any() && !OrderProfiles.Any())
                {
                    MessageBox.Show("Agrega al menos un producto, producción o perfil.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CustomerName))
                {
                    MessageBox.Show("El nombre del cliente es obligatorio.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(CustomerPhone) && !ValidationHelper.IsValidPhone(CustomerPhone))
                {
                    MessageBox.Show("El teléfono ingresado no es válido.");
                    return;
                }

                CheckMissingMaterials();

                if (MissingMaterials.Any())
                {
                    var mensaje = "Faltan materiales:\n\n";
                    foreach (var m in MissingMaterials)
                        mensaje += $"{m.MaterialName} → faltan {m.Quantity}\n";
                    mensaje += "\n¿Deseas continuar y generar pedido automáticamente?";

                    var confirm = MessageBox.Show(mensaje, "Material insuficiente",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (confirm != MessageBoxResult.Yes) return;
                }

                var order = new OrdersModel
                {
                    UserId = _currentUser.Id,
                    Date = DateTime.Now,
                    CustomerName = CustomerName,
                    CustomerPhone = CustomerPhone,
                    Status = "pendiente",
                    TotalPrice = TotalPrice
                };

                var result = _orderService.ProcessOrder(
                    order,
                    OrderProductions.ToList(),
                    OrderProducts.ToList()
                );

                //Guardar perfiles del desglose si los hay
                if (OrderProfiles.Any())
                {
                    var breakdownRepo = new BreakdownRepository();
                    foreach (var p in OrderProfiles)
                    {
                        breakdownRepo.Add(new BreakdownModel
                        {
                            OrderNumber = result,
                            ProfileCode = p.Profile?.Code,
                            ProfileName = p.Profile?.Name,
                            Size = p.Profile?.Size ?? 0,
                            Color = p.Color,
                            Quantity = p.Quantity
                        });
                    }
                }

                MessageBox.Show(result);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la orden: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            OrderProducts.Clear();
            OrderProductions.Clear();
            OrderProfiles.Clear();
            RequiredMaterials.Clear();
            MissingMaterials.Clear();
            CustomerName = "";
            CustomerPhone = "";
            ProfileColor = "";
            ProfileQuantity = 1;
            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(CustomerPhone));
            OnPropertyChanged(nameof(TotalPrice));
        }

        // Perfiles
        private void AddProfile(object obj)
        {
            if (SelectedCatalogProfile == null) return;

            OrderProfiles.Add(new OrderProfileItemModel
            {
                Profile = SelectedCatalogProfile,
                Quantity = ProfileQuantity > 0 ? ProfileQuantity : 1,
                Color = ProfileColor?.Trim()
            });

            ProfileQuantity = 1;
            ProfileColor = "";
        }

        private void RemoveProfile(object obj)
        {
            if (obj is OrderProfileItemModel item)
                OrderProfiles.Remove(item);
        }

        // Productos
        private void AddProduct(object obj)
        {
            var view = new SelectProductView();
            var window = new Window
            {
                Title = "Seleccionar Producto",
                Content = view,
                Width = 600,
                Height = 400
            };
            window.ShowDialog();
        }

        private void AddCatalogProduct(object obj)
        {
            if (SelectedCatalogProduct == null) return;

            var existing = OrderProducts.FirstOrDefault(p => p.ProductId == SelectedCatalogProduct.Id);

            if (existing != null)
            {
                existing.Quantity++;
                OnPropertyChanged(nameof(TotalPrice));
                return;
            }

            OrderProducts.Add(new OrderDetailsProductsModel
            {
                ProductId = SelectedCatalogProduct.Id,
                Quantity = 1,
                UnitPrice = SelectedCatalogProduct.SalePrice,
                Products = SelectedCatalogProduct
            });

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void RemoveProduct(object obj)
        {
            if (obj is OrderDetailsProductsModel product)
            {
                OrderProducts.Remove(product);
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        // Producción 
        private void AddProduction(object obj)
        {
            var selector = new SelectProductionView();
            if (selector.ShowDialog() == true)
            {
                var production = selector.SelectedProduction;
                OrderProductions.Add(new OrderDetailsProductionModel
                {
                    ProductionId = production.Id,
                    Quantity = 1,
                    UnitPrice = production.Price,
                    Production = production
                });
                OnPropertyChanged(nameof(TotalPrice));
            }
            CalculateMaterials();
            CheckMissingMaterials();
        }

        private void RemoveProduction(object obj)
        {
            if (obj is OrderDetailsProductionModel item)
            {
                OrderProductions.Remove(item);
                OnPropertyChanged(nameof(TotalPrice));
                CalculateMaterials();
                CheckMissingMaterials();
            }
        }

        // Materiales 
        private void FilterCatalogProducts()
        {
            FilteredCatalogProducts = string.IsNullOrWhiteSpace(SearchProductText)
                ? new ObservableCollection<ProductsModel>(CatalogProducts)
                : new ObservableCollection<ProductsModel>(
                    CatalogProducts.Where(p => p.Name.ToLower().Contains(SearchProductText.ToLower())));
            OnPropertyChanged(nameof(FilteredCatalogProducts));
        }

        private void CalculateMaterials()
        {
            RequiredMaterials.Clear();
            foreach (var prod in OrderProductions)
            {
                var materials = _productionRepository.GetRequiredMaterials(prod.ProductionId, prod.Quantity);
                foreach (var m in materials)
                {
                    var existing = RequiredMaterials.FirstOrDefault(x => x.MaterialId == m.MaterialId);
                    if (existing != null) existing.Quantity += m.Quantity;
                    else RequiredMaterials.Add(m);
                }
            }
            OnPropertyChanged(nameof(RequiredMaterials));
        }

        private void CheckMissingMaterials()
        {
            MissingMaterials.Clear();
            foreach (var material in RequiredMaterials)
            {
                decimal stock = _materialRepository.GetStock(material.MaterialId);
                if (stock < material.Quantity)
                {
                    MissingMaterials.Add(new RequiredMaterialModel
                    {
                        MaterialId = material.MaterialId,
                        MaterialName = material.MaterialName,
                        Quantity = material.Quantity - stock
                    });
                }
            }
            OnPropertyChanged(nameof(MissingMaterials));
        }
    }
}