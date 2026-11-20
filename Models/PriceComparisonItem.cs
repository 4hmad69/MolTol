
namespace MolTol.Models;
public class PriceComparisonItem
{
    public int ReportId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReportDate { get; set; }
}
