
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MolTol.Interfaces;
using MolTol.Models;
using MolTol.Models.ViewModels;

namespace MolTol.Controllers;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly IPriceReportRepository _priceReportRepository;
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IAreaRepository _areaRepository;

    public UserController(
        IPriceReportRepository priceReportRepository,
        IProductRepository productRepository,
        IShopRepository shopRepository,
        IAreaRepository areaRepository)
    {
        _priceReportRepository = priceReportRepository;
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _areaRepository = areaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var model = new UserDashboardViewModel
        {
            TotalReports = await _priceReportRepository.CountByUserIdAsync(
                userId,
                cancellationToken: cancellationToken),
            Areas = await _areaRepository.GetAllAsync(
                cancellationToken: cancellationToken),

            ApprovedReports = await _priceReportRepository.CountByUserIdAsync(
                userId,
                "Approved",
                cancellationToken),

            PendingReports = await _priceReportRepository.CountByUserIdAsync(
                userId,
                "Pending",
                cancellationToken),

            Products = await _productRepository.GetAllAsync(
                activeOnly: true,
                cancellationToken: cancellationToken),

            Shops = await _shopRepository.GetAllAsync(
                status: "Approved",
                cancellationToken: cancellationToken),

            ReportHistory = await _priceReportRepository.GetUserReportHistoryAsync(
                userId,
                cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitPriceReport(
        int productId,
        int shopId,
        decimal price,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { success = false, message = "Please log in again." });
        }

        if (productId <= 0 || shopId <= 0 || price <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please provide a valid product, shop, and price."
            });
        }

        var product = await _productRepository.GetByIdAsync(
            productId,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            return BadRequest(new
            {
                success = false,
                message = "The selected product is unavailable."
            });
        }

        var shop = await _shopRepository.GetByIdAsync(
            shopId,
            cancellationToken);

        if (shop is null || shop.Status != "Approved")
        {
            return BadRequest(new
            {
                success = false,
                message = "The selected shop is unavailable."
            });
        }

        var report = new PriceReport
        {
            ProductId = productId,
            ShopId = shopId,
            UserId = userId,
            Price = price
        };

        await _priceReportRepository.AddAsync(
            report,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message = "Price report submitted successfully. It is pending admin approval."
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuggestShop(
        string shopName,
        int areaId,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Please log in again."
            });
        }

        shopName = shopName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(shopName) || shopName.Length > 150 || areaId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please provide a valid shop name and area."
            });
        }

        var area = await _areaRepository.GetByIdAsync(
            areaId,
            cancellationToken);

        if (area is null || !area.IsActive)
        {
            return BadRequest(new
            {
                success = false,
                message = "The selected area is unavailable."
            });
        }

        var shop = new Shop
        {
            Name = shopName,
            AreaId = areaId,
            Status = "Pending",
            SuggestedByUserId = userId
        };

        await _shopRepository.AddAsync(shop, cancellationToken);

        return Ok(new
        {
            success = true,
            message = "Shop suggestion submitted successfully. It is pending admin approval."
        });
    }
}
