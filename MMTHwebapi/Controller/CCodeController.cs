using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class CCodeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<CCodeModel> models = new List<CCodeModel>();
                var list = entity.c_code.OrderBy(o => o.id).ToList();

                foreach (var item in list)
                {
                    CCodeModel model = new CCodeModel()
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
