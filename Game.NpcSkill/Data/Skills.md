Skills System

Overview

Skills govern player proficiency in repeatable actions (weapons, piloting, mining, etc.). Each skill has four tiers: Basic, Advanced, Expert, Specialist. Progress within a tier is measured 0–100% and players earn XP by performing relevant actions. When tier progress reaches 100% a neural chip item must be installed to promote to the next tier.

Tiers and effectiveness

- Basic
- Advanced
- Expert
- Specialist

Effectiveness mapping (displayed to players)
- Basic: 0%–30%
- Advanced: 31%–50%
- Expert: 51%–60%
- Specialist: 61%–65%

Mastery

- Reaching 100% in Specialist grants Mastery. Mastery unlocks highest-tier modules and adds a flat +15% effectiveness bonus on top of Specialist effectiveness.
- Mastery is flagged on the PlayerSkill entry.

Progression mechanics

- BaseTierXP: 1000 (configurable)
- Required XP per tier = BaseTierXP * tierMultiplier where tierMultiplier maps: Basic=1, Advanced=2, Expert=3, Specialist=4. This implements diminishing returns in higher tiers.
- XP per action is scaled by 1 / tierMultiplier so higher tiers require more actions to fill.

Specialties

- At Specialist tier the player can choose a specialty (usually 1 of up to 4 options; e.g., weapon types: Beam, Kinetic, Missiles, Pulse).
- Specialty grants access to specialist modules and an additional performance bonus when using modules of that specialty.
- Specialty selection is reversible at a cost: performing a specialty change consumes a special neural chip (neural_chip_specialty_change) and a fee; the server validates and processes the swap.

Neural Chips

- Neural chips are items in the item catalog. They are consumed when used to promote tiers or change specialties.
- Naming convention: neural_chip_{skill}_tier2 (Basic->Advanced), neural_chip_{skill}_tier3 (Advanced->Expert), neural_chip_{skill}_tier4 (Expert->Specialist). There is also neural_chip_specialty_change for specialty swaps.

Skills to include (initial)
- weapons
- defenses
- piloting
- mining
- industry
- negotiation
- scanning
- engineering

API actions
- GET /player/{playerId}/skills
- POST /player/{playerId}/skill/{skillId}/use  { actionContext }
- POST /player/{playerId}/skill/{skillId}/install-chip  { chipItemId }
- POST /player/{playerId}/skill/{skillId}/choose-specialty  { specialtyId }

Data model notes (human-readable)
- PlayerSkill: { skillId, tier, tierProgress, totalXP, specialty, mastery }
- SkillDefinition: { skillId, name, description, allowedSpecialties }

Design notes
- Agents should display tierProgress and computed effectiveness in UI screens.
- When suggesting modules or actions, agents consult the PlayerSkill to show bonuses/unlocks.
- Neural chips are rare/expensive items; progression speed and chip availability are tunable for balance.
