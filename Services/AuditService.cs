using System.Security.Claims;

namespace be_devextreme_starter.Services
{
    public interface IAuditService
    {
        void SetStsrcFields(dynamic obj);
        void DeleteStsrc(dynamic obj);
    }
    public class AuditService : IAuditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetStsrcFields(dynamic obj)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (obj.GetType().GetProperty("created_by") != null && (obj.created_by == null || string.IsNullOrEmpty(obj.created_by.ToString())))
            {
                obj.stsrc = "A";
                obj.created_by = string.IsNullOrEmpty(userId) ? "System" : userId;
                obj.date_created = DateTime.Now;
            }
            else
            {
                obj.modified_by = string.IsNullOrEmpty(userId) ? "System" : userId;
                obj.date_modified = DateTime.Now;
            }
        }

        public void DeleteStsrc(dynamic obj)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            obj.stsrc = "D";
            obj.modified_by = string.IsNullOrEmpty(userId) ? "System" : userId;
            obj.date_modified = DateTime.Now;
        }
    }
}
