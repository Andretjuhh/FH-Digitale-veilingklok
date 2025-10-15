# 🛠️ Database Setup for Auction Clock System (Veiling Klok DB)

This guide outlines the steps to set up the `VeilingKlokDB` (Auction Clock Database) in SQL Server using SQL Server Management Studio (SSMS).

***

## Stap 1: Open SSMS en Verbind

1.  Open **SQL Server Management Studio (SSMS)**.
2.  Open de database engine (druk op **Connect**).

---

## Stap 2: Server Details Invoeren

Vul de volgende verbindingsgegevens in:

* **Server Name:** Vul de naam van uw server in. Bijvoorbeeld: **`(localdb)\MSSQLLocalDB`**.
* **Authentication:** Selecteer **Windows Authentication**.
* **Database Name:** Laat staan op `<default>`.
* Druk op **Connect**.

---

## Stap 3: Query Uitvoeren

1.  Open een **New Query** Tab.
2.  Voeg de volgende SQL code toe en voer deze uit om de database en tabellen aan te maken:

### SQL Code:

```sql
-- 1. Clean up existing database
IF DB_ID(N'VeilingKlokDB') IS NOT NULL
BEGIN
    ALTER DATABASE VeilingKlokDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
    DROP DATABASE VeilingKlokDB;
END
GO

CREATE DATABASE VeilingKlokDB;
GO
USE VeilingKlokDB;
GO

-- Drop old tables if they exist
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS VeilingKlok;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Veilingmeester;
DROP TABLE IF EXISTS Kweker;
DROP TABLE IF EXISTS Koper;
DROP TABLE IF EXISTS Account;
GO


-- ==============================
-- TABLES
-- ==============================

CREATE TABLE Account (
    id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(255) UNIQUE NOT NULL,
    password NVARCHAR(255) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);

-- KOPER (Buyer)
CREATE TABLE Koper (
    account_id INT PRIMARY KEY,  -- FK to Account
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    adress NVARCHAR(255),
    post_code NVARCHAR(20),
    regio NVARCHAR(100),
    
    FOREIGN KEY (account_id) REFERENCES Account(id)
);

-- VEILINGMEESTER (Auction master)
CREATE TABLE Veilingmeester (
    account_id INT PRIMARY KEY, -- FK to Account
    soort_veiling NVARCHAR(100),

    FOREIGN KEY (account_id) REFERENCES Account(id)
);

-- KWEKER (Grower)
CREATE TABLE Kweker (
    account_id INT PRIMARY KEY, -- FK to Account
    name NVARCHAR(255) NOT NULL,
    telephone NVARCHAR(20),
    adress NVARCHAR(255),
    regio NVARCHAR(100),
    kvk_nmr NVARCHAR(50),

    FOREIGN KEY (account_id) REFERENCES Account(id)
);

-- VEILINGKLOK (Auction clock)
CREATE TABLE VeilingKlok (
    id INT PRIMARY KEY IDENTITY(1,1),
    veilingmeester_id INT NOT NULL,  -- FK naar Veilingmeester
    name NVARCHAR(255) NOT NULL,
    sequence_id INT,
    sequence_duration INT,
    location NVARCHAR(255),
    live_views INT DEFAULT 0,
    started_at DATETIME,
    ended_at DATETIME,

    FOREIGN KEY (veilingmeester_id) REFERENCES Veilingmeester(account_id)
);

-- PRODUCT
CREATE TABLE Product (
    id INT PRIMARY KEY IDENTITY(1,1),
    kweker_id INT NOT NULL,  -- FK naar Kweker
    veilingklok_id INT NOT NULL, -- FK naar VeilingKlok
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    price DECIMAL(10,2) NOT NULL,
    minimum_price DECIMAL(10,2),
    quantity INT NOT NULL,
    image_url NVARCHAR(MAX),
    size NVARCHAR(50),

    FOREIGN KEY (kweker_id) REFERENCES Kweker(account_id),
    FOREIGN KEY (veilingklok_id) REFERENCES VeilingKlok(id)
);

-- ORDER
CREATE TABLE [Order] (
    id INT PRIMARY KEY IDENTITY(1,1),
    koper_id INT NOT NULL,    -- FK naar Koper
    product_id INT NOT NULL,  -- FK naar Product
    quantity INT NOT NULL,
    bought_at DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (koper_id) REFERENCES Koper(account_id),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

```
## Stap 4: Verander Naam Van DataBase
Nu dat de database is aangemaakt moeten we de naam veranderen. 
1. In de Object Explorer tab druk op Connect en selecteer Database Engine. 
2. Verander hier de Database Name naar VeilingKlokDB. 