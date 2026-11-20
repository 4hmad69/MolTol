using MolTol.Models;
namespace MolTol.Models.ViewModels;

public class HomePageViewModel
{
    public IReadOnlyList<Category> Categories { get; set; } = [];
    public IReadOnlyList<Area> Areas { get; set; } = [];
}
