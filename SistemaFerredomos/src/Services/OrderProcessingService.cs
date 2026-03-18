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
        private readonly ProductionRepository _productionRepo;
        private readonly MaterialRepository _materialsRepo;
        private readonly SupplierOrdersRepository _supplierOrderRepo;
        private readonly OrdersRepository _ordersRepo;
        private readonly ProductsRepository _productsRepo;

        public OrderProcessingService()
        {
            _productionRepo = new ProductionRepository();
            _materialsRepo = new MaterialRepository();
            _supplierOrderRepo = new SupplierOrdersRepository();
            _ordersRepo = new OrdersRepository();
            _productsRepo = new ProductsRepository();
        }

        public string ProcessOrder(
            OrdersModel order,
            List<OrderDetailsProductionModel> productions,
            List<OrderDetailsProductsModel> products)
        {
            var faltantes = new Dictionary<int, decimal>(); // material_id → cantidad

            // 🔹 1. CALCULAR MATERIALES DE PRODUCCIÓN
            foreach (var prod in productions)
            {
                var materials = _productionRepo
                    .GetRequiredMaterials(prod.ProductionId, prod.Quantity);

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

            // GENERAR PEDIDO A PROVEEDOR SI HAY FALTANTES
            if (faltantes.Any())
            {
                var supplierOrder = new SupplierOrderModel
                {
                    UserId = order.UserId,
                    SupplierId = 1, // mejorar esto después
                    Date = DateTime.Now,
                    TotalPrice = 0
                };

                int supplierOrderId = _supplierOrderRepo.CreateSupplierOrder(supplierOrder);

                var materialsList = new List<SupplierOrderMaterialModel>();

                foreach (var f in faltantes)
                {
                    materialsList.Add(new SupplierOrderMaterialModel
                    {
                        MaterialId = f.Key,
                        Quantity = f.Value,
                        UnitPrice = _materialsRepo.GetPurchasePrice(f.Key)
                    });
                }
                _supplierOrderRepo.SaveOrderMaterials(supplierOrderId, materialsList);
            }

            // 🔹 3. GUARDAR ORDEN
            int orderId = _ordersRepo.CreateOrder(order);

            _ordersRepo.SaveOrderProductions(orderId, productions);
            _ordersRepo.SaveOrderProducts(orderId, products);

            // 🔹 4. DESCONTAR STOCK DE PRODUCTOS
            foreach (var p in products)
            {
                _productsRepo.UpdateStock(p.ProductId, p.Quantity);
            }

            // 🔹 5. DESCONTAR STOCK DE MATERIALES (PRODUCCIÓN)
            foreach (var prod in productions)
            {
                var materials = _productionRepo
                    .GetRequiredMaterials(prod.ProductionId, prod.Quantity);

                foreach (var mat in materials)
                {
                    _materialsRepo.UpdateStock(mat.MaterialId, mat.Quantity);
                }
            }

            // 🔥 RESULTADO FINAL
            return faltantes.Any()
                ? "Orden creada y se generó pedido de material automáticamente"
                : "Orden creada correctamente";
        }
    }
}