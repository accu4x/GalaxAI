GalaxAI - repo README

(Updated: added integration notes and architecture summaries)

See docs/INTEGRATION.md for Azure Tables configuration and schema details.

Agent skill data lives in Game.NpcSkill/Data and includes:
- AgentGuidelines.md
- Lore.md
- ScreenTemplates.md
- Items/items.json

API project: Game.Api
- Background services: MarketTickService, NewsFeedService
- Storage: Azure Table Storage (see docs/INTEGRATION.md)

To run locally:
- set AZURE_TABLES_CONNECTION_STRING to use Azure Storage emulator or Azure Storage account
- dotnet restore && dotnet build
- Note: add NuGet package Azure.Data.Tables
