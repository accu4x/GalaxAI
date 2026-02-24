GalaxAI Integration Guide - Azure Tables

Overview
- This guide explains the Azure Table Storage schema used by Game.Api, table names, partition/rowkey choices, and required environment variables.

Environment
- AZURE_TABLES_CONNECTION_STRING must be set for the Game.Api to talk to Azure Tables.

Tables & Entities
- Players (table name: Players)
  - PartitionKey: "PLAYER"
  - RowKey: playerId (string)
  - Stores basic fields and JSON blobs for profile, but skills/inventory are stored in their own tables.

- Skills (table name: Skills)
  - PartitionKey: "PLAYER_SKILL"
  - RowKey: "{playerId}:{skillId}"
  - SkillJson: JSON representation of PlayerSkillDto

- Inventories (table name: Inventories)
  - PartitionKey: "PLAYER_INV"
  - RowKey: "{playerId}:{inventoryId}" (inventoryId = PERSONAL or SHIP:{shipId})
  - InventoryJson: JSON array of inventory entries

- Ships (table name: Ships)
  - PartitionKey: "SHIP"
  - RowKey: shipId
  - ShipJson: JSON blob with ship state

- Market entries (table name: Market_{systemId})
  - PartitionKey: "MARKET"
  - RowKey: itemId
  - Stores price, supply, demand and last update

- SystemEvents (table name: SystemEvents)
  - PartitionKey: "SYSTEM_EVENT"
  - RowKey: "{systemId}:{eventId}" or just eventId
  - EventJson: JSON payload for the event (title, summary, mappedFaction, published)

Configuration
- MarketTickOptions in appsettings or env variables
  - RealMinutesPerGameDay (default 60)
  - DevMultiplier (default 1.0)
  - PriceRandomness (default 0.05)

Game Time
- GameTimeService exposes NowInGameUtc() and NowInGameString() in format yyyy.MM.dd.HH
- Acceleration mapping: by default 1 real hour = 1 game day

Notes
- Entities use JSON blobs for arrays/complex objects to keep table schema manageable.
- For heavy query workloads consider moving frequently queried data to dedicated tables or Cosmos DB.

TODOs
- Add sample scripts to seed core systems and initial market snapshots.
- Add guidance for partitioning strategies at scale.
