namespace ProcessAutomation.Component
{
    public class AmortizationDTO
    {
        public int Period { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Interest { get; set; }
        public decimal Capital { get; set; }
        public decimal Fee { get; set; }
        public decimal RemainingCapital { get; set; }
    }
}
