using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class ROSubjectController : ApiController
    {
        [HttpGet]
        [Route("api/subject/get/{id}")]
        public IHttpActionResult Get(Guid id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.ro_subject.Where(o => o.subjectid == id).FirstOrDefault();

                ROSubjectModel model = new ROSubjectModel()
                {
                    Id = item.id,
                    SubjectId = item.subjectid,
                    Subject = item.subject,
                    Name = item.name,
                    EnglishName = item.englishname,
                };

                return Json(model);
            }
        }

        [HttpGet]
        [Route("api/subject/all")]
        public IHttpActionResult GetAll()
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<ROSubjectModel> models = new List<ROSubjectModel>();
                var list = entity.ro_subject.ToList();
                
                foreach (var item in list)
                {
                    ROSubjectModel model = new ROSubjectModel()
                    {
                        Id = item.id,
                        SubjectId = item.subjectid,
                        Subject = item.subject,
                        Name = item.name,
                        EnglishName = item.englishname,
                    };

                    models.Add(model);
                }

                return Json(models);
            }
        }
    }
}
