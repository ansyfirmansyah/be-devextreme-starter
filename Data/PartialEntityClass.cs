using Microsoft.AspNetCore.Http;

namespace be_devextreme_starter.Data.Models
{
    public partial class DataEntities
    {
        public readonly Microsoft.AspNetCore.Http.HttpContext httpContext;
        public readonly IWebHostEnvironment env;
        public readonly AppSettings appSettings;
        public DataEntities(AppSettings appSettings, IHttpContextAccessor httpContext, IWebHostEnvironment env)
            : base()
        {
            this.httpContext = httpContext.HttpContext;
            this.env = env;
            this.appSettings = appSettings;
        }
        public DataEntities(string connString)
           : base()
        {

        }

        public Microsoft.AspNetCore.Http.HttpContext HttpContext
        {
            get
            {
                return httpContext;
            }
        }

        public void SetStsrcFields(dynamic obj)
        {
            dynamic obj2 = obj;
            if (object.ReferenceEquals(obj2.created_by, System.Convert.DBNull) || obj2.created_by == null)
            {
                obj2.stsrc = "A";
                if (httpContext == null || httpContext.User.Identity.Name == null)
                {
                    obj2.created_by = "System";
                }
                else
                {
                    obj2.created_by = httpContext.User.Identity.Name;
                }
                obj2.date_created = System.DateTime.Now;
            }
            else
            {
                if (httpContext == null || httpContext.User.Identity.Name == null)
                {
                    obj2.modified_by = "System";

                }
                else
                {
                    obj2.modified_by = httpContext.User.Identity.Name;
                }
                obj2.date_modified = System.DateTime.Now;
            }
        }

        public void DeleteStsrc(dynamic obj)
        {
            dynamic obj2 = obj;
            obj2.stsrc = "D";
            if (httpContext == null || httpContext.User.Identity.Name == null)
            {
                obj2.modified_by = "system";
            }
            else
            {
                obj2.modified_by = httpContext.User.Identity.Name;
            }
            obj2.date_modified = System.DateTime.Now;
        }
    }
}
