namespace be_devextreme_starter.DTOs
{
    public class RoleDto
    {
        public string roleId { get; set; }
        public string roleCatatan { get; set; }
        public List<string> modKodes { get; set; } = new List<string>();
    }

    public class UserRoleDto
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string? userEmail { get; set; }
        public List<string> roles { get; set; } = new List<string>();
    }
}
