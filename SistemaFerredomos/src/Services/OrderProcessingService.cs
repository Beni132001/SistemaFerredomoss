using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories;
using SistemaFerredomos.src.Repositories.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaFerredomos.src.Services
{
    public class OrderProcessingService
    {
        private readonly ProductionRepository _productionRepo = new();
        private readonly MaterialRepository _materialsRepo = new();
        private readonly SupplierOrdersRepository _supplierOrderRepo = new();
        private readonly OrdersRepository _ordersRepo = new();
        private readonly ProductsRepository _productsRepo = new();

        public string ProcessOrder(
            OrdersModel order,
            List<OrderDetailsProductionModel> productions,
            List<OrderDetailsProductsModel> products)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            productions ??= new List<OrderDetailsProductionModel>();
            products ??= new List<OrderDetailsProductsModel>();

            var faltantes = CalculateMissingMaterials(productions);

            if (faltantes.Any())
                CreateSupplierOrders(order.UserId, faltantes);

            int orderId = SaveOrder(order, productions, products);

            UpdateStock(products, productions);

            return faltantes.Any()
                ? "Orden creada y se generó pedido de material automáticamente"
                : "Orden creada correctamente";
        }

        // CALCULAR FALTANTES
        private Dictionary<int, decimal> CalculateMissingMaterials(List<OrderDetailsProductionModel> productions)
        {
            var faltantes = new Dictionary<int, decimal>();

            foreach (var prod in productions)
            {
                var materials = _productionRepo.GetRequiredMaterials(prod.ProductionId, prod.Quantity);

                foreach (var mat in materials)
                {
                    var stock = _materialsRepo.GetStock(mat.MaterialId);

                    if (stock < mat.Quantity)
                    {
                        var falta = mat.Quantity - stock;

                        if (faltantes.ContainsKey(mat.MaterialId))
                            faltantes[mat.MaterialId] += falta;
                        else
                            faltantes.Add(mat.MaterialId, falta);
                    }
                }
            }

            return faltantes;
        }

        // CREAR PEDIDOS A PROVEEDOR 
        private void CreateSupplierOrders(int userId, Dictionary<int, decimal> faltantes)
        {
            var porProveedor = new Dictionary<int, List<SupplierOrderMaterialModel>>();

            foreach (var f in faltantes)
            {
                var material = _materialsRepo.GetById(f.Key);
                if (material == null || material.SupplierId == 0) continue;

                if (!porProveedor.ContainsKey(material.SupplierId))
                    porProveedor[material.SupplierId] = new List<SupplierOrderMaterialModel>();

                porProveedor[material.SupplierId].Add(new SupplierOrderMaterialModel
                {
                    MaterialId = f.Key,
                    Quantity = f.Value,
                    UnitPrice = _materialsRepo.GetPurchasePrice(f.Key)
                });
            }

            // SOLO se crean pedidos por proveedor 
            foreach (var kvp in porProveedor)
            {
                var supplierOrder = new SupplierOrderModel
                {
                    UserId = userId,
                    SupplierId = kvp.Key,
                    Date = DateTime.Now,
                    TotalPrice = kvp.Value.Sum(m => m.Quantity * m.UnitPrice)
                };

                int supplierOrderId = _supplierOrderRepo.CreateSupplierOrder(supplierOrder);
                _supplierOrderRepo.SaveOrderMaterials(supplierOrderId, kvp.Value);
            }
        }

        // GUARDAR ORDEN
        private int SaveOrder(
            OrdersModel order,
            List<OrderDetailsProductionModel> productions,
            List<OrderDetailsProductsModel> products)
        {
            int orderId = _ordersRepo.CreateOrder(order);

            _ordersRepo.SaveOrderProductions(orderId, productions);
            _ordersRepo.SaveOrderProducts(orderId, products);

            return orderId;
        }

        // ACTUALIZAR STOCK
        private void UpdateStock(
            List<OrderDetailsProductsModel> products,
            List<OrderDetailsProductionModel> productions)
        {
            foreach (var p in products)
                _productsRepo.UpdateStock(p.ProductId, p.Quantity);

            foreach (var prod in productions)
            {
                var materials = _productionRepo.GetRequiredMaterials(prod.ProductionId, prod.Quantity);

                foreach (var mat in materials)
                    _materialsRepo.UpdateStock(mat.MaterialId, mat.Quantity);
            }
        }
    }
}