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

-- catalog tables
CREATE TABLE colors (
    code  VARCHAR(30) PRIMARY KEY,
    name  VARCHAR(30) NOT NULL
);

 CREATE TABLE profiles (
    code  VARCHAR(30) PRIMARY KEY,
    name  VARCHAR(30) NOT NULL,
    size  DOUBLE      NOT NULL
);
 
CREATE TABLE glass (
    code      VARCHAR(30) PRIMARY KEY,
    name      VARCHAR(30) NOT NULL,
    width     DOUBLE,
    height    DOUBLE,
    thickness DOUBLE
);

-- Materials Table
CREATE TABLE materials (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    name           VARCHAR(100)  NOT NULL,
    stock          DECIMAL(10,2) NOT NULL DEFAULT 0,
    purchase_price DECIMAL(10,2) NOT NULL,
    sale_price     DECIMAL(10,2) NOT NULL,
    supplier_id    INT,
    image          VARCHAR(255),
    code           VARCHAR(30),
    size           DOUBLE,
    shelf          VARCHAR(20),
    color_code     VARCHAR(30),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
    FOREIGN KEY (color_code)  REFERENCES colors(code)
);

-- Products Table
CREATE TABLE products (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    name           VARCHAR(100)  NOT NULL,
    image          VARCHAR(255),
    stock          INT           NOT NULL DEFAULT 0,
    purchase_price DECIMAL(10,2) NOT NULL,
    sale_price     DECIMAL(10,2) NOT NULL,
    supplier_id    INT,
    mx             VARCHAR(30)   UNIQUE,
    code           VARCHAR(30),
    size           DOUBLE,
    color_code     VARCHAR(30),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
    FOREIGN KEY (color_code)  REFERENCES colors(code)
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
    id             INT AUTO_INCREMENT PRIMARY KEY,
    user_id        INT           NOT NULL,
    date           DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    customer_name  VARCHAR(100),
    customer_phone VARCHAR(20),
    status         ENUM('pendiente','en_proceso','completa','cancelada') NOT NULL DEFAULT 'pendiente',
    total_price    DECIMAL(10,2) NOT NULL,
    supplied       VARCHAR(30),
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
    id          INT AUTO_INCREMENT PRIMARY KEY,
    user_id     INT           NOT NULL,
    date        DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    supplier_id INT           NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    status      ENUM('pendiente','terminado') NOT NULL DEFAULT 'pendiente',
    FOREIGN KEY (user_id)     REFERENCES users(id),
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

-- desglose
CREATE TABLE breakdown (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    order_number VARCHAR(30)   NOT NULL,
    profile_code VARCHAR(30),
    profile_name VARCHAR(30),
    size         DOUBLE,
    color        VARCHAR(50),
    quantity     DECIMAL(10,2) DEFAULT 1,
    created_at   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (profile_code) REFERENCES profiles(code)
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
