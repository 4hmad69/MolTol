
using MolTol.Models;

namespace MolTol.Models.ViewModels;

public class UserDashboardViewModel
{
    public int TotalReports { get; set; }
    public int ApprovedReports { get; set; }
    public int PendingReports { get; set; }
    public IReadOnlyList<Area> Areas { get; set; } = [];
    public IReadOnlyList<Product> Products { get; set; } = [];
    public IReadOnlyList<Shop> Shops { get; set; } = [];
    public IReadOnlyList<UserPriceReportItem> ReportHistory { get; set; } = [];
}
