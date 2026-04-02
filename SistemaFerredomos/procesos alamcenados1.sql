use BDSISTEMAFERREDOMOS;
DELIMITER //
CREATE PROCEDURE GetAllColors()
BEGIN
    SELECT code, name FROM colors ORDER BY name;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE AddColor(IN p_code VARCHAR(30), IN p_name VARCHAR(30))
BEGIN
    INSERT INTO colors (code, name) VALUES (p_code, p_name);
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE UpdateColor(IN p_code VARCHAR(30), IN p_name VARCHAR(30))
BEGIN
    UPDATE colors SET name = p_name WHERE code = p_code;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE DeleteColor(IN p_code VARCHAR(30))
BEGIN
    DELETE FROM colors WHERE code = p_code;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – PROFILES ★
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllProfiles()
BEGIN
    SELECT code, name, size FROM profiles ORDER BY name;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE AddProfile(
    IN p_code VARCHAR(30),
    IN p_name VARCHAR(30),
    IN p_size DOUBLE
)
BEGIN
    INSERT INTO profiles (code, name, size) VALUES (p_code, p_name, p_size);
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE UpdateProfile(
    IN p_code VARCHAR(30),
    IN p_name VARCHAR(30),
    IN p_size DOUBLE
)
BEGIN
    UPDATE profiles SET name = p_name, size = p_size WHERE code = p_code;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE DeleteProfile(IN p_code VARCHAR(30))
BEGIN
    DELETE FROM profiles WHERE code = p_code;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – GLASS ★
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllGlass()
BEGIN
    SELECT code, name, width, height, thickness FROM glass ORDER BY name;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE GetGlassByCode(IN p_search VARCHAR(30))
BEGIN
    SELECT code, name, width, height, thickness
    FROM glass
    WHERE code LIKE CONCAT('%', p_search, '%')
       OR name LIKE CONCAT('%', p_search, '%');
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE AddGlass(
    IN p_code      VARCHAR(30),
    IN p_name      VARCHAR(30),
    IN p_width     DOUBLE,
    IN p_height    DOUBLE,
    IN p_thickness DOUBLE
)
BEGIN
    INSERT INTO glass (code, name, width, height, thickness)
    VALUES (p_code, p_name, p_width, p_height, p_thickness);
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE UpdateGlass(
    IN p_code      VARCHAR(30),
    IN p_name      VARCHAR(30),
    IN p_width     DOUBLE,
    IN p_height    DOUBLE,
    IN p_thickness DOUBLE
)
BEGIN
    UPDATE glass
    SET name = p_name, width = p_width, height = p_height, thickness = p_thickness
    WHERE code = p_code;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE DeleteGlass(IN p_code VARCHAR(30))
BEGIN
    DELETE FROM glass WHERE code = p_code;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – MATERIALS (updated)
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
 
DELIMITER //
CREATE PROCEDURE DeleteMaterial(IN p_id INT)
BEGIN
    DELETE FROM materials WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – PRODUCTS (updated)
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
 
DELIMITER //
CREATE PROCEDURE DeleteProduct(IN p_id INT)
BEGIN
    DELETE FROM products WHERE id = p_id;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – SUPPLIERS (unchanged)
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllSuppliers()
BEGIN
    SELECT id, name, phone, address FROM suppliers;
END //
DELIMITER ;
 
-- ============================================================
-- STORED PROCEDURES – BREAKDOWN ★
-- ============================================================
DELIMITER //
CREATE PROCEDURE GetAllBreakdowns()
BEGIN
    SELECT id, order_number, profile_code, profile_name,
           size, color, quantity, created_at
    FROM breakdown
    ORDER BY created_at DESC;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE GetBreakdownByOrder(IN p_order_number VARCHAR(30))
BEGIN
    SELECT id, order_number, profile_code, profile_name,
           size, color, quantity, created_at
    FROM breakdown
    WHERE order_number = p_order_number;
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE AddBreakdown(
    IN p_order_number VARCHAR(30),
    IN p_profile_code VARCHAR(30),
    IN p_profile_name VARCHAR(30),
    IN p_size         DOUBLE,
    IN p_color        VARCHAR(50),
    IN p_quantity     DECIMAL(10,2)
)
BEGIN
    INSERT INTO breakdown (order_number, profile_code, profile_name, size, color, quantity)
    VALUES (p_order_number, p_profile_code, p_profile_name, p_size, p_color, p_quantity);
END //
DELIMITER ;
 
DELIMITER //
CREATE PROCEDURE DeleteBreakdown(IN p_id INT)
BEGIN
    DELETE FROM breakdown WHERE id = p_id;
END //
DELIMITER ;
 