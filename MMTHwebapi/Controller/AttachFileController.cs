using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using TechLineCaseAPI.Model;

namespace TechLineCaseAPI.Controller
{
    public class AttachFileController : ApiController
    {
        [HttpGet]
        [Route("api/attachfile/get/{id}")]
        public IHttpActionResult Get(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.attachfiles
                    .Where(o => o.id == id)
                    .OrderBy(o => o.CREATED_ON)
                    .FirstOrDefault();
                return Json(item);
            }
        }

        [HttpGet]
        [Route("api/attachfile/min/{id}")]
        public IHttpActionResult GetMin(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.attachfiles.Select(o => new
                {
                    id = o.id,
                    Category = o.Category,
                    ObjectId = o.ObjectId,
                    IsImage = o.IsImage,
                    Thumbnail = o.Thumbnail,
                    FileName = o.FileName,
                    MimeType = o.MimeType,
                    FileSize = o.FileSize,

                    CREATED_BY = o.CREATED_BY,
                    CREATED_ON = o.CREATED_ON,
                    MODIFIED_BY = o.MODIFIED_BY,
                    MODIFIED_ON = o.MODIFIED_ON,
                    STATUS_CODE = o.STATUS_CODE,
                })
                    .Where(o => o.id == id)
                    .OrderBy(o => o.CREATED_ON)
                    .FirstOrDefault();

                return Json(item);
            }
        }

        [HttpGet]
        [Route("api/attachfile/all/{id}")]
        public IHttpActionResult GetAll(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.attachfiles.Select(o => new
                {
                    id = o.id,
                    Category = o.Category,
                    ObjectId = o.ObjectId,
                    IsImage = o.IsImage,
                    Thumbnail = o.Thumbnail,
                    FileName = o.FileName,
                    MimeType = o.MimeType,
                    FileSize = o.FileSize,

                    CREATED_BY = o.CREATED_BY,
                    CREATED_ON = o.CREATED_ON,
                    MODIFIED_BY = o.MODIFIED_BY,
                    MODIFIED_ON = o.MODIFIED_ON,
                    STATUS_CODE = o.STATUS_CODE,
                })
                    .Where(o => o.ObjectId == id)
                    .Where(o => !o.Category.Contains("Chat"))
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        [HttpGet]
        [Route("api/attachfile/allchat/{id}")]
        public IHttpActionResult GetAllChat(int id)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                var item = entity.attachfiles.Select(o => new
                {
                    id = o.id,
                    Category = o.Category,
                    ObjectId = o.ObjectId,
                    IsImage = o.IsImage,
                    Thumbnail = o.Thumbnail,
                    FileName = o.FileName,
                    MimeType = o.MimeType,
                    FileSize = o.FileSize,

                    CREATED_BY = o.CREATED_BY,
                    CREATED_ON = o.CREATED_ON,
                    MODIFIED_BY = o.MODIFIED_BY,
                    MODIFIED_ON = o.MODIFIED_ON,
                    STATUS_CODE = o.STATUS_CODE,
                })
                    .Where(o => o.ObjectId == id)
                    .Where(o => o.Category.Contains("Chat"))
                    .OrderBy(o => o.CREATED_ON)
                    .ToList();

                return Json(item);
            }
        }

        [HttpPost]
        [Route("api/attachfile/create")]
        public ResultMessage Post()
        {
            var httpPostedFile = HttpContext.Current.Request.Files["File"];
            var category = HttpContext.Current.Request.Params["Category"];
            var objectId = HttpContext.Current.Request.Params["ObjectId"];
            var createdBy = HttpContext.Current.Request.Params["CreatedBy"];
            var TempKey = HttpContext.Current.Request.Params["TempKey"];

            if (httpPostedFile == null) return new ResultMessage() { Status = "E", Message = "Require File" };
            if (category == null) return new ResultMessage() { Status = "E", Message = "Require Category" };
            if (objectId == null) return new ResultMessage() { Status = "E", Message = "Require Object Id" };
            if (createdBy == null) return new ResultMessage() { Status = "E", Message = "Require Created By" };

            int? myid = TempKey==null? CreateAttachFile(httpPostedFile, category, objectId, createdBy) : CreateAttachFileTempKey(httpPostedFile, category, objectId, createdBy, TempKey);

            if (myid != null)
            {
                return new ResultMessage() { 
                    Status = "S", 
                    Message = "Create Completed" 
                };
            }
            else
            {
                return new ResultMessage() { 
                    Status = "E", 
                    Message = "Create Incompleted" 
                };
            }
        }

        [HttpPost]
        [Route("api/attachfile/delete")]
        public ResultMessage Delete()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteAttachFile(id))
            {
                return new ResultMessage() { 
                    Status = "S", 
                    Message = "Delete Completed" 
                };
            }
            else
            {
                return new ResultMessage() { 
                    Status = "E", 
                    Message = "Delete Incompleted" 
                };
            }
        }

        private static byte[] GetFileBytes(bool isImage, HttpPostedFile httpPostedFile, Stream stream, WebImage img)
        {
            if (isImage)
            {
                return img.GetBytes();
            }
            else
            {
                using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                {
                    return binaryReader.ReadBytes(httpPostedFile.ContentLength);
                }
            }
        }

        private static byte[] CreateThumbnail(bool isImage, Stream stream, WebImage img)
        {
            if (isImage)
            {
                img.Resize(120, 120);
                return img.GetBytes();
            }
            else
            {
                return null;
            }
        }

        private static bool IsImageFile(Stream stream)
        {
            try
            {
                Image imgInput = Image.FromStream(stream);
                Graphics gInput = Graphics.FromImage(imgInput);
                System.Drawing.Imaging.ImageFormat thisFormat = imgInput.RawFormat;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int? CreateAttachFileTempKey(HttpPostedFile httpPostedFile, string category, string objectId, string createdBy,string tempkey)
        {
            try
            {
                if (httpPostedFile == null) return null;
                if (category == null) return null;
                if (objectId == null) return null;
                if (createdBy == null) return null;

                Stream stream = httpPostedFile.InputStream;
                bool isImage = IsImageFile(stream);
                byte[] filebytes;
                byte[] thumbnailbytes;
                int? myid;

                if (isImage)
                {
                    WebImage img = new WebImage(stream);
                    filebytes = GetFileBytes(isImage, httpPostedFile, null, img);
                    thumbnailbytes = CreateThumbnail(isImage, null, img);
                }
                else
                {
                    filebytes = GetFileBytes(isImage, httpPostedFile, stream, null);
                    thumbnailbytes = null;
                }

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new attachfile()
                    {
                        Category = category,
                        ObjectId = int.Parse(objectId),
                        IsImage = isImage,
                        Thumbnail = thumbnailbytes,
                        DocumentBody = filebytes, //Convert.ToBase64String(filebytes)
                        FileName = new FileInfo(httpPostedFile.FileName).Name,
                        MimeType = httpPostedFile.ContentType,
                        FileSize = filebytes.Length,
                        tempkey= tempkey,
                        CREATED_BY = createdBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = createdBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.attachfiles.AddObject(record);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    myid = record.id;

                    //entity.ratings.Attach(record);
                    //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                    //entity.SaveChanges();
                    //entity.Refresh(RefreshMode.StoreWins, record);
                }

                return myid;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static int? CreateAttachFile(HttpPostedFile httpPostedFile, string category, string objectId, string createdBy)
        {
            try
            {
                if (httpPostedFile == null) return null;
                if (category == null) return null;
                if (objectId == null) return null;
                if (createdBy == null) return null;

                Stream stream = httpPostedFile.InputStream;
                bool isImage = IsImageFile(stream);
                byte[] filebytes;
                byte[] thumbnailbytes;
                int? myid;

                if (isImage)
                {
                    WebImage img = new WebImage(stream);
                    filebytes = GetFileBytes(isImage, httpPostedFile, null, img);
                    thumbnailbytes = CreateThumbnail(isImage, null, img);
                }
                else
                {
                    filebytes = GetFileBytes(isImage, httpPostedFile, stream, null);
                    thumbnailbytes = null;
                }

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new attachfile()
                    {
                        Category = category,
                        ObjectId = int.Parse(objectId),
                        IsImage = isImage,
                        Thumbnail = thumbnailbytes,
                        DocumentBody = filebytes, //Convert.ToBase64String(filebytes)
                        FileName = new FileInfo(httpPostedFile.FileName).Name,
                        MimeType = httpPostedFile.ContentType,
                        FileSize = filebytes.Length,

                        CREATED_BY = createdBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = createdBy,
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };

                    entity.attachfiles.AddObject(record);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    myid = record.id;

                    //entity.ratings.Attach(record);
                    //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                    //entity.SaveChanges();
                    //entity.Refresh(RefreshMode.StoreWins, record);
                }

                return myid;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool DeleteAttachFile(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int myid = int.Parse(id);
                    attachfile record = entity.attachfiles.Where(o => o.id == myid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.attachfiles.Attach(record);
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
    }
}
