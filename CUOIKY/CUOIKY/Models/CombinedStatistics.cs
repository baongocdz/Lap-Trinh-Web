namespace CUOIKY.Models
{
    public class CombinedStatistics
    {
        public string Period { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTopUps { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
