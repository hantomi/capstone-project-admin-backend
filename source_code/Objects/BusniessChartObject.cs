namespace DeliverBox_BE.Objects
{
    public class BusinessChartObject
    {
        public BusinessChartObject(int? id, string? businessName,
            int? busniessAmount)
        {
            this.id = id;
            this.businessName = businessName;
            this.businessAmount = busniessAmount;
        }
        public int? id { get; set; }
        public string? businessName { get; set; }
        public int? businessAmount { get; set; }
    }
}