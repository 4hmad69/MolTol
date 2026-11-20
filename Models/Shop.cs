
namespace MolTol.Models;

public class Shop
{
    public int ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
