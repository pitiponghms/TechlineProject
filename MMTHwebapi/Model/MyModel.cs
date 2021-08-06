using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechLineCaseAPI.Model
{
    public class ResultMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }
    }

    public class Authen
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string MobileKey { get; set; }
        public string Pin { get; set; }
    }

    public class ResultModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
    public class ROSubjectModel
    {
        public int Id { get; set; }
        public Guid? SubjectId { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class RODealerModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }
    public class ACodeModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class BCodeModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class CCodeModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Desc { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class vROMessageModel
    {
        public int? Id { get; set; }
        public int? CaseId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public int AttachFileId { get; set; }

        //public DateTime Time { get; set; }

        public HttpPostedFile File { get; set; }

        public string Category { get; set; }

        //public string ObjectId { get; set; } // CaseId

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class ROCaseModel
    {
        public int? Id { get; set; }
        public string CaseId { get; set; }
        public string Dealer { get; set; }
        public string Out_offcde { get; set; }
        public string Out_cmpcde { get; set; }
        public string Out_rocode { get; set; }
        public string Out_cust_date { get; set; }
        public string Out_ro_status { get; set; }
        public string Out_rodate { get; set; }
        public string Out_rotime { get; set; }
        public string Out_warranty_date { get; set; }
        public string Out_expiry_date { get; set; }
        public string Out_license { get; set; }
        public string Out_prdcde { get; set; }
        public string Out_chasno { get; set; }
        public string Out_engno { get; set; }
        public string Out_model { get; set; }
        public string Out_kilo_last { get; set; }
        public string Out_last_date { get; set; }
        public string Out_idno { get; set; }
        public string Out_cusname { get; set; }
        public string Out_mobile { get; set; }
        public string Out_address { get; set; }
        public string Out_province { get; set; }
        public string Out_zipcode { get; set; }
        public string Out_custype { get; set; }
        public string A_code { get; set; }
        public string B_code { get; set; }
        public string C_code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
        public string LevelofProblem { get; set; }
        public string CaseTitle { get; set; }
        public string CaseType { get; set; }
        public string CaseSubject { get; set; }
        public string CaseDescription { get; set; }
        public string OUT_SYS_CODE { get; set; }
        public string OUT_SYS_STS { get; set; }
        public string OUT_SYS_MSG { get; set; }
        public string StatusCodeText { get; set; }

        public IList<Operation> operation { get; set; }

        public string Problem { get; set; }
        public string TimeOccur { get; set; }
        public string TimeFreq { get; set; }
        public int? TimeFreqAmount { get; set; }
        public string TimeFreqIn { get; set; }
        public string Weather { get; set; }
        public string WeatherOther { get; set; }
        public string RoadCondition { get; set; }
        public string RoadConditionOther { get; set; }
        public string RoadFloor { get; set; }
        public string RoadFloorOther { get; set; }
        public string Gear { get; set; }
        public string GearOther { get; set; }
        public string Tire { get; set; }
        public string TireOther { get; set; }
        public string Tread { get; set; }
        public string TreadOther { get; set; }
        public string MaintenanceHistory { get; set; }
        public string MaintenanceHistoryOther { get; set; }
        public string Accident { get; set; }
        public string AccidentOther { get; set; }
        public string TransformCar { get; set; }
        public string TransformCarOther { get; set; }
        public string MicrosoftTeamLink { get; set; }
        public string CaseSubjectName { get; set; }
        public string SolutionForDealer { get; set; }
        public string TafNo { get; set; }
        public string TafYear { get; set; }

    }

    public class Rocode
    {
        public string rocode { get; set; }
    }

    public class ROCaseLogModel
    {
        public int? Id { get; set; }
        public string CaseId { get; set; }
        public string StatusCodeFrom { get; set; }
        public string StatusCodeTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class UserMitsu
    {
        public string id { get; set; }
        public string user_mail { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_photo_url { get; set; }
        public string token { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public string dealer { get; set; }
        public string dealer_name { get; set; }
        public string mobile_key { get; set; }
        public string pin { get; set; }
    }

    public class Incident
    {
        public string id { get; set; }
        public string CaseId { get; set; }
        public string Dealer { get; set; }
        public string Out_offcde { get; set; }
        public string Out_cmpcde { get; set; }
        public string Out_rocode { get; set; }
        public string Out_cust_date { get; set; }
        public string Out_ro_status { get; set; }
        public string Out_rodate { get; set; }
        public string Out_rotime { get; set; }
        public string Out_warranty_date { get; set; }
        public string Out_expiry_date { get; set; }
        public string Out_license { get; set; }
        public string Out_prdcde { get; set; }
        public string Out_chasno { get; set; }
        public string Out_engno { get; set; }
        public string Out_model { get; set; }
        public string Out_kilo_last { get; set; }
        public string Out_last_date { get; set; }
        public string Out_idno { get; set; }
        public string Out_cusname { get; set; }
        public string Out_mobile { get; set; }
        public string Out_address { get; set; }
        public string Out_province { get; set; }
        public string Out_zipcode { get; set; }
        public string Out_custype { get; set; }
        public string A_code { get; set; }
        public string B_code { get; set; }
        public string C_code { get; set; }
        public string CreatedBy { get; set; }
        public string LevelofProblem { get; set; }
        public string CaseTitle { get; set; }
        public string CaseType { get; set; }
        public string CaseSubject { get; set; }
        public string CaseDescription { get; set; }
        public string createdBy { get; set; }
        public string createdOn { get; set; }
        public string modifiedOn { get; set; }
        public string modifiedBy { get; set; }
        public string statusCode { get; set; }
        public string statusCodeText { get; set; }
    }
    public class RatingModel
    {
        public int? Id { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public int? ROCaseId { get; set; }
        public string CASEID { get; set; }
        public IEnumerable<RatingSubjectModel> SubjectModel { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
    }

    public class RatingSubjectModel
    {
        public int? Id { get; set; }
        public string Subject { get; set; }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        public int? RatingId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
    }

    public class AssessmentMasterModel
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public int? GroupNo { get; set; }
        public int? SeqNo { get; set; }
        public string Subject { get; set; }

        public string ChoiceName1 { get; set; }
        public int? ChoiceValue1 { get; set; }
        public bool? IsRadio1 { get; set; }
        public bool? IsOther1 { get; set; }

        public string ChoiceName2 { get; set; }
        public int? ChoiceValue2 { get; set; }
        public bool? IsRadio2 { get; set; }
        public bool? IsOther2 { get; set; }

        public string ChoiceName3 { get; set; }
        public int? ChoiceValue3 { get; set; }
        public bool? IsRadio3 { get; set; }
        public bool? IsOther3 { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
    }

    public class AssessmentFormModel : AssessmentMasterModel
    {
        public int? ROCaseId { get; set; }
        public int? ChoiceValue { get; set; }
        public string Message { get; set; }
        public bool? IsComment { get; set; }
    }

    public class RatingMasterModel
    {
        public int? Id { get; set; }
        public string Subject { get; set; }
        public decimal? MaxScore { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
    }
    public class Operation
    {
        public string OUT_COMMANDCODE { get; set; }
        public string OUT_COMMANDDESC { get; set; }
        public string OUT_SERVICE_TYPE { get; set; }
        public string OUT_OPTCODE { get; set; }
        public string OUT_OPT_DESC { get; set; }
        public string OUT_EXPENSE_TYPE { get; set; }
    }

    public class RocodeOperation
    {
        public string rocode { get; set; }
        public string dealercode { get; set; }
        public string offdealercode { get; set; }
        public string requestno { get; set; }
    }


    public class ROUserModel
    {
        public int? Id { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string ProfilePhotoUrl { get; set; }
        //public string Token { get; set; }
        //public string IdToken { get; set; }
        //public string RefreshToken { get; set; }
        //public string Dealer { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string StatusCode { get; set; }
    }
    public class TimeOccur
    {
  
        public TimeOccur(string v1, string v2)
        {
            this.Code = v1;
            this.Name = v2;
        }

        public string Code { get; set; }
        public string Name { get; set; }

    }
    public class TimeFreq
    {
        public TimeFreq(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }

        public string Code { get; set; }
        public string Name { get; set; }

    }
    public class Whether
    {
        public Whether(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class RoadCondition
    {
        public RoadCondition(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class RoadFloor
    {
        public RoadFloor(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Gear
    {
        public Gear(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }



    public class Tire
    {
        public Tire(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Tread
    {
        public Tread(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class MaintenanceHistory
    {
        public MaintenanceHistory(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Accident
    {
        public Accident(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class TransformCar
    {
        public TransformCar(string v1, string v2)
        {
            Code = v1;
            Name = v2;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    static class GlobalParam
    {
        //private  List<TimeOccur> _TimeOccurList = new List<TimeOccur>();
        public static List<TimeOccur> TimeOccurList;
        public static List<TimeFreq> TimeFreqList;
        public  static List<Whether> WhetherList;
        public static List<RoadCondition> RoadConditionList;
        public static List<RoadFloor> RoadFloorList;
        public static List<Gear> GearList;
        public static List<Tire> TireList;
        public static List<Tread> TreadList;
        public static List<MaintenanceHistory> MaintenanceHistoryList;
        public static List<Accident> AccidentList;
        public static List<TransformCar> TransformCarList;
        public static List<TimeOccur> getTimeOccurList()
        {
            TimeOccurList = new List<TimeOccur>();

            return TimeOccurList;

        }
        static GlobalParam()        {
            //
            // Allocate the list.
            //
            TimeOccurList = new List<TimeOccur>();
            TimeOccurList.Add(new  TimeOccur("01", "เข้า"));
            TimeOccurList.Add(new TimeOccur("02", "เที่ยง"));
            TimeOccurList.Add(new TimeOccur("03", "เย็น"));
            TimeOccurList.Add(new TimeOccur("04", "ตลอดเวลา"));


            TimeFreqList = new List<TimeFreq>();
            TimeFreqList.Add(new TimeFreq("01", "ตลอดเวลา"));
            TimeFreqList.Add(new TimeFreq("02", "บางครั้ง"));
            TimeFreqList.Add(new TimeFreq("03", "นานๆครั้ง"));
            TimeFreqList.Add(new TimeFreq("04", "จำนวนครั้ง"));


            WhetherList = new List<Whether>();
            WhetherList.Add(new Whether("01", "ร้อนจัด"));
            WhetherList.Add(new Whether("02", "เย็นจัด"));
            WhetherList.Add(new Whether("03", "ฝนตก"));
            WhetherList.Add(new Whether("04", "ปกติ"));
            WhetherList.Add(new Whether("05", "อื่นๆ"));

            RoadConditionList = new List<RoadCondition>();
            RoadConditionList.Add(new RoadCondition("01", "ถนนราบ"));
            RoadConditionList.Add(new RoadCondition("02", "ถนนเอียงซ้าย"));
            RoadConditionList.Add(new RoadCondition("03", "ถนนเอียงขวา"));
            RoadConditionList.Add(new RoadCondition("04", "ถนนหลังเต่า"));
            RoadConditionList.Add(new RoadCondition("05", "ถนนลาดชัน"));            
            RoadConditionList.Add(new RoadCondition("06", "อื่นๆ"));

            RoadFloorList = new List<RoadFloor>();
            RoadFloorList.Add(new RoadFloor("01", "ยางมะตอย"));
            RoadFloorList.Add(new RoadFloor("02", "คอนกรีต"));
            RoadFloorList.Add(new RoadFloor("03", "ขุขระ"));
            RoadFloorList.Add(new RoadFloor("04", "เรียบ"));
            RoadFloorList.Add(new RoadFloor("05", "อื่นๆ"));


            GearList = new List<Gear>();
            GearList.Add(new Gear("01", "เกียร์ธรรมดา"));
            GearList.Add(new Gear("02", "เกียร์ออโต้/CVT"));
            GearList.Add(new Gear("03", "ขับเคลื่อน 4 ล้อ"));
            GearList.Add(new Gear("04", "ระบุตำแหน่ง"));

            TireList = new List<Tire>();
            TireList.Add(new Tire("01", "อ่อน"));
            TireList.Add(new Tire("02", "แข็ง"));
            TireList.Add(new Tire("03", "ปกติ"));


            TreadList = new List<Tread>();
            TreadList.Add(new Tread("01", "ปกติ"));
            TreadList.Add(new Tread("02", "ไม่ปกติ"));


            MaintenanceHistoryList = new List<MaintenanceHistory>();
            MaintenanceHistoryList.Add(new MaintenanceHistory("01", "ครบ"));
            MaintenanceHistoryList.Add(new MaintenanceHistory("02", "ไม่ครบ"));



            AccidentList = new List<Accident>();
            AccidentList.Add(new Accident("01", "มี"));
            AccidentList.Add(new Accident("02", "ไม่มี"));

            TransformCarList = new List<TransformCar>();
            TransformCarList.Add(new TransformCar("01", "มี"));
            TransformCarList.Add(new TransformCar("02", "ไม่มี"));


        }

    }


}