
using MolTol.Models;

namespace MolTol.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalAreas { get; set; }
    public int TotalReports { get; set; }
    public int PendingReports { get; set; }

    public IReadOnlyList<PriceReport> PendingPriceReports { get; set; } = [];
    public IReadOnlyList<Shop> PendingShops { get; set; } = [];
}
