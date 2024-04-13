using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WEBSAIGONGLISTEN.Models;
using WEBSAIGONGLISTEN.Repositories;

namespace WEBSAIGONGLISTEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Company)]
    public class ReportController : Controller
    { 
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(IProductRepository productRepository, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Logic để hiển thị tổng người dùng, tổng doanh thu và tổng số tour
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Summary(DateTime? date)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            var startDate = new DateTime(date.Value.Year, date.Value.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var totalUsers = await _userManager.Users.CountAsync();
            var orders = await _orderRepository.GetOrdersByDateAsync(startDate, endDate);
            var totalRevenue = orders.Sum(o => o.TotalPrice);
            var totalTours = await _productRepository.GetAllAsync();

            var summaryViewModel = new SummaryViewModel
            {
                TotalUsers = totalUsers,
                TotalRevenue = totalRevenue,
                TotalTours = totalTours.Count()
            };

            return View(summaryViewModel);
        }
    }
}
