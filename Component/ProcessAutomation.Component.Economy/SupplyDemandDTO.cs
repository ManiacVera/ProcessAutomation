namespace ProcessAutomation.Component
{
    public class SupplyDemandDTO
    {
        public long Sequence { get; set; }
        public string RelationalKey { get; set; }
        public string DemandKey { get; set; }
        public decimal DemandQuantity { get; set; }
        public decimal DemandRemainingQuantity { get; set; }
        public string SupplyKey { get; set; }
        public decimal SupplyQuantity { get; set; }
        public decimal SupplyRemainingQuantity { get; set; }
        public decimal AssignedQuantity { get; set; }
    }
}
