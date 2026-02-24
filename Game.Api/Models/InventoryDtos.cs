using System;
using System.Collections.Generic;

namespace Game.Api.Models
{
    public class InventoryEntryDto
    {
        public string ItemId { get; set; }
        public string InstanceId { get; set; }
        public int Quantity { get; set; }
        public double MassPerUnit { get; set; }
        public double VolumePerUnit { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class ShipDto
    {
        public string ShipId { get; set; }
        public string OwnerId { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public bool Docked { get; set; }
        public int ShieldsCurrent { get; set; }
        public int ShieldsMax { get; set; }
        public int ArmorCurrent { get; set; }
        public int ArmorMax { get; set; }
        public int CapacitorCurrent { get; set; }
        public int CapacitorMax { get; set; }
        public List<InventoryEntryDto> Cargo { get; set; }
        public List<string> Modules { get; set; }
    }

    public class PlayerInventoryDto
    {
        public string PlayerId { get; set; }
        public string Handle { get; set; }
        public long Credits { get; set; }
        public Dictionary<string, int> Reputation { get; set; }
        public List<InventoryEntryDto> Items { get; set; }
        public List<ShipDto> Ships { get; set; }
        public string ActiveShipId { get; set; }
    }
}
