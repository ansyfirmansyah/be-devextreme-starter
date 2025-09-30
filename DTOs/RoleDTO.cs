namespace be_devextreme_starter.DTOs
{
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string RoleCatatan { get; set; }
        public List<string> ModKodes { get; set; } = new List<string>();
    }
}
