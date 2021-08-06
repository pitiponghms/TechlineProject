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
    public class RatingController : ApiController
    {
        [HttpGet]
        [Route("api/rating/count/{id}")]
        public int CountRatingByROCaseId(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                return entity.ratings
                    .Where(o => o.ro_caseid == id)
                    .Count();
            }
        }

        [HttpGet]
        [Route("api/rating/get/rocase/{id}")]
        public IHttpActionResult GetByROCaseId(int id)
        {
            try
            {
                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var item = entity.ratings.Select(o => new RatingModel
                    {
                        Id = o.id,
                        Category = o.category,
                        Comment = o.comment,
                        ROCaseId = o.ro_caseid,
                        CASEID = o.CASEID,
                        SubjectModel = entity.rating_subject.Select(x => new RatingSubjectModel
                        {
                            Id = x.id,
                            Subject = x.subject,
                            Score = x.score,
                            MaxScore = x.maxscore,
                            RatingId = x.ratingid,
                            CreatedBy = x.CREATED_BY,
                            CreatedOn = x.CREATED_ON,
                            ModifiedBy = x.MODIFIED_BY,
                            ModifiedOn = x.MODIFIED_ON,
                            StatusCode = x.STATUS_CODE,
                        })
                            .Where(x => x.RatingId == o.id),

                        CreatedBy = o.CREATED_BY,
                        CreatedOn = o.CREATED_ON,
                        ModifiedBy = o.MODIFIED_BY,
                        ModifiedOn = o.MODIFIED_ON,
                        StatusCode = o.STATUS_CODE,
                    })
                        .Where(o => o.ROCaseId == id)
                        .OrderBy(o => o.CreatedOn)
                        .FirstOrDefault();

                    return Json(item);
                }
            }catch(Exception ex) { return Json(new { Comment = ex.Message }); }
        }

        [HttpGet]
        [Route("api/rating/subject/get/all/{id}")]
        public IHttpActionResult GetSubject(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.rating_subject
                    .Where(o => o.ratingid == id)
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        [HttpGet]
        [Route("api/rating/master/get/all")]
        public IHttpActionResult GetMaster()
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.rating_master
                    .OrderBy(o => o.order_seq)
                    .ThenBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Route("api/rating/create")]
        public ResultMessage Post()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingModel model = js.Deserialize<RatingModel>(json);
                int? myid;

                if (model.Category == null) return new ResultMessage() { Status = "E", Message = "Require Category" };
                if (model.ROCaseId == null) return new ResultMessage() { Status = "E", Message = "Require RO Case Id" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                myid = CreateRating(model);

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
        [Route("api/rating/subject/create")]
        public ResultMessage PostSubject()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingSubjectModel model = js.Deserialize<RatingSubjectModel>(json);
                int? myid;

                if (model.Subject == null) return new ResultMessage() { Status = "E", Message = "Require Subject" };
                if (model.MaxScore == null) return new ResultMessage() { Status = "E", Message = "Require Max Score" };
                if (model.RatingId == null) return new ResultMessage() { Status = "E", Message = "Require Rating Id" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                myid = CreateRatingSubject(model);

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
        [Route("api/rating/master/create")]
        public ResultMessage PostMaster()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingMasterModel model = js.Deserialize<RatingMasterModel>(json);
                int? myid;

                if (model.Subject == null) return new ResultMessage() { Status = "E", Message = "Require Subject" };
                if (model.MaxScore == null) return new ResultMessage() { Status = "E", Message = "Require Max Score" };
                if (model.CreatedBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

                myid = CreateRatingMaster(model);

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

        //////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Route("api/rating/update")]
        public ResultMessage Update()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingModel model = js.Deserialize<RatingModel>(json);

                if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                if (model.ROCaseId == null) return new ResultMessage() { Status = "E", Message = "Require RO Case Id" };
                if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateRating(model))
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
        [Route("api/rating/subject/update")]
        public ResultMessage UpdateSubject()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingSubjectModel model = js.Deserialize<RatingSubjectModel>(json);

                if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateRatingSubject(model))
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
        [Route("api/rating/master/update")]
        public ResultMessage UpdateMaster()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                RatingMasterModel model = js.Deserialize<RatingMasterModel>(json);

                if (model.Id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };
                //if (model.Subject == null) return new ResultMessage() { Status = "E", Message = "Require Subject" };
                //if (model.MaxScore == null) return new ResultMessage() { Status = "E", Message = "Require Max Score" };
                if (model.ModifiedBy == null) return new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateRatingMaster(model))
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

        //////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Route("api/rating/delete")]
        public ResultMessage Delete()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteRating(id))
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

        [HttpPost]
        [Route("api/rating/subject/delete")]
        public ResultMessage DeleteSubject()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteRatingSubject(id))
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

        [HttpPost]
        [Route("api/rating/master/delete")]
        public ResultMessage DeleteMaster()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteRatingMaster(id))
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

        //////////////////////////////////////////////////////////////////////////////////////////

        public static int? CreateRating(RatingModel model)
        {
            try
            {
                int? myid;

                if (model.ROCaseId == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new rating()
                    {
                        category = model.Category,
                        comment = model.Comment,
                        ro_caseid = model.ROCaseId,
                        CASEID = model.CASEID,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.ratings.AddObject(record);
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

        public static int? CreateRatingSubject(RatingSubjectModel model)
        {
            try
            {
                int? myid;

                if (model.Subject == null) return null;
                if (model.MaxScore == null) return null;
                if (model.RatingId == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new rating_subject()
                    {
                        subject = model.Subject,
                        score = model.Score,
                        maxscore = model.MaxScore,
                        ratingid = model.RatingId,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.rating_subject.AddObject(record);
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

        public static int? CreateRatingMaster(RatingMasterModel model)
        {
            try
            {
                int? myid;

                if (model.Subject == null) return null;
                if (model.MaxScore == null) return null;
                if (model.CreatedBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new rating_master()
                    {
                        subject = model.Subject,
                        maxscore = model.MaxScore,

                        CREATED_BY = model.CreatedBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = model.CreatedBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.rating_master.AddObject(record);
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

        //////////////////////////////////////////////////////////////////////////////////////////

        public static bool UpdateRating(RatingModel model)
        {
            try
            {
                if (model.Id == null) return false;
                if (model.ROCaseId == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.ratings.Where(o => o.id == model.Id).FirstOrDefault();

                    record.comment = AssignStringData(record.comment, model.Comment);
                    record.ro_caseid = AssignNumberData(record.ro_caseid, model.ROCaseId);
                    record.CASEID = AssignStringData(record.CASEID, model.CASEID);

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

        public static bool UpdateRatingSubject(RatingSubjectModel model)
        {
            try
            {
                if (model.Id == null) return false;
                //if (model.Subject == null) return false;
                //if (model.MaxScore == null) return false;
                //if (model.RatingId == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.rating_subject.Where(o => o.id == model.Id).FirstOrDefault();

                    record.subject = AssignStringData(record.subject, model.Subject);
                    record.score = AssignNumberData(record.score, model.Score);
                    record.maxscore = AssignNumberData(record.maxscore, model.MaxScore);

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

        public static bool UpdateRatingMaster(RatingMasterModel model)
        {
            try
            {
                if (model.Id == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.rating_master.Where(o => o.id == model.Id).FirstOrDefault();

                    record.subject = AssignStringData(record.subject, model.Subject);
                    record.maxscore = AssignNumberData(record.maxscore, model.MaxScore);

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

        public static bool DeleteRating(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int myid = int.Parse(id);
                    rating record = entity.ratings.Where(o => o.id == myid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.ratings.Attach(record);
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

        public static bool DeleteRatingSubject(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int myid = int.Parse(id);
                    rating_subject record = entity.rating_subject.Where(o => o.id == myid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.rating_subject.Attach(record);
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

        public static bool DeleteRatingMaster(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int myid = int.Parse(id);
                    rating_master record = entity.rating_master.Where(o => o.id == myid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.rating_master.Attach(record);
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

        private static int? AssignNumberData(int? source, int? data)
        {
            switch (data)
            {
                case 0: return null;
                case null: return source;
                default: return data;
            }
        }

        private static decimal? AssignNumberData(decimal? source, decimal? data)
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
