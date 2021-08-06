using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class BCodeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<BCodeModel> models = new List<BCodeModel>();
                var list = entity.b_code.OrderBy(o => o.id).ToList();

                foreach (var item in list)
                {
                    BCodeModel model = new BCodeModel()
                    {
                        Id = item.id,
                        Code = item.code,
                        Desc = item.description,
                    };

                    models.Add(model);
                }

                return Json(models);
            }
        }
    }
}
