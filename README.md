# TechStock
TechStock är en webbapplikation skapad för att hantera produkter och lagersaldo i en fiktiv teknikbutik. Applikationen är byggd med **ASP.NET Core MVC** som backend-ramverk/frontend-rendering och använder **Entity Framework Core** för databasinteraktion. Frontend-delen är byggd med **Razor Views (MVC)** och **Bootstrap** för styling.

Projektet är utvecklat som en del i kursen DT191G Webbutveckling med .NET, Mittuniversitetet.

## Funktioner
- CRUD-funktionalitet för produkter (skapa, läsa, uppdatera, ta bort).
- Hantering av kategorier och varumärken.
- Filtrering och sortering av produkter.
- Paginering av produktlistor.
- Inloggning och utloggning med ASP.NET Identity.
- Responsivt användargränssnitt anpassat för både stora och små enheter.
- PWA för nedladdning på lokal enhet.
- REST API (GET) för hämtning av produkter.

## Teknologier
- **ASP.NET Core MVC**  för backend-logik och frontend-rendering.
- **Entity Framework Core** för databasinteraktion.
- **SQLite** för databaslagring.
- **Razor Views (MVC)** för frontend-logik.
- **Bootstrap** för styling och användargränssnitt.
- **X.PagedList** för paginering.
- **Azure** för publicering.

## Installation och körning
### Förutsättningar
- .NET SDK (version 6 eller senare)
- SQL Server eller SQLite installerat
- Git installerat
- Visual Studio eller VS Code (valfritt)

### Installation
1. Klona detta repo:
   ```bash
   git clone https://github.com/ditt-repo/TechStock.git
   cd TechStock
   ```

2. Installera nödvändiga paket:
   ```bash
   dotnet restore
   ```

3. Skapa och konfigurera databasen:
   - Uppdatera `appsettings.json` med din databasanslutning i `ConnectionStrings`
   - Kör följande kommando för att migrera databasen:
     ```bash
     dotnet ef database update
     ```

4. Starta applikationen:
   ```bash
   dotnet run
   ```

Applikationen är nu igång och tillgänglig på `https://localhost:5001` eller `http://localhost:5000` beroende på din konfiguration.

## Funktionalitet och sidor
### Huvudfunktioner
- **Produkter:**
  - Visa en lista över alla produkter med information som namn, beskrivning, pris, lagerstatus, kategori och varumärke, både på publik sida och admin-sida.
  - Visa detaljer kring specifika produkter, både på publik sida och admin-sida.
  - Lägga till, redigera eller ta bort produkter om användaren är inloggad.

- **Kategorier:**
  - Visa en lista över alla kategorier.
  - Visa detaljer kring en kategori samt dess tillhörande produkter.
  - Lägg till, redigera eller ta bort kategorier.

- **Varumärken:**
  - Visa en lista över alla varumärken.
  - Visa detaljer kring ett varumärke samt dess tillhörande produkter.
  - Lägg till, redigera eller ta bort varumärken.

- **Användarhantering:**
  - Registrera konto, logga in och logga ut via ASP.NET Identity.

- **API för produkter:**
  - Visar data via /api/produkter (GET)

### Navigering
- **"/"** och **"/hem"**: Publik startsida som visar alla produkter i produktkort och detaljer för produkterna vid klick på respektive produkt.
- **"/produkter"**: Admin-sida för att visa, lägga till, redigera och ta bort produkter (skyddad sida, kräver inloggning).
- **"/kategorier"**: Admin-sida för att visa, lägga till, redigera och ta bort kategorier (skyddad sida, kräver inloggning).
- **"/varumarken"**: Admin-sida för att visa, lägga till, redigera och ta bort varumärken (skyddad sida, kräver inloggning).
- **"/identity/account/login"**: Inloggningssida via Identity.
- **"/identity/account/register"**: Registreringssida via Identity.

## API
Applikationen erbjuder ett REST API för att hämta produkter via endpoint `/api/produkter`. Exempel på svar från API-anrop:

```json
[
  {
    "id": 1,
    "articleNumber": "TS-2025-3482a",
    "name": "Spacer 5000 X3",
    "description": "Högpresterande bärbar dator",
    "price": 12999.0,
    "weight": 1579,
    "stockQuantity": 58,
    "imageName": null,
    "categoryId": 1,
    "categoryName": "Dator",
    "brandId": 1,
    "brandName": "Spacer",
    "createdAt": "2025-02-22T12:08:08.2554388",
    "updatedAt": "2025-02-22T12:15:02.8592066",
    "stockStatus": "I lager"
  }
]
```

## Publicering
Applikationen finns publicerad på **Azure**.

Länk till applikationen: https://iswe2301-techstock.azurewebsites.net

## Av
- **Namn:** Isa Westling
- **E-post:** iswe2301@student.miun.se
- **Kurs:** DT191G Webbutveckling med .NET
- **År:** 2025
- **Skola:** Mittuniversitetet