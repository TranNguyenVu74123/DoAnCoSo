using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WEBSAIGONGLISTEN.Models;
using WEBSAIGONGLISTEN.Repositories;

namespace WEBSAIGONGLISTEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Company)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;// day la hung
        private readonly int _pageSize = 10;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
        {
            _context = context; // cho nay la hung
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        // Hiển thị danh sách sản phẩm
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
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile
        imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage

                    product.ImageUrl = await SaveImage(imageUrl);

                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); //Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        //Nhớ tạo folder images trong wwwroot

        // Hiển thị thông tin chi tiết sản phẩm
        // Route: /admin/display/{id}
        [HttpGet("product/display/{id}")]
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Hiển thị form cập nhật sản phẩm
        // Route: /admin/update/{id}
        [HttpGet("product/update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",
            product.CategoryId);
            return View(product);
        }
        // Xử lý cập nhật sản phẩm
        // Route: /admin/update/{id} (POST)
        [HttpPost("product/update/{id}")]
      /*  [HttpPost]*/
        public async Task<IActionResult> Update(int id, Product product,
        IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await
                _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới

                    product.ImageUrl = await SaveImage(imageUrl);

                }
                // Cập nhật các thông tin khác của sản phẩm

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        // Route: /admin/delete/{id}
        [HttpGet("product/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query, string searchType)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Trả về một View trống hoặc thông báo không tìm thấy.
                return View("Search", new List<Product>());
            }

            if (searchType == "product")
            {
                // Chuyển đổi query để tìm kiếm các sản phẩm chứa các từ được phân tách bởi khoảng trắng
                var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var products = await _context.Products
                    .Where(p => keywords.Any(keyword => p.Name.Contains(keyword)))
                    .ToListAsync();

                return View("Search", products);
            }
            else if (searchType == "category")
            {
                // Chuyển đổi query để tìm kiếm các sản phẩm chứa các từ được phân tách bởi khoảng trắng
                var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var categories = await _context.Categories
                    .Where(c => keywords.Any(keyword => c.Name.Contains(keyword)))
                    .ToListAsync();

                return View("SearchCategory", categories);
            }
            else if (searchType == "user")
            {
                // Chuyển đổi query để tìm kiếm các sản phẩm chứa các từ được phân tách bởi khoảng trắng
                var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var users = _userManager.Users
                    .Where(u => keywords.Any(keyword => u.UserName.Contains(keyword) || u.Email.Contains(keyword)))
                    .OrderBy(u => u.UserName);

                var searchResults = await users.ToListAsync();

                return View("SearchUser", searchResults);
            }

            return NotFound(); // Trong trường hợp searchType không hợp lệ
        }




    }
}