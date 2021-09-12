namespace TemperatureAlert.Domain
{
    public class TemperatureRule
    {
        public decimal MinTemperature { get; set; }
        public decimal MaxTemperature { get; set; }
        public int MaximumMinutes { get; set; }
        public int MaximumNumberOfAnomalies { get; set; }
    }
}
