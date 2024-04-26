using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WEBSAIGONGLISTEN.Models;

namespace WEBSAIGONGLISTEN.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;// day la hung
        private readonly int _pageSize = 8;
        public HomeController(IProductRepository productRepository, ApplicationDbContext context) // cho nay la hung
        {
            _context = context; // cho nay la hung
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var allProducts = await _productRepository.GetAllAsync();

            int totalProducts = allProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / _pageSize);

            var paginatedProducts = allProducts.Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            return View(paginatedProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Tour()
        {
            var tours = _context.Products.ToList(); // Lấy danh sách tour từ cơ sở dữ liệu
            return View(tours);
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int? productId, int? quantity)
        {
            if (productId == null)
            {
                return NotFound();
            }
            var product = await _productRepository.GetByIdAsync(productId.Value);
            if (product == null)
            {
                return NotFound();
            }
            // Xử lý thêm cho quantity nếu cần
            return View(product);
        }

        public async Task<IActionResult> Search(string query)
        {
            // Loại bỏ các khoảng trắng không cần thiết từ query
            query = query.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                // Trả về một View trống hoặc thông báo không tìm thấy.
                return View("Search", new List<Product>());
            }

            // Chuyển đổi query để tìm kiếm các sản phẩm chứa các từ được phân tách bởi khoảng trắng
            var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var result = await _context.Products
                .Where(p => keywords.Any(keyword => p.Name.Contains(keyword)))
                .ToListAsync();

            // Trả về kết quả tìm kiếm qua View "Search" hoặc một View khác mà bạn muốn hiển thị kết quả
            return View("Search", result);
        }
    }
}