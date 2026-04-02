use BDSISTEMAFERREDOMOS;

ALTER TABLE products 
ADD COLUMN color_code VARCHAR(30) NULL AFTER size;

ALTER TABLE materials
    ADD CONSTRAINT fk_materials_color
    FOREIGN KEY (color_code) REFERENCES colors(code);
    
ALTER TABLE products
    ADD CONSTRAINT uq_products_mx UNIQUE (mx);    
    
ALTER TABLE products
    ADD CONSTRAINT fk_products_color
    FOREIGN KEY (color_code) REFERENCES colors(code);
    
ALTER TABLE orders
    ADD COLUMN supplied VARCHAR(30) NULL AFTER total_price;
    
ALTER TABLE supplier_orders
    ADD COLUMN status ENUM('pendiente','terminado') NOT NULL DEFAULT 'pendiente' AFTER total_price;
    
INSERT IGNORE INTO colors VALUES
    ('WHT', 'White'),
    ('BLK', 'Black'),
    ('CHP', 'Champagne'),
    ('BRZ', 'Bronze'),
    ('ANO', 'Anodized'),
    ('RAW', 'Raw');
    
    
INSERT IGNORE INTO profiles VALUES
    ('P035', 'Serie 35',   35.0),
    ('P040', 'Serie 40',   40.0),
    ('P045', 'Serie 45',   45.0),
    ('P100', 'Serie 100', 100.0);
    
    
DROP PROCEDURE IF EXISTS GetAllMaterials;
DROP PROCEDURE IF EXISTS GetMaterialById;
DROP PROCEDURE IF EXISTS AddMaterial;
DROP PROCEDURE IF EXISTS UpdateMaterial;
DROP PROCEDURE IF EXISTS DeleteMaterial;
 
DROP PROCEDURE IF EXISTS GetAllProducts;
DROP PROCEDURE IF EXISTS AddProduct;
DROP PROCEDURE IF EXISTS UpdateProduct;
DROP PROCEDURE IF EXISTS DeleteProduct;
 
DROP PROCEDURE IF EXISTS GetAllSuppliers;


-- RECREAR GetAllMaterials (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllMaterials()
BEGIN
    SELECT m.id, m.name, m.stock, m.purchase_price, m.sale_price,
           m.image, m.code, m.size, m.shelf, m.color_code,
           s.id AS supplier_id, s.name AS supplier_name, s.phone, s.address
    FROM materials m
    LEFT JOIN suppliers s ON m.supplier_id = s.id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR GetMaterialById (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetMaterialById(IN p_id INT)
BEGIN
    SELECT m.id, m.name, m.stock, m.purchase_price, m.sale_price,
           m.image, m.code, m.size, m.shelf, m.color_code,
           s.id AS supplier_id, s.name AS supplier_name, s.phone, s.address
    FROM materials m
    LEFT JOIN suppliers s ON m.supplier_id = s.id
    WHERE m.id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR AddMaterial (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE AddMaterial(
    IN p_name           VARCHAR(100),
    IN p_stock          DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price     DECIMAL(10,2),
    IN p_supplier_id    INT,
    IN p_image          VARCHAR(255),
    IN p_code           VARCHAR(30),
    IN p_size           DOUBLE,
    IN p_shelf          VARCHAR(20),
    IN p_color_code     VARCHAR(30)
)
BEGIN
    INSERT INTO materials
        (name, stock, purchase_price, sale_price, supplier_id, image, code, size, shelf, color_code)
    VALUES
        (p_name, p_stock, p_purchase_price, p_sale_price, p_supplier_id,
         p_image, p_code, p_size, p_shelf, p_color_code);
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR UpdateMaterial (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE UpdateMaterial(
    IN p_id             INT,
    IN p_name           VARCHAR(100),
    IN p_stock          DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price     DECIMAL(10,2),
    IN p_supplier_id    INT,
    IN p_image          VARCHAR(255),
    IN p_code           VARCHAR(30),
    IN p_size           DOUBLE,
    IN p_shelf          VARCHAR(20),
    IN p_color_code     VARCHAR(30)
)
BEGIN
    UPDATE materials
    SET name           = p_name,
        stock          = p_stock,
        purchase_price = p_purchase_price,
        sale_price     = p_sale_price,
        supplier_id    = p_supplier_id,
        image          = p_image,
        code           = p_code,
        size           = p_size,
        shelf          = p_shelf,
        color_code     = p_color_code
    WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR DeleteMaterial
-- ============================================================
DELIMITER //
CREATE PROCEDURE DeleteMaterial(IN p_id INT)
BEGIN
    DELETE FROM materials WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR GetAllProducts (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllProducts()
BEGIN
    SELECT p.id, p.name, p.stock, p.purchase_price, p.sale_price, p.image,
           p.mx, p.code, p.size, p.color_code,
           s.id AS supplier_id, s.name AS supplier_name, s.phone, s.address
    FROM products p
    LEFT JOIN suppliers s ON p.supplier_id = s.id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR AddProduct (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE AddProduct(
    IN p_name           VARCHAR(100),
    IN p_stock          DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price     DECIMAL(10,2),
    IN p_supplier_id    INT,
    IN p_image          VARCHAR(255),
    IN p_mx             VARCHAR(30),
    IN p_code           VARCHAR(30),
    IN p_size           DOUBLE,
    IN p_color_code     VARCHAR(30)
)
BEGIN
    INSERT INTO products
        (name, stock, purchase_price, sale_price, supplier_id, image, mx, code, size, color_code)
    VALUES
        (p_name, p_stock, p_purchase_price, p_sale_price, p_supplier_id,
         p_image, p_mx, p_code, p_size, p_color_code);
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR UpdateProduct (con campos nuevos)
-- ============================================================
DELIMITER //
CREATE PROCEDURE UpdateProduct(
    IN p_id             INT,
    IN p_name           VARCHAR(100),
    IN p_stock          DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price     DECIMAL(10,2),
    IN p_supplier_id    INT,
    IN p_image          VARCHAR(255),
    IN p_mx             VARCHAR(30),
    IN p_code           VARCHAR(30),
    IN p_size           DOUBLE,
    IN p_color_code     VARCHAR(30)
)
BEGIN
    UPDATE products
    SET name           = p_name,
        stock          = p_stock,
        purchase_price = p_purchase_price,
        sale_price     = p_sale_price,
        supplier_id    = p_supplier_id,
        image          = p_image,
        mx             = p_mx,
        code           = p_code,
        size           = p_size,
        color_code     = p_color_code
    WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR DeleteProduct
-- ============================================================
DELIMITER //
CREATE PROCEDURE DeleteProduct(IN p_id INT)
BEGIN
    DELETE FROM products WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- RECREAR GetAllSuppliers
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllSuppliers()
BEGIN
    SELECT id, name, phone, address FROM suppliers;
END //
DELIMITER ;


SHOW PROCEDURE STATUS WHERE Db = 'BDSISTEMAFERREDOMOS';