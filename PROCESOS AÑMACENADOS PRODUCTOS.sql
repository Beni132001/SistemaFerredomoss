USE BDSISTEMAFERREDOMOS;

-- Obtener todos los productos
DELIMITER //
CREATE PROCEDURE GetAllProducts()
BEGIN
    SELECT p.id, p.name, p.stock, p.purchase_price, p.sale_price, p.image,
           s.id AS supplier_id, s.name AS supplier_name, s.phone, s.address
    FROM products p
    LEFT JOIN suppliers s ON p.supplier_id = s.id;
END //
DELIMITER ;

-- Insertar producto
DELIMITER //
CREATE PROCEDURE AddProduct(
    IN p_name VARCHAR(100),
    IN p_stock DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price DECIMAL(10,2),
    IN p_supplier_id INT,
    IN p_image VARCHAR(255)
)
BEGIN
    INSERT INTO products (name, stock, purchase_price, sale_price, supplier_id, image)
    VALUES (p_name, p_stock, p_purchase_price, p_sale_price, p_supplier_id, p_image);
END //
DELIMITER ;

-- Actualizar producto
DELIMITER //
CREATE PROCEDURE UpdateProduct(
    IN p_id INT,
    IN p_name VARCHAR(100),
    IN p_stock DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price DECIMAL(10,2),
    IN p_supplier_id INT,
    IN p_image VARCHAR(255)
)
BEGIN
    UPDATE products
    SET name = p_name,
        stock = p_stock,
        purchase_price = p_purchase_price,
        sale_price = p_sale_price,
        supplier_id = p_supplier_id,
        image = p_image
    WHERE id = p_id;
END //
DELIMITER ;

-- Eliminar producto
DELIMITER //
CREATE PROCEDURE DeleteProduct(IN p_id INT)
BEGIN
    DELETE FROM products WHERE id = p_id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE GetAllSuppliers()
BEGIN
    SELECT * FROM suppliers;
END //
DELIMITER ;

USE BDSISTEMAFERREDOMOS;

DELIMITER //
CREATE PROCEDURE GetAllSuppliers()
BEGIN
    SELECT id, name, phone, address
    FROM suppliers;
END //
DELIMITER ;
