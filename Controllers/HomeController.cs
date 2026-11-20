
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MolTol.Interfaces;
using MolTol.Models;
using MolTol.Models.ViewModels;

namespace MolTol.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly IPriceReportRepository _priceReportRepository;

    public HomeController(
        ILogger<HomeController> logger,
        ICategoryRepository categoryRepository,
        IAreaRepository areaRepository,
        IPriceReportRepository priceReportRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _areaRepository = areaRepository;
        _priceReportRepository = priceReportRepository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new HomePageViewModel
        {
            Categories = await _categoryRepository.GetAllAsync(
                cancellationToken: cancellationToken),

            Areas = await _areaRepository.GetAllAsync(
                cancellationToken: cancellationToken)
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetPriceComparisons(
        int? categoryId,
        int? areaId,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var results = await _priceReportRepository.GetApprovedComparisonsAsync(
            categoryId,
            areaId,
            searchTerm,
            cancellationToken);

        return Json(results);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
