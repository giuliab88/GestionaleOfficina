# Gestionale Officina

Questo progetto è un gestionale per un'officina meccanica sviluppato come esercizio full-stack.
Permette di gestire le principali attività dell'officina tramite API REST e interfaccia web.

Il backend è realizzato in .NET 8, mentre il frontend è sviluppato con Blazor Server.

## Funzionalità principali

- Gestione clienti
- Gestione veicoli associati ai clienti
- Gestione interventi
- Creazione preventivi
- Creazione fatture
- Autenticazione tramite JWT

## Tecnologie utilizzate

- ASP.NET Core (.NET 8)
- Blazor Server
- Entity Framework Core
- SQL Server (LocalDB)
- JWT per autenticazione

## Prerequisiti

- .NET 8 SDK
- SQL Server LocalDB (incluso con Visual Studio)

## Avvio in sviluppo

1. Clonare il repository
2. Aprire la soluzione `OfficinaGestionale.Api.sln` in Visual Studio
3. Impostare come progetti di avvio:
   - OfficinaGestionale.Api
   - OfficinaGestionale.Blazor
4. Avviare con F5

Al primo avvio il database viene creato automaticamente e vengono applicate le migrazioni.

## Configurazione

La chiave JWT (`Jwt:Key`) è definita in `appsettings.Development.json` per l'ambiente di sviluppo.

In produzione è consigliato impostarla tramite variabili d'ambiente o strumenti di gestione dei segreti, senza inserirla direttamente nei file del progetto.

## Credenziali di accesso (solo per sviluppo)

| Campo    | Valore            |
|----------|-------------------|
| Email    | admin@officina.it |
| Password | Admin123!         |

Queste credenziali vengono create automaticamente all'avvio e sono pensate solo per test locali.

## Struttura del progetto

```
OfficinaGestionale.Api/
  Controllers/   — endpoint REST
  Services/      — logica di business
  Repositories/  — accesso al database (EF Core)
  Models/        — entità del dominio
  DTOs/          — oggetti di trasferimento dati
  Migrations/    — migrazioni EF Core

OfficinaGestionale.Blazor/
  Components/Pages/  — pagine principali (Clienti, Veicoli, Interventi, Preventivi, Fatture)
  Services/          — gestione chiamate API e autenticazione
```
