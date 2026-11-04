using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class EconomyDTO : IComponentDTO
    {
        public string EconomyType { get; set; }
        public string DemandSourceBy { get; set; }
        public string DemandTable { get; set; }
        public string DemandQuery { get; set; }
        public string DemandRelationalField { get; set; }
        public string DemandKeyField { get; set; }
        public string DemandQuantityField { get; set; }
        public string DemandOrderBy { get; set; }
        public string SupplySourceBy { get; set; }
        public string SupplyTable { get; set; }
        public string SupplyQuery { get; set; }
        public string SupplyRelationalField { get; set; }
        public string SupplyKeyField { get; set; }
        public string SupplyQuantityField { get; set; }
        public string SupplyOrderBy { get; set; }
        public string DecimalOption { get; set; }
        public int DecimalQuantity { get; set; }
        public int NumberPeriods { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
    }
}
