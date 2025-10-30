# 🛠️ Database Setup for Auction Clock System (Veiling Klok DB )

This guide outlines the steps to set up the **`VeilingKlokDB`** (Auction Clock Database) in SQL Server using SQL Server Management Studio (SSMS).

***

## Stap 1: Open SSMS en Server Details Invoeren

1.  Open **SQL Server Management Studio (SSMS)**.
2.  Controleer welke naam er staat in het veld **Server-Name**. Bijvoorbeeld `(localdb)\MSSQLLocalDB`.
3.  **Authentication:** Selecteer **Windows Authentication**.
4.  **Database Name:** Laat staan op `<default>`.
5.  Klik op **Connect**.

---

## Stap 2: Open het VVS Project en Update de Database

**(Note: I've renumbered this section to follow Stap 1 logically, as "Stap 3" was used before.)**

1.  Navigeer naar het bestand **`appsettings.json`**.
2.  Pas de waarde van het veld **`DefaultConnection`** aan naar het juiste formaat, bijvoorbeeld:
    `"Server=(localdb)\MSSQLLocalDB;Database=VeilingKlokDB;Trusted_Connection=True;MultipleActiveResultSets=true"`
    **(Let op: Vul hier de correcte servernaam en gewenste databasenaam in.)**
3.  Voer de volgende command uit om de database aan te maken of bij te werken:
    A. Als je in **Visual Studio Community** werkt, run de volgende command in **Package Manager Console**:

    ```powershell
    Update-Database
    ```

    B. Als je in **Visual Studio Code** werkt, run de volgende in je **Terminal**:

    ```bash
    dotnet ef database update
    ```

4.  Dan ben je **ready to go!**

---

## Important: Migraties uitvoeren voor Database Aanpassingen

Elke keer dat je iets in de database wilt aanpassen (zoals tabellen, kolommen of relaties), moet je in de terminal **eerst een migratie uitvoeren** voordat je de database bijwerkt.

A. Visual Studio Community (Package Manager Console):
```powershell
Add-Migration <NaamVanMigratie>
Update-Database
```
B. Visual Studio Code (Terminal):
```bash
dotnet ef migrations add <NaamVanMigratie>
dotnet ef database update
```
