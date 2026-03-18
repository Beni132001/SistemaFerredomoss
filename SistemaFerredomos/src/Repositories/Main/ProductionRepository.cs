using MySql.Data.MySqlClient;
using SistemaFerredomos.src.Models;
using SistemaFerredomos.src.Repositories.Commons;
using System.Collections.Generic;

namespace SistemaFerredomos.src.Repositories.Main
{
    class ProductionRepository
    {
        private readonly IDatabaseService _databaseService;

        public ProductionRepository()
        {
            _databaseService = new DatabaseService();
        }

        public List<ProductionModel> GetProductions()
        {
            var list = new List<ProductionModel>();

            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"SELECT 
                    id,
                    name,
                    type,
                    design_id,
                    price,
                    height,
                    width,
                    length
                FROM production";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ProductionModel
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Type = reader.GetString("type"),
                            Price = reader.GetDecimal("price"),
                            Height = reader.IsDBNull(reader.GetOrdinal("height")) ? null : reader.GetDecimal("height"),
                            Width = reader.IsDBNull(reader.GetOrdinal("width")) ? null : reader.GetDecimal("width")
                        });
                    }
                }
            }

            return list;
        }

        public List<ProductionMaterialModel> GetProductionMaterials(int productionId)
        {
            var materials = new List<ProductionMaterialModel>();

            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"SELECT pm.production_id,
                         pm.material_id,
                         pm.quantity,
                         m.name
                         FROM production_materials pm
                         JOIN materials m ON pm.material_id = m.id
                         WHERE pm.production_id = @productionId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(new ProductionMaterialModel
                            {
                                ProductionId = reader.GetInt32("production_id"),
                                MaterialId = reader.GetInt32("material_id"),
                                Quantity = reader.GetDecimal("quantity"),
                                Material = new MaterialModel
                                {
                                    Name = reader.GetString("name")
                                }
                            });
                        }
                    }
                }
            }

            return materials;
        }
        public Dictionary<int, decimal> CalculateMaterials(int productionId, int quantity)
        {
            var materials = GetProductionMaterials(productionId);

            var result = new Dictionary<int, decimal>();

            foreach (var material in materials)
            {
                result[material.MaterialId] = material.Quantity * quantity;
            }

            return result;
        }

        public void UpdateStock(int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE materials
                         SET stock = stock - @quantity
                         WHERE id = @id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@id", materialId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //para requerir materiales
        public List<RequiredMaterialModel> GetRequiredMaterials(int productionId, int quantity)
        {
            List<RequiredMaterialModel> list = new List<RequiredMaterialModel>();

            using (var connection = _databaseService.GetConnection())
            {
                connection.Open();

                string query = @"
        SELECT m.id, m.name, pm.quantity
        FROM production_materials pm
        JOIN materials m ON pm.material_id = m.id
        WHERE pm.production_id = @productionId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@productionId", productionId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RequiredMaterialModel
                            {
                                MaterialId = reader.GetInt32("id"),
                                MaterialName = reader.GetString("name"),
                                Quantity = reader.GetDecimal("quantity") * quantity
                            });
                        }
                    }
                }
            }

            return list;
        }

        //para vistas mejor
        public List<ProductionModel> GetProductionsByType(string type)
        {
            var list = new List<ProductionModel>();

            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = "SELECT * FROM production WHERE type = @type";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", type);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ProductionModel
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Type = reader.GetString("type"),
                                Price = reader.GetDecimal("price")
                            });
                        }
                    }
                }
            }

            return list;
        }

        //agregar material para produccion
        public void AddMaterialToProduction(int productionId, int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO production_materials
                         (production_id, material_id, quantity)
                         VALUES (@productionId, @materialId, @quantity)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@materialId", materialId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //actualizar cantidad
        public void UpdateMaterial(int productionId, int materialId, decimal quantity)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE production_materials
                         SET quantity = @quantity
                         WHERE production_id = @productionId
                         AND material_id = @materialId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@materialId", materialId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //eliminar cantidad
        public void DeleteMaterial(int productionId, int materialId)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"DELETE FROM production_materials
                         WHERE production_id = @productionId
                         AND material_id = @materialId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productionId", productionId);
                    cmd.Parameters.AddWithValue("@materialId", materialId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //insertar
        public void AddProduction(ProductionModel production)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO production
            (type, name, design_id, price, height, width, length)
            VALUES
            (@type, @name, @design_id, @price, @height, @width, @length)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", production.Type);
                    cmd.Parameters.AddWithValue("@name", production.Name);
                    cmd.Parameters.AddWithValue("@design_id", production.DesignId);
                    cmd.Parameters.AddWithValue("@price", production.Price);
                    cmd.Parameters.AddWithValue("@height", production.Height);
                    cmd.Parameters.AddWithValue("@width", production.Width);
                    cmd.Parameters.AddWithValue("@length", production.Length);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //actualizar
        public void UpdateProduction(ProductionModel production)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = @"UPDATE production SET
            type=@type,
            name=@name,
            design_id=@design_id,
            price=@price,
            height=@height,
            width=@width,
            length=@length
            WHERE id=@id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", production.Id);
                    cmd.Parameters.AddWithValue("@type", production.Type);
                    cmd.Parameters.AddWithValue("@name", production.Name);
                    cmd.Parameters.AddWithValue("@design_id", production.DesignId);
                    cmd.Parameters.AddWithValue("@price", production.Price);
                    cmd.Parameters.AddWithValue("@height", production.Height);
                    cmd.Parameters.AddWithValue("@width", production.Width);
                    cmd.Parameters.AddWithValue("@length", production.Length);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //eliminar
        public void DeleteProduction(int id)
        {
            using (var conn = _databaseService.GetConnection())
            {
                conn.Open();

                string query = "DELETE FROM production WHERE id=@id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}