
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MolTol.Interfaces;
using MolTol.Models.ViewModels;

namespace MolTol.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly IPriceReportRepository _priceReportRepository;
    private readonly IShopRepository _shopRepository;

    public AdminController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IAreaRepository areaRepository,
        IPriceReportRepository priceReportRepository,
        IShopRepository shopRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _areaRepository = areaRepository;
        _priceReportRepository = priceReportRepository;
        _shopRepository = shopRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(
            activeOnly: false,
            cancellationToken: cancellationToken);

        var categories = await _categoryRepository.GetAllAsync(
            activeOnly: false,
            cancellationToken: cancellationToken);

        var areas = await _areaRepository.GetAllAsync(
            activeOnly: false,
            cancellationToken: cancellationToken);

        var model = new AdminDashboardViewModel
        {
            TotalProducts = products.Count,
            TotalCategories = categories.Count,
            TotalAreas = areas.Count,
            TotalReports = await _priceReportRepository.CountAsync(
                cancellationToken: cancellationToken),
            PendingReports = await _priceReportRepository.CountAsync(
                "Pending",
                cancellationToken),
            PendingPriceReports = await _priceReportRepository.GetAllAsync(
                "Pending",
                cancellationToken),
            PendingShops = await _shopRepository.GetAllAsync(
                status: "Pending",
                cancellationToken: cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePriceReportStatus(
        int reportId,
        string status,
        CancellationToken cancellationToken)
    {
        if (reportId <= 0 || status is not ("Approved" or "Rejected"))
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid report information."
            });
        }

        var updated = await _priceReportRepository.UpdateStatusAsync(
            reportId,
            status,
            cancellationToken);

        if (!updated)
        {
            return NotFound(new
            {
                success = false,
                message = "Report not found or already reviewed."
            });
        }

        return Json(new
        {
            success = true,
            message = $"Price report {status.ToLower()} successfully."
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShopStatus(
        int shopId,
        string status,
        CancellationToken cancellationToken)
    {
        if (shopId <= 0 || status is not ("Approved" or "Rejected"))
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid shop information."
            });
        }

        var updated = await _shopRepository.UpdateStatusAsync(
            shopId,
            status,
            cancellationToken);

        if (!updated)
        {
            return NotFound(new
            {
                success = false,
                message = "Shop not found or already reviewed."
            });
        }

        return Json(new
        {
            success = true,
            message = $"Shop {status.ToLower()} successfully."
        });
    }
}
