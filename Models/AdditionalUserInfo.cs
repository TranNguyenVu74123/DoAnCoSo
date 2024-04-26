using System.ComponentModel.DataAnnotations;

namespace WEBSAIGONGLISTEN.Models
{
    public class AdditionalUserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ProviderKey { get; set; }

        [Required]
        public string ProviderDisplayName { get; set; }
    }
}
