using System.ComponentModel.DataAnnotations;

namespace WEBSAIGONGLISTEN.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public DateTime DepartureDate { get; set; } // Ngày khởi hành
        public DateTime ReturnDate { get; set; } // Ngày kết thúc
        public string Destination { get; set; } // Điểm đến
        public string Itinerary { get; set; } // Lộ trình
        public string Inclusions { get; set; } // Các dịch vụ bao gồm
        public string Exclusions { get; set; } // Các dịch vụ không bao gồm
        public string Notes { get; set; } // Ghi chú
        public int MaximumCapacity { get; set; } // Số lượng người tối đa cho mỗi tour
        public int RemainingCapacity { get; set; } // Số lượng người còn trống
    }
}
