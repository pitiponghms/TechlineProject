using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class RODealerController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.ro_dealer.Where(o => o.id == id).FirstOrDefault();

                RODealerModel model = new RODealerModel()
                {
                    Id = item.id,
                    Code = item.code,
                    Name = item.name,
                    EnglishName = item.englishname,
                };

                return Json(model);
            }
        }
    }
}
