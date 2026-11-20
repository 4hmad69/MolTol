
namespace MolTol.Models;

public class PriceReport
{
    public int ReportId { get; set; }
    public int ProductId { get; set; }
    public int ShopId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReportDate { get; set; }
    public string Status { get; set; } = "Pending";
}
