using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Main;
using SistemaFerredomos.src.Services;
using SistemaFerredomos.src.ViewModels.Commons;
using SistemaFerredomos.src.Views.Main;
using SistemaFerredomos.src.Services;
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

        private readonly OrderProcessingService _orderService;

        public ObservableCollection<OrderDetailsProductsModel> OrderProducts { get; set; }
            = new ObservableCollection<OrderDetailsProductsModel>();      

        public ObservableCollection<OrderDetailsProductionModel> OrderProductions { get; set; }
            = new ObservableCollection<OrderDetailsProductionModel>();

        public ObservableCollection<ProductsModel> CatalogProducts { get; set; }
        public ObservableCollection<ProductsModel> FilteredCatalogProducts { get; set; }

        public ObservableCollection<RequiredMaterialModel> RequiredMaterials { get; set; }
    = new ObservableCollection<RequiredMaterialModel>();

        public ObservableCollection<RequiredMaterialModel> MissingMaterials { get; set; }
    = new ObservableCollection<RequiredMaterialModel>();

        public ProductsModel SelectedCatalogProduct { get; set; }

        private string _searchProductText;
        public string SearchProductText
        {
            get => _searchProductText;
            set
            {
                _searchProductText = value;
                OnPropertyChanged();
                FilterCatalogProducts();
            }
        }
        public ICommand AddProductionCommand { get; }
        public ICommand RemoveProductionCommand { get; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public ICommand SaveOrderCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveProductCommand { get; }

        public NewOrdersViewModel()
        {
            _ordersRepository = new OrdersRepository();
            _productsRepository = new ProductsRepository();
            _productionRepository = new ProductionRepository();
            _materialRepository = new MaterialRepository();

            _orderService = new OrderProcessingService();

            SaveOrderCommand = new RelayCommand(SaveOrder);
            AddProductCommand = new RelayCommand(AddProduct);

            AddProductionCommand = new RelayCommand(AddProduction);
            RemoveProductionCommand = new RelayCommand(RemoveProduction);

            RemoveProductCommand = new RelayCommand(RemoveProduct);

            OrderProducts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            OrderProductions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));

            AddCatalogProductCommand = new RelayCommand(AddCatalogProduct);

            CatalogProducts = new ObservableCollection<ProductsModel>(_productsRepository.GetAll());
            FilteredCatalogProducts = new ObservableCollection<ProductsModel>(CatalogProducts);

        }

        private void SaveOrder(object obj)
        {
            try
            {
                if (!OrderProducts.Any() && !OrderProductions.Any())
                {
                    MessageBox.Show("Agrega al menos un producto o producción.");
                    return;
                }

                // 🔥 detectar faltantes antes
                CheckMissingMaterials();

                if (MissingMaterials.Any())
                {
                    var mensaje = "Faltan materiales:\n\n";

                    foreach (var m in MissingMaterials)
                    {
                        mensaje += $"{m.MaterialName} → faltan {m.Quantity}\n";
                    }

                    mensaje += "\n¿Deseas continuar y generar pedido automáticamente?";

                    var resultConfirm = MessageBox.Show(
                        mensaje,
                        "Material insuficiente",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (resultConfirm != MessageBoxResult.Yes)
                        return;
                }

                OrdersModel order = new OrdersModel
                {
                    UserId = 1,
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

                MessageBox.Show(result);

                // 🔥 limpiar
                OrderProducts.Clear();
                OrderProductions.Clear();
                RequiredMaterials.Clear();
                MissingMaterials.Clear();

                CustomerName = "";
                CustomerPhone = "";

                OnPropertyChanged(nameof(CustomerName));
                OnPropertyChanged(nameof(CustomerPhone));
                OnPropertyChanged(nameof(TotalPrice));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la orden: " + ex.Message);
            }
        }

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

        public decimal TotalPrice
        {
            get
            {
                decimal productsTotal = OrderProducts.Sum(p => p.Quantity * p.UnitPrice);
                decimal productionTotal = OrderProductions.Sum(p => p.Quantity * p.UnitPrice);

                return productsTotal + productionTotal;
            }
        }

        private void RemoveProduct(object obj)
        {
            if (obj is OrderDetailsProductsModel product)
            {
                OrderProducts.Remove(product);
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

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

        //filtrar procutos
        private void FilterCatalogProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchProductText))
            {
                FilteredCatalogProducts = new ObservableCollection<ProductsModel>(CatalogProducts);
            }
            else
            {
                var filtered = CatalogProducts
                    .Where(p => p.Name.ToLower().Contains(SearchProductText.ToLower()))
                    .ToList();

                FilteredCatalogProducts = new ObservableCollection<ProductsModel>(filtered);
            }

            OnPropertyChanged(nameof(FilteredCatalogProducts));
        }
        //agregar producto rapido
        public ICommand AddCatalogProductCommand { get; }

        private void AddCatalogProduct(object obj)
        {
            if (SelectedCatalogProduct == null)
                return;

            OrderProducts.Add(new OrderDetailsProductsModel
            {
                ProductId = SelectedCatalogProduct.Id,
                Quantity = 1,
                UnitPrice = SelectedCatalogProduct.SalePrice,
                Products = SelectedCatalogProduct
            });

            OnPropertyChanged(nameof(TotalPrice));
        }

        //calcular materiales
        private void CalculateMaterials()
        {
            RequiredMaterials.Clear();

            foreach (var prod in OrderProductions)
            {
                var materials = _productionRepository.GetRequiredMaterials(prod.ProductionId, prod.Quantity);

                foreach (var m in materials)
                {
                    var existing = RequiredMaterials
                        .FirstOrDefault(x => x.MaterialId == m.MaterialId); // 🔥 CAMBIO CLAVE

                    if (existing != null)
                    {
                        existing.Quantity += m.Quantity;
                    }
                    else
                    {
                        RequiredMaterials.Add(m);
                    }
                }
            }

            OnPropertyChanged(nameof(RequiredMaterials));
        }

        //metodo paraa detectar faltantes
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