using System;
using System.Collections.Generic;

namespace Game.Api.Models
{
    public class MarketPriceEntryDto
    {
        public string ItemId { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class MarketSnapshotDto
    {
        public string SystemId { get; set; }
        public List<MarketPriceEntryDto> Prices { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class MarketTradeRequestDto
    {
        public string PlayerId { get; set; }
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsBuying { get; set; }
    }

    public class MarketTradeResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal TotalCost { get; set; }
        public List<MarketPriceEntryDto> UpdatedPrices { get; set; }
    }
}
