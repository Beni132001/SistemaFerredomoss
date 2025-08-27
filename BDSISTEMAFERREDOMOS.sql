CREATE DATABASE BDSISTEMAFERREDOMOS;
USE BDSISTEMAFERREDOMOS;

-- Users Table
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    type ENUM('admin', 'user') NOT NULL
);

-- Suppliers Table
CREATE TABLE suppliers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    address TEXT
);

-- Designs Table
CREATE TABLE designs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    color VARCHAR(50),
    image VARCHAR(255),
    description TEXT
);

-- Materials Table
CREATE TABLE materials (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    stock DECIMAL(10,2) NOT NULL DEFAULT 0,
    purchase_price DECIMAL(10,2) NOT NULL,
    sale_price DECIMAL(10,2) NOT NULL,
    supplier_id INT,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id)
);

-- Products Table
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    image VARCHAR(255),
    stock INT NOT NULL DEFAULT 0,
    purchase_price DECIMAL(10,2) NOT NULL,
    sale_price DECIMAL(10,2) NOT NULL,
    supplier_id INT,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id)
);

-- Production Table
CREATE TABLE production(
    id INT AUTO_INCREMENT PRIMARY KEY,
    type ENUM('domos', 'puertas', 'ventanas', 'vidrios') NOT NULL,
    name VARCHAR(100) NOT NULL,
    design_id INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    height DECIMAL(8,2),
    width DECIMAL(8,2),
    length DECIMAL(8,2),
    FOREIGN KEY (design_id) REFERENCES designs(id)
);

-- Production (Many-to-Many)
CREATE TABLE production_materials(
    production_id INT NOT NULL,
    material_id INT NOT NULL,
    quantity DECIMAL(8,2) NOT NULL,
    PRIMARY KEY (production_id, material_id),
    FOREIGN KEY (production_id) REFERENCES production(id),
    FOREIGN KEY (material_id) REFERENCES materials(id)
);

-- Orders Table
CREATE TABLE orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    customer_name VARCHAR(100),
    customer_phone VARCHAR(20),
    status ENUM('pendiente', 'cancelada', 'completa') NOT NULL DEFAULT 'pendiente',
    total_price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Order Details (Production)
CREATE TABLE order_production (
    order_id INT NOT NULL,
    production_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (order_id, production_id),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (production_id) REFERENCES production(id)
);

-- Order Details (Products)
CREATE TABLE order_products (
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Supplier Orders Table
CREATE TABLE supplier_orders (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    supplier_id INT NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id)
);

-- Supplier Order Details (Materials)
CREATE TABLE supplier_order_materials (
    supplier_order_id INT NOT NULL,
    material_id INT NOT NULL,
    quantity DECIMAL(10,2) NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (supplier_order_id, material_id),
    FOREIGN KEY (supplier_order_id) REFERENCES supplier_orders(id),
    FOREIGN KEY (material_id) REFERENCES materials(id)
);

-- Supplier Order Details (Products)
CREATE TABLE supplier_order_hardware_products (
    supplier_order_id INT NOT NULL,
    products_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (supplier_order_id, products_id),
    FOREIGN KEY (supplier_order_id) REFERENCES supplier_orders(id),
    FOREIGN KEY (products_id) REFERENCES products(id)
);

-- Activity Log Table
CREATE TABLE activity_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    activity ENUM('login', 'logout', 'orden', 'provedor_orden') NOT NULL,
    reference_id INT, -- ID of order or supplier order
    date_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);