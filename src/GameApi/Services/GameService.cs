using GameApi.Models;

namespace GameApi.Services
{
    public class GameService
    {
        private readonly Storage.ITableStore _store;
        private readonly KeyService _keys;
        public GameService(Storage.ITableStore store, KeyService keys) { _store = store; _keys = keys; }

        public async Task<PlayerState> GetPlayerStateAsync(string playerId)
        {
            var ent = await _store.GetPlayerAsync(playerId);
            if (ent == null) return null;
            return new PlayerState { PlayerId = ent.RowKey, Location = ent.Location, HP = ent.HP, Inventory = ent.Inventory?.ToList() ?? new List<string>(), AvailableActions = ent.AvailableActions?.ToList() ?? new List<string>(), ETag = ent.ETag };
        }

        public async Task<Models.ActionResult> ApplyActionAsync(string playerId, Models.ActionRequest req)
        {
            // Basic placeholder: validate, apply simple transformations, persist via store
            var ent = await _store.GetPlayerAsync(playerId);
            if (ent == null) return new Models.ActionResult { Success = false, Message = "Player not found" };

            // Example: if action is "wait" regain 1 HP
            if (req.ActionId == "wait")
            {
                ent.HP += 1;
                await _store.UpsertPlayerAsync(ent);
                return new Models.ActionResult { Success = true, Message = "You wait and catch your breath.", NewState = new PlayerState { PlayerId = ent.RowKey, Location = ent.Location, HP = ent.HP, Inventory = ent.Inventory, AvailableActions = ent.AvailableActions } };
            }

            return new Models.ActionResult { Success = false, Message = "Unknown action" };
        }
    }
}
