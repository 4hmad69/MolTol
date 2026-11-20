
namespace MolTol.Models;

public class UserPriceReportItem
{
    public int ReportId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
