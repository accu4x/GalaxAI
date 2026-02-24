# Master Data — Example Gameplay Workflow

Saved example workflow (user-specified) — use this as a starting point for game-state design, quests, and session flow.

## Example: Mining Mission (high-level flow)

1. Game start / wake-up
   - Player wakes up in their ship at the last docked station or location.
   - Initial UI options: newsfeed, undock, inventory, messages, star chart, market, message NPC, etc.

2. Check Newsfeed
   - Player views the newsfeed and sees mission postings (e.g., mining mission).
   - Player can accept a mission from the newsfeed.

3. Undock and Travel
   - Player undocks from station and navigates to destination (asteroid belt for mining)
   - Travel may consume fuel/time and may present navigation choices.

4. Encounter: Pirates
   - While in the asteroid belt, hostile pirates appear.
   - Combat subroutine is triggered.
   - Player can choose actions (fight, flee, call for help, use countermeasures).

5. Flee and Return
   - Player chooses to flee and returns to the nearest station.
   - On return, mission state is updated (failed, partial, or completed depending on rules).

6. Upload Captain's Log
   - The ship automatically or manually uploads a captain's log describing the incident.
   - The log is stored in player history/messages and may influence future missions or reputation.

7. Log Off / End Session
   - Player can save/quit and log off; session state persists for next login.


## Notes / Implementation ideas
- Session model: one active session per player/chat; snapshot-driven state passed into the NPC/skill.
- Actions are driven by menu choices and free-text NPC interactions.
- Important game events (mission accept, combat outcomes) should POST to the Game.Api to update persistent state.
- Use master-data.md as the canonical place for walkthroughs, quest outlines, and flow diagrams.


*Saved: 2026-02-23*