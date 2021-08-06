using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class AssessmentFormController : ApiController
    {
        [HttpGet]
        [Route("api/assessment/master/get/{type}")]
        public IHttpActionResult GetMaster(string type)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<AssessmentMasterModel> models = new List<AssessmentMasterModel>();
                var list = entity.assessmentform_master
                    .Where(o => o.assessmentform_type.Contains(type))
                    .OrderBy(o => o.groupno)
                    .ThenBy(o => o.seqno)
                    .ToList();

                foreach (var item in list)
                {
                    AssessmentMasterModel model = new AssessmentMasterModel()
                    {
                        Id = item.id,
                        Type = item.assessmentform_type,
                        GroupNo = item.groupno,
                        SeqNo = item.seqno,
                        Subject = item.subject,

                        ChoiceName1 = item.choicename1,
                        ChoiceValue1 = item.choicevalue1,
                        IsRadio1 = item.isradio1,
                        IsOther1 = item.isother1,

                        ChoiceName2 = item.choicename2,
                        ChoiceValue2 = item.choicevalue2,
                        IsRadio2 = item.isradio2,
                        IsOther2 = item.isother2,

                        ChoiceName3 = item.choicename3,
                        ChoiceValue3 = item.choicevalue3,
                        IsRadio3 = item.isradio3,
                        IsOther3 = item.isother3,
                    };

                    models.Add(model);
                }
                return Json(models);
            }
        }

        [HttpGet]
        [Route("api/assessment/form/get/{rocaseid}/{type}")]
        public IHttpActionResult GetFormView(int rocaseid, string type)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<AssessmentFormModel> models = new List<AssessmentFormModel>();
                var list = entity.vAssessmentForms
                    .Where(o => o.assessmentform_type.Contains(type))
                    .Where(o => o.ro_caseid == rocaseid || (o.ro_caseid == null && o.seqno == 0)) //Get Header
                    .OrderBy(o => o.groupno)
                    .ThenBy(o => o.seqno)
                    .ToList();

                foreach (var item in list)
                {
                    AssessmentFormModel model = new AssessmentFormModel()
                    {
                        Id = item.id,
                        Type = item.assessmentform_type,
                        GroupNo = item.groupno,
                        SeqNo = item.seqno,
                        Subject = item.subject,

                        ChoiceName1 = item.choicename1,
                        ChoiceValue1 = item.choicevalue1,
                        IsRadio1 = item.isradio1,
                        IsOther1 = item.isother1,

                        ChoiceName2 = item.choicename2,
                        ChoiceValue2 = item.choicevalue2,
                        IsRadio2 = item.isradio2,
                        IsOther2 = item.isother2,

                        ChoiceName3 = item.choicename3,
                        ChoiceValue3 = item.choicevalue3,
                        IsRadio3 = item.isradio3,
                        IsOther3 = item.isother3,

                        ChoiceValue = item.choicevalue,
                        Message = item.message,
                        IsComment = item.iscomment,
                    };

                    models.Add(model);
                }
                return Json(models);
            }
        }

        [HttpGet]
        [Route("api/assessment/form/count/{rocaseid}/{type}")]
        public IHttpActionResult CountForm(int rocaseid, string type)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var list = entity.assessmentforms
                    .Where(o => o.assessmentform_type.Contains(type))
                    .Where(o => o.groupno != null)
                    .Where(o => o.ro_caseid == rocaseid)
                    .ToList();

                return Json(list.Count());
            }
        }

        [HttpPost]
        [Route("api/assessment/form/post/create")]
        public ResultMessage Post()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                AssessmentFormModel model = js.Deserialize<AssessmentFormModel>(json);
                int? myid;

                if (model.ROCaseId == null) return new ResultMessage() { Status = "E", Message = "Require RO Case Id" };
                if (model.Type == null) return new ResultMessage() { Status = "E", Message = "Require Type" };
                if (model.GroupNo == null && model.SeqNo == null) return new ResultMessage() { Status = "E", Message = "Require Group No." };
                if (model.SeqNo == null) return new ResultMessage() { Status = "E", Message = "Require Seq No." };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                myid = CreateAssessmentForm(model);

                if (myid != null)
                {
                    return new ResultMessage()
                    {
                        Status = "S",
                        Message = "Create Completed",
                        Value = myid,
                    };
                }
                else
                {
                    return new ResultMessage()
                    {
                        Status = "E",
                        Message = "Create Incompleted"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    Status = "E",
                    Message = "Create Incompleted (" + ex.Message + ")"
                };
            }
        }

        public static int? CreateAssessmentForm(AssessmentFormModel model)
        {
            try
            {
                int? myid;

                if (model.ROCaseId == null) return null;
                if (model.Type == null) return null;
                if (model.GroupNo == null && model.SeqNo == null) return null;
                if (model.SeqNo == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new assessmentform()
                    {
                        ro_caseid = model.ROCaseId,

                        assessmentform_type = model.Type,
                        groupno = model.GroupNo,
                        seqno = model.SeqNo,
                        choicevalue = model.ChoiceValue,
                        message = model.Message,
                        iscomment = model.IsComment,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.assessmentforms.AddObject(record);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    myid = record.id;
                }

                return myid;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
