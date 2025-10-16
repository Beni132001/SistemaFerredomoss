use BDSISTEMAFERREDOMOS;
-- 🟢 Obtener todos los materiales
DELIMITER //
CREATE PROCEDURE GetAllMaterials()
BEGIN
    SELECT m.id, m.name, m.stock, m.purchase_price, m.sale_price, 
           s.id AS supplier_id, s.name AS supplier_name, s.phone, s.address
    FROM materials m
    LEFT JOIN suppliers s ON m.supplier_id = s.id;
END //
DELIMITER ;

-- 🟢 Insertar material
DELIMITER //
CREATE PROCEDURE AddMaterial(
    IN p_name VARCHAR(100),
    IN p_stock DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price DECIMAL(10,2),
    IN p_supplier_id INT
)
BEGIN
    INSERT INTO materials (name, stock, purchase_price, sale_price, supplier_id)
    VALUES (p_name, p_stock, p_purchase_price, p_sale_price, p_supplier_id);
END //
DELIMITER ;

-- 🟡 Actualizar material
DELIMITER //
CREATE PROCEDURE UpdateMaterial(
    IN p_id INT,
    IN p_name VARCHAR(100),
    IN p_stock DECIMAL(10,2),
    IN p_purchase_price DECIMAL(10,2),
    IN p_sale_price DECIMAL(10,2),
    IN p_supplier_id INT
)
BEGIN
    UPDATE materials
    SET name = p_name,
        stock = p_stock,
        purchase_price = p_purchase_price,
        sale_price = p_sale_price,
        supplier_id = p_supplier_id
    WHERE id = p_id;
END //
DELIMITER ;

-- 🔴 Eliminar material
DELIMITER //
CREATE PROCEDURE DeleteMaterial(IN p_id INT)
BEGIN
    DELETE FROM materials WHERE id = p_id;
END //
DELIMITER ;

CALL GetAllMaterials();