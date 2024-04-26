using Microsoft.AspNetCore.Mvc.Rendering;

namespace WEBSAIGONGLISTEN.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; } // ID của người dùng cần thay đổi vai trò

        public string CurrentRole { get; set; } // Vai trò hiện tại của người dùng

        public string SelectedRole { get; set; } // Vai trò mới được chọn

        public List<SelectListItem> Roles { get; set; } // Danh sách các vai trò để chọn
    }
}
