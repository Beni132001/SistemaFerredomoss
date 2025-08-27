use BDSISTEMAFERREDOMOS;
INSERT INTO users (name, username, password, type) VALUES
('Administrador General', 'admin', 'admin123', 'admin'),
('Juan Pérez', 'juanp', 'clavejuan', 'user'),
('María López', 'marial', 'clavemaria', 'user');

INSERT INTO suppliers (name, phone, address) VALUES
('AluProveedora SA', '555-123-4567', 'Calle Industrial 45, Ciudad de México'),
('Vidrios y Cristales Martínez', '555-987-6543', 'Av. Central 300, Puebla'),
('Herrajes Duramax', '555-321-7890', 'Calle Metalurgia 12, Monterrey');

INSERT INTO designs (name, color, image, description) VALUES
('Ventana corrediza doble', 'Blanco', 'ventana_corrediza_blanca.jpg', 'Ventana de aluminio blanco con dos hojas corredizas'),
('Puerta abatible cristal', 'Negro', 'puerta_abatible_negra.jpg', 'Puerta de cristal templado con marco negro'),
('Domo policarbonato', 'Transparente', 'domo_transparente.jpg', 'Domo de policarbonato resistente a rayos UV'),
('Vidrio esmerilado decorativo', 'Esmerilado', 'vidrio_esmerilado.jpg', 'Vidrio decorativo para privacidad');

INSERT INTO materials (name, stock, purchase_price, sale_price, supplier_id) VALUES
('Perfil de aluminio 6m', 50, 250.00, 350.00, 1),
('Vidrio templado 6mm', 30, 500.00, 700.00, 2),
('Policarbonato alveolar 6mm', 20, 450.00, 650.00, 2),
('Bisagra acero inoxidable', 100, 35.00, 55.00, 3),
('Tornillo autorroscante', 500, 1.50, 3.00, 3);

INSERT INTO products (name, image, stock, purchase_price, sale_price, supplier_id) VALUES
('Cerradura multipunto', 'cerradura_multipunto.jpg', 25, 280.00, 450.00, 3),
('Manija aluminio', 'manija_aluminio.jpg', 40, 50.00, 90.00, 3),
('Silicón sellador transparente', 'silicon_sellador.jpg', 60, 35.00, 60.00, 3);

INSERT INTO production (type, name, design_id, price, height, width, length) VALUES
('ventanas', 'Ventana corrediza doble 1.2x1.0m', 1, 1800.00, 120.00, 100.00, NULL),
('puertas', 'Puerta abatible cristal 2.0x0.8m', 2, 2500.00, 200.00, 80.00, NULL),
('domos', 'Domo policarbonato 1.5x1.5m', 3, 2200.00, 150.00, 150.00, NULL),
('vidrios', 'Vidrio esmerilado 1.0x0.8m', 4, 900.00, 100.00, 80.00, NULL);

INSERT INTO production_materials (production_id, material_id, quantity) VALUES
(1, 1, 2),  -- Ventana corrediza, perfiles de aluminio
(1, 2, 1),  -- Vidrio templado
(2, 1, 2),
(2, 2, 1),
(2, 4, 2),
(3, 3, 2),
(3, 5, 20),
(4, 2, 1);

INSERT INTO orders (user_id, customer_name, customer_phone, status, total_price) VALUES
(2, 'Carlos García', '555-444-1122', 'pendiente', 4300.00),
(3, 'Lucía Fernández', '555-333-8899', 'completa', 2500.00);

INSERT INTO order_production (order_id, production_id, quantity, unit_price) VALUES
(1, 1, 1, 1800.00),
(1, 2, 1, 2500.00),
(2, 4, 1, 900.00);

INSERT INTO order_products (order_id, product_id, quantity, unit_price) VALUES
(1, 1, 1, 450.00),
(1, 2, 2, 90.00),
(2, 3, 3, 60.00);

INSERT INTO supplier_orders (user_id, supplier_id, total_price) VALUES
(1, 1, 5000.00),
(1, 3, 2000.00);

INSERT INTO supplier_order_materials (supplier_order_id, material_id, quantity, unit_price) VALUES
(1, 1, 20, 250.00),
(1, 2, 5, 500.00),
(2, 4, 30, 35.00),
(2, 5, 200, 1.50);

INSERT INTO supplier_order_hardware_products (supplier_order_id, products_id, quantity, unit_price) VALUES
(2, 1, 10, 280.00),
(2, 2, 15, 50.00);

INSERT INTO activity_logs (user_id, activity, reference_id) VALUES
(1, 'login', NULL),
(1, 'provedor_orden', 1),
(2, 'orden', 1),
(3, 'orden', 2),
(1, 'logout', NULL);

