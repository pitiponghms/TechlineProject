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
    public class ROUserController : ApiController
    {
        [HttpGet]
        [Route("api/rouser/get/{id}")]
        public IHttpActionResult Get(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.ro_user
                    .Where(o => o.id == id)
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        [HttpGet]
        [Route("api/rouser/get/all")]
        public IHttpActionResult GetAll(int? startIndex, int? maxRows)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                if(startIndex != null && maxRows != null)
                {
                    int sIndex = (int)startIndex;
                    int mRows = (int)maxRows;

                    var item = (from record in entity.ro_user
                                select record)
                        .OrderBy(o => o.CREATED_ON)
                        .Skip(sIndex)
                        .Take(mRows).ToList();

                    return Json(item);
                }
                else
                {
                    var item = entity.ro_user
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                    return Json(item);
                }
            }
        }

        [HttpPost]
        [Route("api/rouser/create")]
        public ResultMessage Post()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                ROUserModel model = js.Deserialize<ROUserModel>(json);
                int? myid;

                if (model.Mail == null) return new ResultMessage() { Status = "E", Message = "Require Mail" };
                if (model.FirstName == null) return new ResultMessage() { Status = "E", Message = "Require First Name" };
                if (model.LastName == null) return new ResultMessage() { Status = "E", Message = "Require Last Name" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                myid = CreateROUser(model);

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

        [HttpPost]
        [Route("api/rouser/update")]
        public ResultMessage Update()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                ROUserModel model = js.Deserialize<ROUserModel>(json);

                if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                if (model.StatusCode == null) return new ResultMessage() { Status = "E", Message = "Require Status Code" };
                if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateROUser(model))
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
        [Route("api/rouser/chuck/update")]
        public ResultMessage UpdateChunk()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                List<ROUserModel> models = js.Deserialize<List<ROUserModel>>(json);

                bool result = true;

                foreach (var model in models)
                {
                    try
                    {
                        if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                        if (model.StatusCode == null) return new ResultMessage() { Status = "E", Message = "Require Status Code" };
                        if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                        UpdateROUser(model);
                    }
                    catch
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
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

        public static int? CreateROUser(ROUserModel model)
        {
            try
            {
                int? myid;

                if (model.Mail == null) return null;
                if (model.FirstName == null) return null;
                if (model.LastName == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new ro_user()
                    {
                        user_mail = model.Mail,
                        first_name = model.FirstName,
                        last_name = model.LastName,
                        //profile_photo_url = model.,
                        //token = model.,
                        //id_token = model.,
                        //refresh_token = model.,
                        //dealer = model.,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.ro_user.AddObject(record);
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

        public static bool UpdateROUser(ROUserModel model)
        {
            try
            {
                if (model.Id == null) return false;
                if (model.StatusCode == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.ro_user.Where(o => o.id == model.Id).FirstOrDefault();

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

        //////////////////////////////////////////////////////////////////////////////////////////
        private static string AssignStringData(string source, string data)
        {
            switch (data)
            {
                case "": return null;
                case null: return source;
                default: return data;
            }
        }
    }
}
