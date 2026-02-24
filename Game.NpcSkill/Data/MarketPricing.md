Market pricing and DTOs

Purpose
- Describe the daily price generation model, API DTO scaffolding for prices, and how player actions affect supply/demand.

Key concepts
- basePrice: intrinsic value of an item (catalog-level)
- supply: current available quantity in the market (numeric)
- demand: current demand factor (numeric)
- modifier: computed price modifier based on supply/demand and random shock
- finalPrice = basePrice * modifier

DTOs (JSON schema-like)

MarketPriceEntry
- itemId: string
- systemId: string
- bodyId: string (optional)  # planet/moon id
- basePrice: number
- supply: number
- demand: number
- modifier: number
- finalPrice: number
- lastUpdated: ISODate

MarketSnapshot
- systemId: string
- date: ISODate
- prices: [ MarketPriceEntry ]

API endpoints (suggested)
- GET /market/{systemId}/snapshot  => MarketSnapshot
- POST /market/{systemId}/transaction  { playerId, itemId, qty, buySell } => validates and returns updated MarketPriceEntry and player inventory change
- POST /market/{systemId}/adjust  { itemId, deltaSupply }  => internal use to record shipments / big events

Daily price generation algorithm (server-side)
1) Start from basePrice and previous day's supply/demand.
2) Apply natural drift: supply *= (1 + driftSupply), demand *= (1 + driftDemand) where drift ~ Normal(0, sigma)
3) Apply player-driven changes: for each logged transaction today, supply -= qtySold, demand += qtyBought
4) Apply event shocks: random chance of local events (e.g., asteroid strike, raid, festival) that modify supply/demand significantly.
5) Compute modifier:
   - priceElasticity = k1 * (1 / (supply + epsilon)) + k2 * demand
   - randomShock = 1 + Normal(0, sigma_shock)
   - modifier = clamp(1 + priceElasticity + randomShock, min=0.1, max=10)
6) finalPrice = basePrice * modifier
7) Store MarketSnapshot for the day; archive history for analytics.

Supply/demand impacts from player actions
- Buying reduces supply and increases local demand -> price rises.
- Selling increases supply and lowers price.
- Repeated shipments into a planet cause supply to spike and can crash prices; repeated raids lower supply and increase price.

Deterministic seeding for generation
- Use server RNG seeded from server date + systemId so daily prices are reproducible for that date given server state and transactions.

Implementation notes
- Keep historical granularity at daily snapshots for performance; keep intra-day transactions aggregated for the day's snapshot.
- For testing/demo, a simple simulation will demonstrate price swings based on scripted transactions.

Where to store configuration
- Game.Api/config/market.yaml — basePrice overrides and per-system modifiers
- Game.NpcSkill/Data/Market/ — sample snapshots and initial market state for core systems

Next steps I can do for you
- Create Game.NpcSkill/Data/Market/initial_snapshot.json for the core systems using default basePrices from items.json (if you ask me to produce items.json now)
- Add API DTO scaffolding in Game.Api/Models/Market*.cs
- Add a simple simulation script that accepts a sequence of transactions and prints the resulting price snapshots.