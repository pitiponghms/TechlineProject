using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class ACodeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<ACodeModel> models = new List<ACodeModel>();
                var list = entity.a_code.ToList();
                
                foreach (var item in list)
                {
                    ACodeModel model = new ACodeModel()
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
