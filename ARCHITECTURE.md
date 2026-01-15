# Architectuurdocument FH-Digitale-veilingklok

## 1. Projectoverzicht
De FH-Digitale-veilingklok bestaat uit een React frontend en een ASP.NET Core backend met een SQL Server database. De kern van het systeem is een digitale veilingklok met realtime prijsupdates en biedingen via SignalR.

## 2. System context
**Gebruikersrollen**
- Koper: bekijkt veilingen, plaatst biedingen en maakt orders.
- Kweker: beheert producten en ziet orderinformatie.
- Veilingmeester: beheert veilingen en klokken.
- Admin: beheert accounts.

**Hoofdcomponenten**
- FrontendWeb (React): UI, routing, state, SignalR client.
- BackendAPI (ASP.NET Core): REST API, Identity, business logic, realtime services.
- SQL Server database: persistente opslag.

## 3. Architectuurstijl
Het backend volgt een Clean Architecture-achtige opzet:
- **API**: Controllers, routing, middleware, CORS, swagger.
- **Application**: use cases (MediatR), DTOs, mappers, interfaces.
- **Domain**: entities, value objects, enums en domeinregels.
- **Infrastructure**: EF Core, repositories, Identity, SignalR engine.

Deze scheiding houdt domeinlogica los van infrastructuur en UI.

## 4. Backend architectuur
**API laag**
- `BackendAPI/API/Program.cs` configureert services, middleware, CORS en endpoints.
- Controllers per rol:
  - `api/account` (auth en account info)
  - `api/account/koper` (koper acties)
  - `api/account/kweker` (kweker acties)
  - `api/account/meester` (veilingmeester acties)
  - `api/account/admin` (admin acties)
- Identity endpoints worden gemapt via `MapIdentityApi`.

**Application laag**
- MediatR handlers in `BackendAPI/Application/UseCases/**`.
- DTOs in `BackendAPI/Application/DTOs/**`.
- Mappers in `BackendAPI/Application/Common/Mappers/**`.
- Interfaces voor repositories en services in `BackendAPI/Application/Repositories/**` en `BackendAPI/Application/Services/**`.

**Domain laag**
- Entiteiten zoals `Account`, `Koper`, `Kweker`, `VeilingKlok`, `Product`, `Order`.
- Enums: `AccountType`, `OrderStatus`, `VeilingKlokStatus`.
- Domeinexceptions in `BackendAPI/Domain/Exceptions/**`.

**Infrastructure laag**
- EF Core context in `BackendAPI/Infrastructure/Persistence/Data/AppDbContext.cs`.
- Repositories in `BackendAPI/Infrastructure/Persistence/Repositories/**`.
- Identity configuratie in `BackendAPI/Infrastructure/Extensions/AuthenticationExtension.cs`.
- SignalR services in `BackendAPI/Infrastructure/Microservices/SignalR/**`.

## 5. Realtime veilingklok
- `VeilingKlokEngine` is een hosted service en singleton die actieve klokken beheert.
- SignalR hub `VeilingHub` exposeert realtime events.
- Frontend hook `useVeilingKlokSignalR` abonneert op events en beheert groepen (region/clock).

## 6. Frontend architectuur
**Techniek**
- React 19, CRA, TypeScript, React Router, Framer Motion.
- Styling via SCSS/CSS en utility styles.

**Routing**
- Centrale routes in `FrontendWeb/src/App.tsx`.
- Routes per rol (koper, kweker, veilingmeester, admin).

**State en services**
- Contexts in `FrontendWeb/src/components/contexts/**`.
- API integratie via controllers in `FrontendWeb/src/controllers/**`.
- SignalR client in `FrontendWeb/src/hooks/useVeilingKlokSignalR.ts`.

**Configuratie**
- `REACT_APP_API_BASE_URL` in `FrontendWeb/src/constant/application.ts` bepaalt API en SignalR base URL.

## 7. Data model (hoog niveau)
Belangrijkste tabellen (zie `AppDbContext`):
- Account (TPT met Koper, Kweker, Veilingmeester)
- Address
- Product
- VeilingKlok en VeilingKlokProduct
- Order en OrderItem
- RefreshToken

Relaties (samengevat):
- Kweker -> Products (1:N)
- Koper -> Orders (1:N)
- Order -> OrderItems (1:N)
- VeilingKlok -> VeilingKlokProducts (1:N)
- Veilingmeester -> VeilingKlok (1:N)

## 8. Belangrijkste flows
**Authenticatie**
- Identity endpoints in backend; rollen bepalen toegang tot controllers.
- Frontend bewaart auth data lokaal en stuurt requests via API controllers.

**Product beheer (kweker)**
- Frontend -> API -> MediatR handler -> repository -> database.

**Veiling starten (veilingmeester)**
- Frontend start veiling via API.
- `VeilingKlokEngine` start timer en publiceert prijsupdates via SignalR.

**Bieden (koper)**
- Client ontvangt ticks, plaatst bid via API/SignalR, backend valideert en broadcast updates.

## 9. Configuratie en deployment
- Backend CORS whitelist in `BackendAPI/API/Program.cs`.
- DB connecties via `appsettings.json` (DefaultConnection).
- Database setup instructies in `Instructions_DataBase.md`.
- Swagger alleen in development.

## 10. Tests
- Backend tests in `BackendAPI/Test/**` met unit en controller tests.

## 11. Randvoorwaarden en niet-functionele eisen
- Realtime updates via SignalR zijn kernfunctie.
- Stateful veilingengine draait in geheugen; bij restart worden actieve klokken hersteld.
- SQL Server is vereist voor productie en lokale ontwikkeling.

---
Dit document is gebaseerd op de huidige codebase en kan worden uitgebreid met gedetailleerde ERD of deployment diagrammen indien gewenst.
