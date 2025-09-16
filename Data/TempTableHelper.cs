using be_devextreme_starter.Data.Models;
using Newtonsoft.Json;

namespace be_devextreme_starter.Data
{
    public class TempTableHelper<T>
    {
        private DataEntities db;
        public TempTableHelper(DataEntities db)
        {
            this.db = db;
        }

        public T GetContentAsObject(Guid id)
        {
            var obj = db.FW_Temp_Tables.Find(id);
            if (obj != null)
            {
                var returnObj = JsonConvert.DeserializeObject<T>(obj.temp_content);
                return returnObj;
            }
            else
            {
                return default(T);
            }
        }

        public int? UpdateContent(Guid id, T obj)
        {
            FW_Temp_Table ent = db.FW_Temp_Tables.Find(id);
            if (ent != null)
            {
                string content = JsonConvert.SerializeObject(obj);
                ent.temp_content = content;
                ent.date_modified = DateTime.Now;
                return db.SaveChanges();
            }
            return null;
        }

        public Guid CreateContent(T obj)
        {
            string content = JsonConvert.SerializeObject(obj);

            FW_Temp_Table ent = new FW_Temp_Table();
            ent.temp_content = content;
            ent.temp_id = Guid.NewGuid();
            if (db.HttpContext != null)
            {
                if (db.HttpContext.User != null)
                {
                    ent.user_id = db.HttpContext.User.Identity.Name;
                } else if (db.HttpContext.Request != null)
                {
                    ent.user_id = db.HttpContext.Request.Host.Host;
                }
            } else
            {
                ent.user_id = "test";
            }
            ent.date_created = DateTime.Now;
            db.FW_Temp_Tables.Add(ent);
            db.SaveChanges();
            return ent.temp_id;
        }

    }
}
