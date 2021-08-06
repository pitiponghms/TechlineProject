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
    public class ROMessageController : ApiController
    {
        [HttpGet]
        [Route("api/message/get/{id}")]
        public IHttpActionResult Get(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.vROMessages
                    .Where(o => o.caseid == id)
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        [HttpPost]
        [Route("api/message/create")]
        public ResultMessage Post()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var httpPostedFile = HttpContext.Current.Request.Files["File"];
                var json = HttpContext.Current.Request.Form["vModel"];
                
                vROMessageModel model = js.Deserialize<vROMessageModel>(json);
                int? myid;

                if (model.CaseId == null) return new ResultMessage() { Status = "E", Message = "Require Case Id" };
                if (model.SenderId == null) return new ResultMessage() { Status = "E", Message = "Require Sender Id" };
                if (model.SenderName == null) return new ResultMessage() { Status = "E", Message = "Require Sender Name" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                if (httpPostedFile != null)
                {
                    int? attachfileid = AttachFileController
                        .CreateAttachFile(httpPostedFile, model.Category, model.CaseId.ToString(), model.CreatedBy);

                    if(attachfileid != null)
                    {
                        model.AttachFileId = int.Parse(attachfileid.ToString());
                        myid = CreateROMessage(model);
                    }
                    else
                    {
                        return new ResultMessage() { 
                            Status = "E", 
                            Message = "Create Incompleted (Create Attach File Error)" 
                        };
                    }
                }
                else
                {
                    if (model.Text.Length == 0) new ResultMessage() { Status = "E", Message = "Require Text/File" };

                    myid = CreateROMessage(model);
                }

                if (myid != null)
                {
                    return new ResultMessage()
                    {
                        Status = "S",
                        Message = "Create Completed"
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


        [HttpPost]
        [Route("api/message/createTech")]
        public ResultMessage PostTech()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var httpPostedFile = HttpContext.Current.Request.Files["File"];
                var json = HttpContext.Current.Request.Form["vModel"];

                vROMessageModel model = js.Deserialize<vROMessageModel>(json);
                int? myid;

                if (model.CaseId == null) return new ResultMessage() { Status = "E", Message = "Require Case Id" };
                if (model.SenderId == null) return new ResultMessage() { Status = "E", Message = "Require Sender Id" };
                if (model.SenderName == null) return new ResultMessage() { Status = "E", Message = "Require Sender Name" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                if (httpPostedFile != null)
                {
                    int? attachfileid = AttachFileController
                        .CreateAttachFile(httpPostedFile, model.Category, model.CaseId.ToString(), model.CreatedBy);

                    if (attachfileid != null)
                    {
                        model.AttachFileId = int.Parse(attachfileid.ToString());
                        myid = CreateROMessage(model);

                        string statusHeader = "มีข้อความใหม่จาก Techline";
                        string statusBody = model.Text;

                        ROCaseController.SendUpdateStatus(statusHeader, statusBody);

                    }
                    else
                    {
                        return new ResultMessage()
                        {
                            Status = "E",
                            Message = "Create Incompleted (Create Attach File Error)"
                        };
                    }
                }
                else
                {
                    if (model.Text.Length == 0) new ResultMessage() { Status = "E", Message = "Require Text/File" };

                    myid = CreateROMessage(model);
                    string statusHeader = "มีข้อความใหม่จาก Techline";
                    string statusBody = model.Text;
                    
                        ROCaseController.SendUpdateStatus(statusHeader, statusBody);
                    
                }

                if (myid != null)
                {
                    return new ResultMessage()
                    {
                        Status = "S",
                        Message = "Create Completed"
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


        [HttpPost]
        [Route("api/message/update")]
        public ResultMessage Update()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                vROMessageModel model = js.Deserialize<vROMessageModel>(json);

                if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                //if (model.CaseId == null) return new ResultMessage() { Status = "E", Message = "Require Case Id" };
                //if (model.SenderId == null) return new ResultMessage() { Status = "E", Message = "Require Sender Id" };
                //if (model.SenderName == null) return new ResultMessage() { Status = "E", Message = "Require Sender Name" };
                if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateROMessage(model))
                {
                    return new ResultMessage()
                    {
                        Status = "S",
                        Message = "Update Completed"
                    };
                }
                else
                {
                    return new ResultMessage()
                    {
                        Status = "E",
                        Message = "Update Incompleted"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    Status = "E",
                    Message = "Update Incompleted (" + ex.Message + ")"
                };
            }
        }

        [HttpPost]
        [Route("api/message/delete")]
        public ResultMessage Delete()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteROMessage(id))
            {
                return new ResultMessage()
                {
                    Status = "S",
                    Message = "Delete Completed"
                };
            }
            else
            {
                return new ResultMessage()
                {
                    Status = "E",
                    Message = "Delete Incompleted"
                };
            }
        }

        public static int? CreateROMessage(vROMessageModel model)
        {
            try
            {
                int? myid;

                if (model.CaseId == null) return null;
                if (model.SenderId == null) return null;
                if (model.SenderName == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new ro_messages()
                    {
                        caseid = model.CaseId,
                        sender_id = model.SenderId.ToString(),
                        sender_name = model.SenderName,
                        text = model.Text,
                        attachfileid = model.AttachFileId,
                        time = DateTime.Now,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.ro_messages.AddObject(record);
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

        public static bool UpdateROMessage(vROMessageModel model)
        {
            try
            {
                if (model.Id == null) return false;
                //if (model.CaseId == null) return false;
                //if (model.SenderId == null) return false;
                //if (model.SenderName == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.ro_messages.Where(o => o.id == model.Id).FirstOrDefault();

                    //record.caseid = AssignStringData(record.caseid, model.CaseId);
                    //record.sender_id = AssignStringData(record.sender_id, model.SenderId.ToString());
                    //record.sender_name = AssignStringData(record.sender_name, model.SenderName);
                    record.text = AssignStringData(record.text, model.Text);
                    record.attachfileid = AssignNumberData(record.attachfileid, model.AttachFileId);
                    //record.time = model.Time;

                    string sourceStatus = record.STATUS_CODE;
                    //CREATED_BY = model.CreatedBy;
                    //CREATED_ON = DateTime.Now;
                    record.MODIFIED_BY = model.ModifiedBy;
                    record.MODIFIED_ON = DateTime.Now;
                    record.STATUS_CODE = AssignStringData(sourceStatus, model.StatusCode);

                    //entity.ro_case.Attach(record);
                    //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteROMessage(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int myid = int.Parse(id);
                    ro_messages record = entity.ro_messages.Where(o => o.id == myid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.ro_messages.Attach(record);
                        entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Deleted);
                        entity.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string AssignStringData(string source, string data)
        {
            switch (data)
            {
                case "": return null;
                case null: return source;
                default: return data;
            }
        }

        private static int? AssignNumberData(int? source, int? data)
        {
            switch (data)
            {
                case 0: return null;
                case null: return source;
                default: return data;
            }
        }
    }
}
