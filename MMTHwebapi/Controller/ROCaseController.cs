using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using TechLineCaseAPI.MMThApi;
using TechLineCaseAPI.Model;
using static TechLineCaseAPI.MMThApi.WS_InterfaceCRMSoapClient;


namespace TechLineCaseAPI.Controller
{
    public class ROCaseController : ApiController
    {
        public IOrganizationService _service = null;

        public void ConnectToMSCRM()
        {
            string UserName = TechLineCaseAPI.Properties.Settings.Default.UserName;
            string Password = TechLineCaseAPI.Properties.Settings.Default.Password;
            string SoapOrgServiceUri = TechLineCaseAPI.Properties.Settings.Default.SoapOrgServiceUri;//
            try
            {

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                     UserName = TechLineCaseAPI.Properties.Settings.Default.HMSUserName;
                     Password = TechLineCaseAPI.Properties.Settings.Default.HMSPassword;
                     SoapOrgServiceUri = TechLineCaseAPI.Properties.Settings.Default.HMSSoapOrgServiceUri;//

                }
                else
                {
                     UserName = TechLineCaseAPI.Properties.Settings.Default.UserName;
                     Password = TechLineCaseAPI.Properties.Settings.Default.Password;
                     SoapOrgServiceUri = TechLineCaseAPI.Properties.Settings.Default.SoapOrgServiceUri;//

                }

                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _service = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }

        [HttpGet]
        [Route("api/rocaselog/all")]
        public IHttpActionResult GetLogAll(string caseid)
        {
            using (mmthapiEntities entity = new mmthapiEntities())
            {
                List<ROCaseLogModel> models = new List<ROCaseLogModel>();
                var list = entity.ro_case_log.Where(o => o.CASEID.Contains(caseid)).ToList();

                foreach (var item in list)
                {
                    ROCaseLogModel model = new ROCaseLogModel()
                    {
                        Id = item.id,
                        CaseId = item.CASEID,
                        StatusCodeFrom = item.STATUS_CODE_FROM,
                        StatusCodeTo = item.STATUS_CODE_TO,
                        CreatedBy = item.CREATED_BY,
                        CreatedOn = item.CREATED_ON,
                        ModifiedBy = item.MODIFIED_BY,
                        ModifiedOn = item.MODIFIED_ON,
                    };

                    models.Add(model);
                }

                return Json(models);
            }
        }

        [HttpGet]
        [Route("api/rocaselog/status/pending/{dealercode}")]
        public IHttpActionResult GetListCaseByStatus(string dealercode)
        {
            //var PendingStatus = new string[] { "0", "1", "2","3","4" };
            using (mmthapiEntities entity = new mmthapiEntities())
            {

                var list =( dealercode == "admin"|| dealercode==null) ? entity.vRoCases.Where(o => (o.STATUS_CODE == "0" || o.STATUS_CODE == "1" || o.STATUS_CODE == "2" || o.STATUS_CODE == "3" || o.STATUS_CODE == "4")).ToList()
                    : entity.vRoCases.Where(o => (o.STATUS_CODE == "0" || o.STATUS_CODE == "1" || o.STATUS_CODE == "2" || o.STATUS_CODE == "3" || o.STATUS_CODE == "4") && o.DEALER == dealercode).ToList();

                if (list != null)
                {
                    List<ROCaseModel> items = new List<ROCaseModel>();
                    foreach (var row in list)
                    {
                        int caseid = row.id;
                        List<Operation> mdl = new List<Operation>();
                        var ol = entity.ro_operation.Where(o => o.case_id == caseid).ToList();
                        if (ol != null)
                        {
                            if (ol.Count > 0)
                            {
                                foreach (var oli in ol)
                                {

                                    Operation opmodel = new Operation()
                                    {
                                        OUT_COMMANDCODE = oli.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = oli.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = oli.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = oli.OUT_OPTCODE,
                                        OUT_OPT_DESC = oli.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = oli.OUT_EXPENSE_TYPE,
                                    };

                                    mdl.Add(opmodel);
                                }
                                items.Add(new ROCaseModel
                                {

                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    operation = mdl,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo=row.tafno,
                                    TafYear=row.tafyear,
                                   // MicrosoftTeamLink = row.MicrosoftTeamLink,

                                }
);
                            }
                            else
                            {
                                items.Add(new ROCaseModel
                                {
                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }

                        }

                    }








                    return Json(items);
                }
                else { return Json(""); }


            }
        }
        [HttpGet]
        [Route("api/rocaselog/status/completed/{dealercode}")]
        public IHttpActionResult GetListCaseByStatusCompleted(string dealercode)
        {
            //var PendingStatus = new string[] { "0", "1", "2","3","4" };
            using (mmthapiEntities entity = new mmthapiEntities())
            {

                var list = (dealercode == "admin" || dealercode == null) ? entity.vRoCases.Where(o => (o.STATUS_CODE == "5" || o.STATUS_CODE == "6")).ToList(): entity.vRoCases.Where(o => (o.STATUS_CODE == "5" || o.STATUS_CODE == "6") && o.DEALER == dealercode).ToList();

                if (list != null)
                {
                    List<ROCaseModel> items = new List<ROCaseModel>();
                    foreach (var row in list)
                    {
                        int caseid = row.id;
                        List<Operation> mdl = new List<Operation>();
                        var ol = entity.ro_operation.Where(o => o.case_id == caseid).ToList();
                        if (ol != null)
                        {
                            if (ol.Count > 0)
                            {
                                foreach (var oli in ol)
                                {

                                    Operation opmodel = new Operation()
                                    {
                                        OUT_COMMANDCODE = oli.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = oli.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = oli.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = oli.OUT_OPTCODE,
                                        OUT_OPT_DESC = oli.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = oli.OUT_EXPENSE_TYPE,
                                    };

                                    mdl.Add(opmodel);
                                }
                                items.Add(new ROCaseModel
                                {

                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    operation = mdl,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }
                            else
                            {
                                items.Add(new ROCaseModel
                                {
                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }

                        }

                    }








                    return Json(items);
                }
                else { return Json(""); }


            }
        }
        // GET api/blog/5

        [HttpGet]
        [Route("api/case/status/pending/{dealercode}")]
        public IHttpActionResult GetNewListCaseByStatus(string dealercode)
        {
            //var PendingStatus = new string[] { "0", "1", "2","3","4" };
            using (mmthapiEntities entity = new mmthapiEntities())
            {

                var list = (dealercode == "admin" || dealercode == null) ? entity.vRoCases.Where(o => (o.STATUS_CODE == "0" || o.STATUS_CODE == "1" || o.STATUS_CODE == "2" || o.STATUS_CODE == "3" )).ToList()
                    : entity.vRoCases.Where(o => (o.STATUS_CODE == "0" || o.STATUS_CODE == "1" || o.STATUS_CODE == "2" || o.STATUS_CODE == "3" ) && o.DEALER == dealercode).ToList();

                if (list != null)
                {
                    List<ROCaseModel> items = new List<ROCaseModel>();
                    foreach (var row in list)
                    {
                        int caseid = row.id;
                        List<Operation> mdl = new List<Operation>();
                        var ol = entity.ro_operation.Where(o => o.case_id == caseid).ToList();
                        if (ol != null)
                        {
                            if (ol.Count > 0)
                            {
                                foreach (var oli in ol)
                                {

                                    Operation opmodel = new Operation()
                                    {
                                        OUT_COMMANDCODE = oli.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = oli.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = oli.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = oli.OUT_OPTCODE,
                                        OUT_OPT_DESC = oli.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = oli.OUT_EXPENSE_TYPE,
                                    };

                                    mdl.Add(opmodel);
                                }
                                items.Add(new ROCaseModel
                                {

                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    operation = mdl,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,

                                    Problem = row.Problem==null?"": row.Problem,
                                    TimeOccur = row.TimeOccur == null ? "" : row.TimeOccur,
                                    TimeFreq = row.TimeFreq == null ? "" : row.TimeFreq,
                                    TimeFreqAmount = row.TimeFreqAmount == null ? 0: row.TimeFreqAmount,
                                    TimeFreqIn = row.TimeFreqIn == null ? "" : row.TimeFreqIn,
                                    Weather = row.Weather == null ? "" : row.Weather,
                                    WeatherOther = row.WeatherOther == null ? "" : row.WeatherOther,
                                    RoadCondition = row.RoadCondition == null ? "" : row.RoadCondition,
                                    RoadConditionOther = row.RoadConditionOther == null ? "" : row.RoadConditionOther,
                                    RoadFloor = row.RoadFloor == null ? "" : row.RoadFloor,
                                    RoadFloorOther = row.RoadFloorOther == null ? "" : row.RoadFloorOther,
                                    Gear = row.Gear == null ? "" : row.Gear,
                                    GearOther = row.GearOther == null ? "" : row.GearOther,
                                    Tire = row.Tire == null ? "" : row.Tire,
                                    TireOther = row.TireOther == null ? "" : row.TireOther,
                                    Tread = row.Tread == null ? "" : row.Tread,
                                    TreadOther = row.TreadOther == null ? "" : row.TreadOther,
                                    MaintenanceHistory = row.MaintenanceHistory == null ? "" : row.MaintenanceHistory,
                                    MaintenanceHistoryOther = row.MaintenanceHistoryOther == null ? "" : row.MaintenanceHistoryOther,
                                    Accident = row.Accident == null ? "" : row.Accident,
                                    AccidentOther = row.AccidentOther == null ? "" : row.AccidentOther,
                                    TransformCar = row.TransformCar == null ? "" : row.TransformCar,
                                    TransformCarOther = row.TransformCarOther == null ? "" : row.TransformCarOther,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,


                                }
);
                            }
                            else
                            {
                                items.Add(new ROCaseModel
                                {
                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,

                                    Problem = row.Problem == null ? "" : row.Problem,
                                    TimeOccur = row.TimeOccur == null ? "" : row.TimeOccur,
                                    TimeFreq = row.TimeFreq == null ? "" : row.TimeFreq,
                                    TimeFreqAmount = row.TimeFreqAmount == null ? 0 : row.TimeFreqAmount,
                                    TimeFreqIn = row.TimeFreqIn == null ? "" : row.TimeFreqIn,
                                    Weather = row.Weather == null ? "" : row.Weather,
                                    WeatherOther = row.WeatherOther == null ? "" : row.WeatherOther,
                                    RoadCondition = row.RoadCondition == null ? "" : row.RoadCondition,
                                    RoadConditionOther = row.RoadConditionOther == null ? "" : row.RoadConditionOther,
                                    RoadFloor = row.RoadFloor == null ? "" : row.RoadFloor,
                                    RoadFloorOther = row.RoadFloorOther == null ? "" : row.RoadFloorOther,
                                    Gear = row.Gear == null ? "" : row.Gear,
                                    GearOther = row.GearOther == null ? "" : row.GearOther,
                                    Tire = row.Tire == null ? "" : row.Tire,
                                    TireOther = row.TireOther == null ? "" : row.TireOther,
                                    Tread = row.Tread == null ? "" : row.Tread,
                                    TreadOther = row.TreadOther == null ? "" : row.TreadOther,
                                    MaintenanceHistory = row.MaintenanceHistory == null ? "" : row.MaintenanceHistory,
                                    MaintenanceHistoryOther = row.MaintenanceHistoryOther == null ? "" : row.MaintenanceHistoryOther,
                                    Accident = row.Accident == null ? "" : row.Accident,
                                    AccidentOther = row.AccidentOther == null ? "" : row.AccidentOther,
                                    TransformCar = row.TransformCar == null ? "" : row.TransformCar,
                                    TransformCarOther = row.TransformCarOther == null ? "" : row.TransformCarOther,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }

                        }

                    }








                    return Json(items);
                }
                else { return Json(""); }


            }
        }
        [HttpGet]
        [Route("api/case/status/completed/{dealercode}")]
        public IHttpActionResult GetNewListCaseByStatusCompleted(string dealercode)
        {
            //var PendingStatus = new string[] { "0", "1", "2","3","4" };
            using (mmthapiEntities entity = new mmthapiEntities())
            {

                var list = (dealercode == "admin" || dealercode == null) ? entity.vRoCases.Where(o => (o.STATUS_CODE == "5" || o.STATUS_CODE == "6" || o.STATUS_CODE == "4" || o.STATUS_CODE == "7")).ToList() : entity.vRoCases.Where(o => (o.STATUS_CODE == "5" || o.STATUS_CODE == "6" || o.STATUS_CODE == "4" || o.STATUS_CODE == "7") && o.DEALER == dealercode).ToList();

                if (list != null)
                {
                    List<ROCaseModel> items = new List<ROCaseModel>();
                    foreach (var row in list)
                    {
                        int caseid = row.id;
                        List<Operation> mdl = new List<Operation>();
                        var ol = entity.ro_operation.Where(o => o.case_id == caseid).ToList();
                        if (ol != null)
                        {
                            if (ol.Count > 0)
                            {
                                foreach (var oli in ol)
                                {

                                    Operation opmodel = new Operation()
                                    {
                                        OUT_COMMANDCODE = oli.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = oli.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = oli.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = oli.OUT_OPTCODE,
                                        OUT_OPT_DESC = oli.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = oli.OUT_EXPENSE_TYPE,
                                    };

                                    mdl.Add(opmodel);
                                }
                                items.Add(new ROCaseModel
                                {

                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    operation = mdl,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    Problem = row.Problem == null ? "" : row.Problem,
                                    TimeOccur = row.TimeOccur == null ? "" : row.TimeOccur,
                                    TimeFreq = row.TimeFreq == null ? "" : row.TimeFreq,
                                    TimeFreqAmount = row.TimeFreqAmount == null ? 0 : row.TimeFreqAmount,
                                    TimeFreqIn = row.TimeFreqIn == null ? "" : row.TimeFreqIn,
                                    Weather = row.Weather == null ? "" : row.Weather,
                                    WeatherOther = row.WeatherOther == null ? "" : row.WeatherOther,
                                    RoadCondition = row.RoadCondition == null ? "" : row.RoadCondition,
                                    RoadConditionOther = row.RoadConditionOther == null ? "" : row.RoadConditionOther,
                                    RoadFloor = row.RoadFloor == null ? "" : row.RoadFloor,
                                    RoadFloorOther = row.RoadFloorOther == null ? "" : row.RoadFloorOther,
                                    Gear = row.Gear == null ? "" : row.Gear,
                                    GearOther = row.GearOther == null ? "" : row.GearOther,
                                    Tire = row.Tire == null ? "" : row.Tire,
                                    TireOther = row.TireOther == null ? "" : row.TireOther,
                                    Tread = row.Tread == null ? "" : row.Tread,
                                    TreadOther = row.TreadOther == null ? "" : row.TreadOther,
                                    MaintenanceHistory = row.MaintenanceHistory == null ? "" : row.MaintenanceHistory,
                                    MaintenanceHistoryOther = row.MaintenanceHistoryOther == null ? "" : row.MaintenanceHistoryOther,
                                    Accident = row.Accident == null ? "" : row.Accident,
                                    AccidentOther = row.AccidentOther == null ? "" : row.AccidentOther,
                                    TransformCar = row.TransformCar == null ? "" : row.TransformCar,
                                    TransformCarOther = row.TransformCarOther == null ? "" : row.TransformCarOther,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }
                            else
                            {
                                items.Add(new ROCaseModel
                                {
                                    Id = row.id,
                                    CaseId = row.CASEID,
                                    Dealer = row.DEALER,
                                    Out_offcde = row.OUT_OFFCDE,
                                    Out_cmpcde = row.OUT_CMPCDE,
                                    Out_rocode = row.OUT_ROCODE,
                                    Out_cust_date = row.OUT_CUST_DATE,
                                    Out_ro_status = row.OUT_RO_STATUS,
                                    Out_rodate = row.OUT_RODATE,
                                    Out_rotime = row.OUT_ROTIME,
                                    Out_warranty_date = row.OUT_WARRANTY_DATE,
                                    Out_expiry_date = row.OUT_EXPIRY_DATE,
                                    Out_license = row.OUT_LICENSE,
                                    Out_prdcde = row.OUT_PRDCDE,
                                    Out_chasno = row.OUT_CHASNO,
                                    Out_engno = row.OUT_ENGNO,
                                    Out_model = row.OUT_MODEL,
                                    Out_kilo_last = row.OUT_KILO_LAST,
                                    Out_last_date = row.OUT_LAST_DATE,
                                    Out_idno = row.OUT_IDNO,
                                    Out_cusname = row.OUT_CUSNAME,
                                    Out_mobile = row.OUT_MOBILE,
                                    Out_address = row.OUT_ADDRESS,
                                    Out_province = row.OUT_PROVINCE,
                                    Out_zipcode = row.OUT_ZIPCODE,
                                    Out_custype = row.OUT_CUSTYPE,
                                    A_code = row.A_CODE,
                                    B_code = row.B_CODE,
                                    C_code = row.C_CODE,
                                    LevelofProblem = row.LevelofProblem,
                                    CaseDescription = row.CaseDescription,
                                    CaseTitle = row.CaseTitle,
                                    CaseSubject = row.CaseSubject,
                                    CaseType = row.CaseType,
                                    StatusCode = row.STATUS_CODE,
                                    StatusCodeText = row.statusCodeText,
                                    ModifiedOn = row.MODIFIED_ON,
                                    CreatedOn = row.CREATED_ON,
                                    CreatedBy = row.CREATED_BY,
                                    ModifiedBy = row.MODIFIED_BY,
                                    Problem = row.Problem == null ? "" : row.Problem,
                                    TimeOccur = row.TimeOccur == null ? "" : row.TimeOccur,
                                    TimeFreq = row.TimeFreq == null ? "" : row.TimeFreq,
                                    TimeFreqAmount = row.TimeFreqAmount == null ? 0 : row.TimeFreqAmount,
                                    TimeFreqIn = row.TimeFreqIn == null ? "" : row.TimeFreqIn,
                                    Weather = row.Weather == null ? "" : row.Weather,
                                    WeatherOther = row.WeatherOther == null ? "" : row.WeatherOther,
                                    RoadCondition = row.RoadCondition == null ? "" : row.RoadCondition,
                                    RoadConditionOther = row.RoadConditionOther == null ? "" : row.RoadConditionOther,
                                    RoadFloor = row.RoadFloor == null ? "" : row.RoadFloor,
                                    RoadFloorOther = row.RoadFloorOther == null ? "" : row.RoadFloorOther,
                                    Gear = row.Gear == null ? "" : row.Gear,
                                    GearOther = row.GearOther == null ? "" : row.GearOther,
                                    Tire = row.Tire == null ? "" : row.Tire,
                                    TireOther = row.TireOther == null ? "" : row.TireOther,
                                    Tread = row.Tread == null ? "" : row.Tread,
                                    TreadOther = row.TreadOther == null ? "" : row.TreadOther,
                                    MaintenanceHistory = row.MaintenanceHistory == null ? "" : row.MaintenanceHistory,
                                    MaintenanceHistoryOther = row.MaintenanceHistoryOther == null ? "" : row.MaintenanceHistoryOther,
                                    Accident = row.Accident == null ? "" : row.Accident,
                                    AccidentOther = row.AccidentOther == null ? "" : row.AccidentOther,
                                    TransformCar = row.TransformCar == null ? "" : row.TransformCar,
                                    TransformCarOther = row.TransformCarOther == null ? "" : row.TransformCarOther,
                                    CaseSubjectName = row.subject,
                                    MicrosoftTeamLink = row.MicrosoftTeamLink,
                                    SolutionForDealer = row.SolutionForDealer,
                                    TafNo = row.tafno,
                                    TafYear = row.tafyear,
                                }
);
                            }

                        }

                    }








                    return Json(items);
                }
                else { return Json(""); }


            }
        }
        // GET api/blog/5
        [HttpPost]
        [Route("api/ROCase/rocheck")]
        public ResultModel GetRONumber([FromBody] Rocode rcode)
        {


            //WS_InterfaceCRMSoapClient soapClient = new WS_InterfaceCRMSoapClient(TechLineCaseAPI.Properties.Settings.Default.RO_WS);
            //WS_InterfaceCRMSoapClient ws = new WS_InterfaceCRMSoapClient();
            WS_InterfaceCRMSoapClient soapClient = new WS_InterfaceCRMSoapClient("WS_InterfaceCRMSoap", "http://pre-mmth.dms-ccp.com/WS_InterfaceCRM/WS_InterfaceCRM.asmx");

            using (new OperationContextScope(soapClient.InnerChannel))
            {
                //Create message header containing the credentials
                //var header = MessageHeader.CreateHeader("SC_Credentials",
                // "http://soapservice.com", credentials, new CFMessagingSerializer(typeof(SC_Credentials)));
                //Add the credentials message header to the outgoing request
                // OperationContext.Current.OutgoingMessageHeaders.Add(header);

                try
                {
                    WSCheckHistoryRORequest wr = new WSCheckHistoryRORequest();
                    wr.CMPCDE = "110059";
                    wr.OFFCDE = "110059";
                    wr.RONO = rcode.rocode;
                    wr.REQUEST_NO = "123456789012345678901234567890";
                    WSCheckHistoryROResponse resp = soapClient.WSCheckHistoryRO(wr);
                    // var result = Task.Run(async () => soapClient.WSCheckHistoryROAsync(wr)).GetAwaiter().GetResult();

                    DataTable cv = resp.WSCheckHistoryROResult;




                    IList<ROCaseModel> items = cv.AsEnumerable().Select(row =>
              new ROCaseModel
              {
                  Out_offcde = row.Field<string>("OUT_CMPCDE"),
                  Out_cmpcde = row.Field<string>("OUT_OFFCDE"),
                  Out_rocode = row.Field<string>("OUT_ROCODE"),
                  Out_cust_date = row.Field<string>("OUT_CUST_DATE"),
                  Out_ro_status = row.Field<string>("OUT_RO_STATUS"),
                  Out_rodate = row.Field<string>("OUT_RODATE"),
                  Out_rotime = row.Field<string>("OUT_ROTIME"),
                  Out_warranty_date = row.Field<string>("OUT_WARRANTY_DATE"),
                  Out_expiry_date = row.Field<string>("OUT_EXPIRY_DATE"),
                  Out_license = row.Field<string>("OUT_LICENSE"),
                  Out_prdcde = row.Field<string>("OUT_PRDCDE"),
                  Out_chasno = row.Field<string>("OUT_CHASNO"),
                  Out_engno = row.Field<string>("OUT_ENGNO"),
                  Out_model = row.Field<string>("OUT_MODEL"),
                  Out_kilo_last = row.Field<string>("OUT_KILO_LAST"),
                  Out_last_date = row.Field<string>("OUT_LAST_DATE"),
                  Out_idno = row.Field<string>("OUT_IDNO"),
                  Out_cusname = row.Field<string>("OUT_CUSNAME"),
                  Out_mobile = row.Field<string>("OUT_MOBILE"),
                  Out_address = row.Field<string>("OUT_ADDRESS"),
                  Out_province = row.Field<string>("OUT_PROVINCE"),
                  Out_zipcode = row.Field<string>("OUT_ZIPCODE"),
                  Out_custype = row.Field<string>("OUT_CUSTYPE"),

                  OUT_SYS_CODE = row.Field<string>("OUT_SYS_CODE"),
                  OUT_SYS_STS = row.Field<string>("OUT_SYS_STS"),
                  OUT_SYS_MSG = row.Field<string>("OUT_SYS_MSG"),

              }).ToList();
                    if (items[0].OUT_SYS_CODE == "E001")
                    {
                        return new ResultModel()
                        {
                            Status = "S",
                            Message = items[0].OUT_SYS_MSG,
                            Result = items,
                        };
                    }
                    else
                    {
                        return new ResultModel()
                        {
                            Status = "E",
                            Message = items[0].OUT_SYS_MSG,
                            Result = items,
                        };
                    }

                }
                catch (Exception ex)
                {
                    return new ResultModel()
                    {
                        Status = "S",
                        Message = "Create Completed",

                    };
                    //throw;
                }
            }




        }

        public  string ConvertDate(string date)
        {
            if (date.Length == 8)
            {
                DateTime dt = DateTime.ParseExact(date, "ddMMyyyy", null);
                return dt.ToString("dd/MM/yyyy");
            }
            else
            {
               
                return "";
            }
            
        }
        public string ConvertDateYYYYMMDD(string date)
        {
            if (date.Length == 8)
            {
                DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", null);
                return dt.ToString("dd/MM/yyyy");
            }
            else
            {

                return "";
            }

        }

        [HttpPost]
        [Route("api/ROCase/rocheckdealer")]
        public ResultModel GetRONumberByDealer([FromBody] RocodeOperation rcode)
        {
            WS_InterfaceCRMSoapClient soapClient = new WS_InterfaceCRMSoapClient("WS_InterfaceCRMSoap", TechLineCaseAPI.Properties.Settings.Default.WS_InterfaceCRMSoap);

            using (new OperationContextScope(soapClient.InnerChannel))
            {
                try
                {
                    WSCheckHistoryRORequest wr = new WSCheckHistoryRORequest();
                    wr.CMPCDE = rcode.dealercode;
                    wr.OFFCDE = rcode.offdealercode;
                    wr.RONO = rcode.rocode;
                    wr.REQUEST_NO = rcode.requestno;
                    //wr.REQUEST_NO = "123456789012345678901234567890";
                    WSCheckHistoryROResponse resp = soapClient.WSCheckHistoryRO(wr);
                    // var result = Task.Run(async () => soapClient.WSCheckHistoryROAsync(wr)).GetAwaiter().GetResult();

                    DataTable cv = resp.WSCheckHistoryROResult;
                    IList<ROCaseModel> items = cv.AsEnumerable().Select(row =>
              new ROCaseModel
              {
                  Out_offcde = row.Field<string>("OUT_CMPCDE"),
                  Out_cmpcde = row.Field<string>("OUT_OFFCDE"),
                  Out_rocode = row.Field<string>("OUT_ROCODE"),
                  Out_cust_date = row.Field<string>("OUT_CUST_DATE"),
                  Out_ro_status = row.Field<string>("OUT_RO_STATUS"),
                  Out_rodate = ConvertDate(row.Field<string>("OUT_RODATE")),
                  Out_rotime = row.Field<string>("OUT_ROTIME"),
                  Out_warranty_date = ConvertDate(row.Field<string>("OUT_WARRANTY_DATE")),
                  Out_expiry_date = ConvertDate(row.Field<string>("OUT_EXPIRY_DATE")),
                  Out_license = row.Field<string>("OUT_LICENSE"),
                  Out_prdcde = row.Field<string>("OUT_PRDCDE"),
                  Out_chasno = row.Field<string>("OUT_CHASNO"),
                  Out_engno = row.Field<string>("OUT_ENGNO"),
                  Out_model = row.Field<string>("OUT_MODEL"),
                  Out_kilo_last = row.Field<string>("OUT_KILO_LAST"),
                  Out_last_date = ConvertDateYYYYMMDD(row.Field<string>("OUT_LAST_DATE")),
                  Out_idno = row.Field<string>("OUT_IDNO"),
                  Out_cusname = row.Field<string>("OUT_CUSNAME"),
                  Out_mobile = row.Field<string>("OUT_MOBILE"),
                  Out_address = row.Field<string>("OUT_ADDRESS"),
                  Out_province = row.Field<string>("OUT_PROVINCE"),
                  Out_zipcode = row.Field<string>("OUT_ZIPCODE"),
                  Out_custype = row.Field<string>("OUT_CUSTYPE"),

                  OUT_SYS_CODE = row.Field<string>("OUT_SYS_CODE"),
                  OUT_SYS_STS = row.Field<string>("OUT_SYS_STS"),
                  OUT_SYS_MSG = row.Field<string>("OUT_SYS_MSG"),

              }).ToList();

                    IList<Operation> detail = cv.AsEnumerable().Select(row =>
      new Operation
      {
          OUT_COMMANDCODE = row.Field<string>("OUT_COMMANDCODE"),
          OUT_COMMANDDESC = row.Field<string>("OUT_COMMANDDESC"),
          OUT_SERVICE_TYPE = row.Field<string>("OUT_SERVICE_TYPE"),
          OUT_OPTCODE = row.Field<string>("OUT_OPTCODE"),
          OUT_OPT_DESC = row.Field<string>("OUT_OPT_DESC"),
          OUT_EXPENSE_TYPE = row.Field<string>("OUT_EXPENSE_TYPE"),
      }).ToList();


                    if (items[0].OUT_SYS_STS == "S")
                    {
                        items[0].operation = detail;
                        return new ResultModel()
                        {

                            Status = "S",
                            Message = items[0].OUT_SYS_MSG,
                            Result = items,
                        };
                    }
                    else
                    {
                        return new ResultModel()
                        {
                            Status = "E",
                            Message = items[0].OUT_SYS_MSG,
                            Result = items,
                        };
                    }

                }
                catch (Exception ex)
                {
                    return new ResultModel()
                    {
                        Status = "E",
                        Message = "Error"+ex.ToString(),

                    };
                    //throw;
                }
            }
        }
        private bool IsExistingRO(string rcode)
        {
            bool bExist = false;
            try
            {
                //string conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;
                string conString = "";

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringHMS;

                }
                else
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;

                }


                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "SELECT TOP 1 * FROM ro_case  where out_rocode='" + rcode + "' ",
                        connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            bExist = reader.HasRows;
                        }

                    }
                    connection.Close();
                    return bExist;


                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        [HttpPost]
        [Route("api/ROCase/techlinevalid")]
        public ResultModel techlinevalid([FromBody] Rocode rcode)
        {



            try
            {

                if (IsExistingRO(rcode.rocode))
                {

                    return new ResultModel()
                    {
                        Status = "E",
                        Message = "Duplicate RO",

                    };
                }
                else
                {
                    return new ResultModel()
                    {
                        Status = "S",
                        Message = "Can Create",

                    };
                }

            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Status = "S",
                    Message = "Create Completed",

                };
                //throw;
            }





        }

        [HttpPost]
        [Route("api/rocase/create")]
        public ResultModel Post([FromBody] ROCaseModel model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {

                var js = new JavaScriptSerializer();
                if (model.CaseId == null) new ResultMessage() { Status = "E", Message = "Require Case Id" };
                if (model.Dealer == null) new ResultMessage() { Status = "E", Message = "Require Dealer" };
                if (model.CreatedBy == null) new ResultMessage() { Status = "E", Message = "Require Created By" };
                ro_case rcase = CreateROCase(model);
                model.Id = rcase.id;
                model.TafNo = rcase.tafno.Trim();
                if (rcase != null)
                {
                    
                    if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                    {
                        //var crmCase = CreateCRMCase(model);
                       // sb.AppendLine("TechLineCaseAPI.Properties.Settings.Default.DEV=False");
                        if (model.operation != null)
                        {
                            using (mmthapiEntities entity = new mmthapiEntities())
                            {
                                foreach (var op in model.operation)
                                {
                                    var record = new ro_operation()
                                    {
                                        case_id = rcase.id,
                                        OUT_COMMANDCODE = op.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = op.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = op.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = op.OUT_OPTCODE,
                                        OUT_OPT_DESC = op.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = op.OUT_EXPENSE_TYPE,

                                        CREATED_BY = model.CreatedBy,
                                        CREATED_ON = DateTime.Now,
                                        MODIFIED_BY = model.CreatedBy,
                                        MODIFIED_ON = DateTime.Now,
                                        STATUS_CODE = "1",
                                    };

                                    entity.ro_operation.AddObject(record);
                                    entity.SaveChanges();
                                    entity.Refresh(RefreshMode.StoreWins, record);

                                    var myid = record.id;
                                }

                            }
                        }


                       // var crmCase = CreateCRMCase(model);
                        if (model.StatusCode == "1")
                        {
                            var crmCase = CreateCRMCase(model);
                            //sb.AppendLine(crmCase.Message);
                            //sb.AppendLine("-----------");
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = crmCase.Result.ToString() };
                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(model);
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };

                           // return new ResultModel() { Status = "S", Message = "Create Complete", Result = model.TafNo.ToString() };
                        }
                        //sb.AppendLine(crmCase.Message);
                        //sb.AppendLine("-----------");
                       // return new ResultModel() { Status = "S", Message = "Create Complete", Result = crmCase.Result.ToString() };


                        /*
                        if (UpdateROCase("DEV0000" + rocaseid, model.Out_rocode))
                        {
                            model.CaseId = "DEV0000" + rocaseid;
                            var json = JsonConvert.SerializeObject(model);
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };
                        }
                        else
                        {
                            return new ResultModel() { Status = "E", Message = "ไม่มี Dealer code" };
                        }*/
                    }
                    else
                    {
                        if (model.operation != null)
                        {
                            using (mmthapiEntities entity = new mmthapiEntities())
                            {
                                foreach (var op in model.operation)
                                {
                                    var record = new ro_operation()
                                    {
                                        case_id = rcase.id,
                                        OUT_COMMANDCODE = op.OUT_COMMANDCODE,
                                        OUT_COMMANDDESC = op.OUT_COMMANDDESC,
                                        OUT_SERVICE_TYPE = op.OUT_SERVICE_TYPE,
                                        OUT_OPTCODE = op.OUT_OPTCODE,
                                        OUT_OPT_DESC = op.OUT_OPT_DESC,
                                        OUT_EXPENSE_TYPE = op.OUT_EXPENSE_TYPE,

                                        CREATED_BY = model.CreatedBy,
                                        CREATED_ON = DateTime.Now,
                                        MODIFIED_BY = model.CreatedBy,
                                        MODIFIED_ON = DateTime.Now,
                                        STATUS_CODE = "1",
                                    };

                                    entity.ro_operation.AddObject(record);
                                    entity.SaveChanges();
                                    entity.Refresh(RefreshMode.StoreWins, record);

                                    var myid = record.id;
                                }

                            }
                        }
                        //sb.AppendLine("TechLineCaseAPI.Properties.Settings.Default.DEV=True");
                        //sb.AppendLine("-----------");
                        if (model.StatusCode == "1")
                        {
                            var crmCase = CreateCRMCase(model);
                            //sb.AppendLine(crmCase.Message);
                            //sb.AppendLine("-----------");
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = crmCase.Result.ToString() };
                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(model);
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };
                        }
                       

                    }


                }
                else
                {
                    return new ResultModel()
                    {
                        Status = "E",
                        Message = "Create Incompleted",
                        Result = 0,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Status = "E",
                    Message = "Create Incompleted (" + ex.Message + "----" + sb.ToString() + ")",
                    Result = 0,
                };
            }
        }

        [HttpPost]
        [Route("api/rocase/update")]
        public ResultModel Update([FromBody] ROCaseModel model)
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(model);
                if (UpdateROCase(model))
                {
                    if (model.StatusCode == "1")
                    {
                        if (model.CaseId == "")
                        {
                            var crmCase = CreateCRMCase(model);
                            //sb.AppendLine(crmCase.Message);
                            //sb.AppendLine("-----------");
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = crmCase.Result.ToString() };
                        }

                    }
                       
                    else
                    {
                        
                        return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };
                    }


                    string statusHeader = "Techline เปลี่ยนสถานะเป็น " + model.StatusCode;
                    string statusBody = "Techline เปลี่ยนสถานะเป็น " + model.StatusCode;
                    if (model.StatusCode == "2" || model.StatusCode == "3" || model.StatusCode == "4" || model.StatusCode == "5" || model.StatusCode == "6" || model.StatusCode == "7")
                    {
                        SendUpdateStatus(statusBody, statusHeader);
                    }
                    
                            return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };
                }
                else
                {
                    return new ResultModel()
                    {
                        Status = "E",
                        Message = "Update Incompleted"
                        ,
                        Result = json
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Status = "E",
                    Message = "Update Incompleted (" + ex.Message + ")"
                };
            }
        }
        public string GetStatusText(string statuscode)
        {

          switch (statuscode)
            {
                case "0":
                    return "Draft";
                case "1":
                    return "Send to Techline";

                case "2":
                    return "Tech-line Onprocess";
                case "3":
                    return "Scramble onprocess";
                case "4":
                    return "Dealer Completed";
                case "5":
                    return "Tech-line Completed";
                case "6":
                    return "Scramble Completed";
                case "7":
                    return "Closed";
                default: return "Closed";
            }
            
        }

        [HttpPost]
        [Route("api/rocase/updatecrm")]
        public ResultMessage UpdateCRM()
        {
            try
            {
                var js = new JavaScriptSerializer();
                 var json = HttpContext.Current.Request.Form["Model"];

                 ROCaseModel model = js.Deserialize<ROCaseModel>(json);

                if (model.Id == null) new ResultMessage() { Status = "E", Message = "Require RO Case Id" };
                if (model.StatusCode == null) new ResultMessage() { Status = "E", Message = "Require Status Code" };
                if (model.ModifiedBy == null) new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateROCase(model))
                {
                    string statusHeader = "มีข้อความใหม่จาก Techline";
                    string statusBody = "Techline เปลี่ยนสถานะเป็น " + GetStatusText(model.StatusCode);
                    string solutionForDealer = "การแก้ไข  :" + model.SolutionForDealer;
                    string link = "meeting Link  :" + model.MicrosoftTeamLink;
                    statusBody = statusBody + "\n" + solutionForDealer + "\n" + link;

                    if (model.StatusCode == "2" || model.StatusCode == "3" || model.StatusCode == "4" || model.StatusCode == "5" || model.StatusCode == "6" || model.StatusCode == "7")
                    {
                        SendUpdateStatus(statusHeader,statusBody);
                    }

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
        [Route("api/rocase/update/status")]
        public ResultMessage UpdateStatus()
        {
            try
            {
                var js = new JavaScriptSerializer();
                var json = HttpContext.Current.Request.Form["Model"];

                ROCaseModel model = js.Deserialize<ROCaseModel>(json);

                if (model.Id == null) new ResultMessage() { Status = "E", Message = "Require RO Case Id" };
                if (model.CaseId == null) new ResultMessage() { Status = "E", Message = "Require Case Id" };
                if (model.StatusCode == null) new ResultMessage() { Status = "E", Message = "Require Status Code" };
                if (model.ModifiedBy == null) new ResultMessage() { Status = "E", Message = "Require Modified By" };

                if (UpdateROCaseStatus(model))
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
        [Route("api/rocase/delete")]
        public ResultMessage Delete()
        {
            var id = HttpContext.Current.Request.Params["id"];

            if (id == null) return new ResultMessage() { Status = "E", Message = "Require Id" };

            if (DeleteROCase(id))
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
        public static void SendUpdateStatus(string Message,string sBody)
        {
            var client = new RestClient(TechLineCaseAPI.Properties.Settings.Default.fcmsend);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", TechLineCaseAPI.Properties.Settings.Default.Authorization);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{" + "\n" +
            @"  ""to"": ""cBWH-nMQTv6qkxFSTyilas:APA91bGRJZ2JTvnxfYKE0ENhmuSL4OLE399OyyxbVv72fNnX9S4gGe7NMbW5R_xmfyTZJiFQSZDtPgkK5PULCZndU_3z39bF8p_7M58n_-Rsk1DcfrM_TcsOl6tYBvID7tA8KfsMuCfV""," + "\n" +
            @"  ""notification"": {" + "\n" +
            @"    ""title"": ""#Message""," + "\n" +
            @"    ""body"": ""#Body""" + "\n" +
            @"  }" + "\n" +
            @"}";
            body=body.Replace("#Message", Message).Replace("#Body", sBody);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        public static ro_case CreateROCase(ROCaseModel model)
        {
            try
            {
                int? rocaseid;
                int? ratingid;
                //  if (model.CaseId == null) return null;
                if (model.Dealer == null) return null;
                if (model.CreatedBy == null) return null;
                var record = new ro_case()
                {
                    CASEID = model.CaseId,
                    DEALER = model.Dealer,
                    OUT_OFFCDE = model.Out_offcde,
                    OUT_CMPCDE = model.Out_cmpcde,
                    OUT_ROCODE = model.Out_rocode,
                    OUT_CUST_DATE = model.Out_cust_date,
                    OUT_RO_STATUS = model.Out_ro_status,
                    OUT_RODATE = model.Out_rodate,
                    OUT_ROTIME = model.Out_rotime,
                    OUT_WARRANTY_DATE = model.Out_warranty_date,
                    OUT_EXPIRY_DATE = model.Out_expiry_date,
                    OUT_LICENSE = model.Out_license,
                    OUT_PRDCDE = model.Out_prdcde,
                    OUT_CHASNO = model.Out_chasno,
                    OUT_ENGNO = model.Out_engno,
                    OUT_MODEL = model.Out_model,
                    OUT_KILO_LAST = model.Out_kilo_last,
                    OUT_LAST_DATE = model.Out_last_date,
                    OUT_IDNO = model.Out_idno,
                    OUT_CUSNAME = model.Out_cusname,
                    OUT_MOBILE = model.Out_mobile,
                    OUT_ADDRESS = model.Out_address,
                    OUT_PROVINCE = model.Out_province,
                    OUT_ZIPCODE = model.Out_zipcode,
                    OUT_CUSTYPE = model.Out_custype,
                    A_CODE = model.A_code,
                    B_CODE = model.B_code,
                    C_CODE = model.C_code,

                    CREATED_BY = model.CreatedBy,
                    CREATED_ON = DateTime.Now,
                    MODIFIED_BY = model.CreatedBy,
                    MODIFIED_ON = DateTime.Now,
                    STATUS_CODE = model.StatusCode,

                    LevelofProblem = model.LevelofProblem,
                    CaseTitle = model.CaseTitle,
                    CaseType = model.CaseType,
                    CaseSubject = model.CaseSubject,
                    CaseDescription = model.CaseDescription,

                    Problem = model.Problem,
                    TimeOccur = model.TimeOccur,
                    TimeFreq = model.TimeFreq,
                    TimeFreqAmount = model.TimeFreqAmount,
                    TimeFreqIn = model.TimeFreqIn,
                    Weather = model.Weather,
                    WeatherOther = model.WeatherOther,
                    RoadCondition = model.RoadCondition,
                    RoadConditionOther = model.RoadConditionOther,
                    RoadFloor = model.RoadFloor,
                    RoadFloorOther = model.RoadFloorOther,
                    Gear = model.Gear,
                    GearOther = model.GearOther,
                    Tire = model.Tire,
                    TireOther = model.TireOther,
                    Tread = model.Tread,
                    TreadOther = model.TreadOther,
                    MaintenanceHistory = model.MaintenanceHistory,
                    MaintenanceHistoryOther = model.MaintenanceHistoryOther,
                    Accident = model.Accident,
                    AccidentOther = model.AccidentOther,
                    TransformCar = model.TransformCar,
                    TransformCarOther = model.TransformCarOther,
                    MicrosoftTeamLink = model.MicrosoftTeamLink,
                    tafyear = DateTime.Now.Year.ToString(),
                    IsRating = "",
                };
                using (mmthapiEntities entity = new mmthapiEntities())
                {                 

                    entity.ro_case.AddObject(record);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    rocaseid = record.id;
                }
                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var atta = entity.attachfiles.Where(o => o.tempkey == model.Out_rocode).ToList();


                    atta.ForEach(a => a.ObjectId = rocaseid);
                    entity.SaveChanges();


                }
                    using (mmthapiEntities roRating = new mmthapiEntities())
                {
                    var ratingr = new rating()
                    {
                        category = "Dealer",
                        ro_caseid= rocaseid,
                        //score = 0,
                        //maxscore = li.maxscore,
                        //ratingid = li.RatingId,

                        CREATED_BY = "1",
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = "1",
                        MODIFIED_ON = DateTime.Now,
                        STATUS_CODE = "1",
                    };
                    roRating.ratings.AddObject(ratingr);
                    roRating.SaveChanges();
                    roRating.Refresh(RefreshMode.StoreWins, ratingr);
                    ratingid = ratingr.id;

                    var item = roRating.rating_master
                        .OrderBy(o => o.order_seq)
                        .ThenBy(o => o.CREATED_ON)
                        .ToList();
                    foreach(var li  in item )
                    {
                        var ratingsub = new rating_subject()
                        {
                            subject = li.subject,
                            score=0,
                            maxscore=li.maxscore,
                            ratingid= ratingid,
                            
                            CREATED_BY = "1",
                            CREATED_ON = DateTime.Now,
                            MODIFIED_BY = "1",
                            MODIFIED_ON = DateTime.Now,
                            STATUS_CODE = "1",
                        };
                        roRating.rating_subject.AddObject(ratingsub);
                    }



                    roRating.SaveChanges();
                    //entity.Refresh(RefreshMode.StoreWins, record);


                    //return Json(item);
                }



                return record;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool UpdateROCase(ROCaseModel model)
        {
            try
            {
                if (model.Id == null) return false;
                if (model.StatusCode == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.ro_case.Where(o => o.id == model.Id).FirstOrDefault();

                    record.CASEID = AssignStringData(record.CASEID, model.CaseId);
                    record.DEALER = AssignStringData(record.DEALER, model.Dealer);
                    record.OUT_OFFCDE = AssignStringData(record.OUT_OFFCDE, model.Out_offcde);
                    record.OUT_CMPCDE = AssignStringData(record.OUT_CMPCDE, model.Out_cmpcde);
                    record.OUT_ROCODE = AssignStringData(record.OUT_ROCODE, model.Out_rocode);
                    record.OUT_CUST_DATE = AssignStringData(record.OUT_CUST_DATE, model.Out_cust_date);
                    record.OUT_RO_STATUS = AssignStringData(record.OUT_RO_STATUS, model.Out_ro_status);
                    record.OUT_RODATE = AssignStringData(record.OUT_RODATE, model.Out_rodate);
                    record.OUT_ROTIME = AssignStringData(record.OUT_ROTIME, model.Out_rotime);
                    record.OUT_WARRANTY_DATE = AssignStringData(record.OUT_WARRANTY_DATE, model.Out_warranty_date);
                    record.OUT_EXPIRY_DATE = AssignStringData(record.OUT_EXPIRY_DATE, model.Out_expiry_date);
                    record.OUT_LICENSE = AssignStringData(record.OUT_LICENSE, model.Out_license);
                    record.OUT_PRDCDE = AssignStringData(record.OUT_PRDCDE, model.Out_prdcde);
                    record.OUT_CHASNO = AssignStringData(record.OUT_CHASNO, model.Out_chasno);
                    record.OUT_ENGNO = AssignStringData(record.OUT_ENGNO, model.Out_engno);
                    record.OUT_MODEL = AssignStringData(record.OUT_MODEL, model.Out_model);
                    record.OUT_KILO_LAST = AssignStringData(record.OUT_KILO_LAST, model.Out_kilo_last);
                    record.OUT_LAST_DATE = AssignStringData(record.OUT_LAST_DATE, model.Out_last_date);
                    record.OUT_IDNO = AssignStringData(record.OUT_IDNO, model.Out_idno);
                    record.OUT_CUSNAME = AssignStringData(record.OUT_CUSNAME, model.Out_cusname);
                    record.OUT_MOBILE = AssignStringData(record.OUT_MOBILE, model.Out_mobile);
                    record.OUT_ADDRESS = AssignStringData(record.OUT_ADDRESS, model.Out_address);
                    record.OUT_PROVINCE = AssignStringData(record.OUT_PROVINCE, model.Out_province);
                    record.OUT_ZIPCODE = AssignStringData(record.OUT_ZIPCODE, model.Out_zipcode);
                    record.OUT_CUSTYPE = AssignStringData(record.OUT_CUSTYPE, model.Out_custype);
                    record.A_CODE = AssignStringData(record.A_CODE, model.A_code);
                    record.B_CODE = AssignStringData(record.B_CODE, model.B_code);
                    record.C_CODE = AssignStringData(record.C_CODE, model.C_code);

                    string sourceStatus = record.STATUS_CODE;
                    //CREATED_BY = model.CreatedBy,
                    //CREATED_ON = DateTime.Now,
                    record.MODIFIED_BY = model.ModifiedBy;
                    record.MODIFIED_ON = DateTime.Now;
                    record.STATUS_CODE = AssignStringData(sourceStatus, model.StatusCode);

                    record.LevelofProblem = AssignStringData(record.LevelofProblem, model.LevelofProblem);
                    record.CaseTitle = AssignStringData(record.CaseTitle, model.CaseTitle);
                    record.CaseType = AssignStringData(record.CaseType, model.CaseType);
                    record.CaseSubject = AssignStringData(record.CaseSubject, model.CaseSubject);
                    record.CaseDescription = AssignStringData(record.CaseDescription, model.CaseDescription);

                    record.Problem = AssignStringData(record.Problem, model.Problem);
                    record.TimeOccur = AssignStringData(record.TimeOccur, model.TimeOccur);
                    record.TimeFreq = AssignStringData(record.TimeFreq, model.TimeFreq);
                    record.TimeFreqAmount = AssignIntData(record.TimeFreqAmount, model.TimeFreqAmount);
                    record.TimeFreqIn = AssignStringData(record.TimeFreqIn, model.TimeFreqIn);
                    record.Weather = AssignStringData(record.Weather, model.Weather);
                    record.WeatherOther = AssignStringData(record.WeatherOther, model.WeatherOther);
                    record.RoadCondition = AssignStringData(record.RoadCondition, model.RoadCondition);
                    record.RoadConditionOther = AssignStringData(record.RoadConditionOther, model.RoadConditionOther);
                    record.RoadFloor = AssignStringData(record.RoadFloor, model.RoadFloor);
                    record.RoadFloorOther = AssignStringData(record.RoadFloorOther, model.RoadFloorOther);
                    record.Gear = AssignStringData(record.Gear, model.Gear);
                    record.GearOther = AssignStringData(record.GearOther, model.GearOther);
                    record.Tire = AssignStringData(record.Tire, model.Tire);
                    record.TireOther = AssignStringData(record.TireOther, model.TireOther);
                    record.Tread = AssignStringData(record.Tread, model.Tread);
                    record.TreadOther = AssignStringData(record.TreadOther, model.TreadOther);
                    record.MaintenanceHistory = AssignStringData(record.MaintenanceHistory, model.MaintenanceHistory);
                    record.MaintenanceHistoryOther = AssignStringData(record.MaintenanceHistoryOther, model.MaintenanceHistoryOther);
                    record.Accident = AssignStringData(record.Accident, model.Accident);
                    record.AccidentOther = AssignStringData(record.AccidentOther, model.AccidentOther);
                    record.TransformCar = AssignStringData(record.TransformCar, model.TransformCar);
                    record.TransformCarOther = AssignStringData(record.TransformCarOther, model.TransformCarOther);
                    record.MicrosoftTeamLink = AssignStringData(record.MicrosoftTeamLink, model.MicrosoftTeamLink);
                    record.SolutionForDealer = AssignStringData(record.SolutionForDealer, model.SolutionForDealer);
                    
                    //entity.ro_case.Attach(record);
                    //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    if (sourceStatus != model.StatusCode)
                    {
                        CreateROCaseLog(record.CASEID, sourceStatus, model.StatusCode, model.ModifiedBy);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateROCaseStatus(ROCaseModel model)
        {
            try
            {
                if (model.Id == null) return false;
                if (model.CaseId == null) return false;
                if (model.StatusCode == null) return false;
                if (model.ModifiedBy == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = entity.ro_case.Where(o => o.id == model.Id).FirstOrDefault();

                    string sourceStatus = record.STATUS_CODE;
                    record.MODIFIED_BY = model.ModifiedBy;
                    record.MODIFIED_ON = DateTime.Now;
                    record.STATUS_CODE = AssignStringData(sourceStatus, model.StatusCode);

                    //entity.ro_case.Attach(record);
                    //entity.ObjectStateManager.ChangeObjectState(record, System.Data.EntityState.Modified);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    if (sourceStatus != model.StatusCode)
                    {
                        CreateROCaseLog(record.CASEID, sourceStatus, model.StatusCode, model.ModifiedBy);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteROCase(string id)
        {
            try
            {
                if (id == null) return false;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    int rocaseid = int.Parse(id);
                    var record = entity.ro_case.Where(o => o.id == rocaseid).FirstOrDefault();

                    if (record != null)
                    {
                        entity.ro_case.Attach(record);
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
                //case "": return null;
                case null: return source;
                default: return data;
            }
        }

        private static int? AssignIntData(int? source, int? data)
        {
            switch (data)
            {
                case -1: return null;
                case null: return source;
                default: return data;
            }
        }
        private Entity getSubject(string subjectid)

        {
            try
            {
                if (_service == null)
                {
                    ConnectToMSCRM();
                }

                var columns = new ColumnSet();
                columns.AllColumns = true;

                FilterExpression filter = new FilterExpression(LogicalOperator.Or);

                FilterExpression filter1 = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("subjectid", ConditionOperator.Equal, new Guid(subjectid)),
                        new ConditionExpression("subhectid", ConditionOperator.NotNull)
                    },
                };



                filter.AddFilter(filter1);
                //filter.AddFilter(filter2);
                //filter.AddFilter(filter3);

                var query = new QueryExpression("subject");
                query.ColumnSet = columns;
                query.Criteria = filter;
                query.TopCount = 1;

                EntityCollection result = _service.RetrieveMultiple(query);

                return (result.Entities.Count > 0) ? result.Entities[0] : null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private Entity getAccount(string accountCode)

        {
            try
            {
                if (_service == null)
                {
                    ConnectToMSCRM();
                }

                var columns = new ColumnSet();
                columns.AllColumns = true;

                FilterExpression filter = new FilterExpression(LogicalOperator.Or);

                FilterExpression filter1 = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("hms_code", ConditionOperator.Equal, accountCode),
                        new ConditionExpression("hms_code", ConditionOperator.NotNull)
                    },
                };



                filter.AddFilter(filter1);
                //filter.AddFilter(filter2);
                //filter.AddFilter(filter3);

                var query = new QueryExpression("account");
                query.ColumnSet = columns;
                query.Criteria = filter;
                query.TopCount = 1;

                EntityCollection result = _service.RetrieveMultiple(query);

                return (result.Entities.Count > 0) ? result.Entities[0] : null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private Entity getCase(Guid caseid)

        {
            try
            {
                if (_service == null)
                {
                    ConnectToMSCRM();
                }

                var columns = new ColumnSet();
                columns.AllColumns = true;

                FilterExpression filter = new FilterExpression(LogicalOperator.Or);

                FilterExpression filter1 = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("incidentid", ConditionOperator.Equal, caseid),

                    },
                };



                filter.AddFilter(filter1);
                //filter.AddFilter(filter2);
                //filter.AddFilter(filter3);

                var query = new QueryExpression("incident");
                query.ColumnSet = columns;
                query.Criteria = filter;
                query.TopCount = 1;

                EntityCollection result = _service.RetrieveMultiple(query);

                return (result.Entities.Count > 0) ? result.Entities[0] : null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private ResultModel CreateCRMCase(ROCaseModel inc)
        {
            Entity entity = new Entity("incident");
            try
            {
                if (_service == null)
                {
                    ConnectToMSCRM();
                }


                //var model = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(myentity);
                var account = getAccount(inc.Out_offcde);
                if (account != null)
                {
                    entity["customerid".ToLower()] = new EntityReference("account", account.Id);
                    entity["hms_outOffcde".ToLower()] = inc.Out_offcde;
                    entity["hms_outCmpcde".ToLower()] = inc.Out_cmpcde;
                    entity["hms_outRocode".ToLower()] = inc.Out_rocode;
                    entity["hms_outCustDate".ToLower()] = inc.Out_cust_date;
                    entity["hms_outRoStatus".ToLower()] = inc.Out_ro_status;
                    entity["hms_outRodate".ToLower()] = inc.Out_rodate;
                    entity["hms_outRotime".ToLower()] = inc.Out_rotime;
                    entity["hms_outWarrantyDate".ToLower()] = inc.Out_warranty_date;
                    entity["hms_outExpiryDate".ToLower()] = inc.Out_expiry_date;
                    entity["hms_outLicense".ToLower()] = inc.Out_license;
                    entity["hms_outChasno".ToLower()] = inc.Out_chasno;
                    entity["hms_outEngno".ToLower()] = inc.Out_engno;
                    entity["hms_outModel".ToLower()] = inc.Out_model;

                    entity["hms_outKiloLast".ToLower()] = inc.Out_kilo_last;
                    entity["hms_outIdno".ToLower()] = inc.Out_idno;
                    entity["hms_outCusname".ToLower()] = inc.Out_cusname;
                    entity["hms_outMobile".ToLower()] = inc.Out_mobile;
                    entity["hms_outAddress".ToLower()] = inc.Out_address;
                    entity["hms_outProvince".ToLower()] = inc.Out_province;
                    entity["hms_outZipcode".ToLower()] = inc.Out_zipcode;
                    entity["hms_outCustype".ToLower()] = inc.Out_custype;
                    entity["hms_aCode".ToLower()] = inc.A_code;
                    entity["hms_bCode".ToLower()] = inc.B_code;
                    entity["hms_cCode".ToLower()] = inc.C_code;
                    entity["hms_levelofProblem".ToLower()] = inc.LevelofProblem;
                    entity["hms_caseTypetext".ToLower()] = inc.CaseType;
                    //entity["hms_caseSubject".ToLower()] = inc.CaseSubject;
                    // entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    entity["hms_caseDescription".ToLower()] = inc.CaseDescription;

                    //entity["hms_statusCode".ToLower()] = inc.StatusCode;
                    //    entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    entity["hms_levelofProblem".ToLower()] = inc.LevelofProblem;

                    //entity["hms_caseType".ToLower()] = inc.CaseType;
                    entity["hms_caseSubject".ToLower()] = inc.CaseSubject;
                    // entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    //entity["hms_caseDescription".ToLower()] = inc.CaseDescription;

                    entity["title".ToLower()] = inc.CaseTitle;

                    entity["hms_background".ToLower()] = inc.CaseDescription;
                    int value = int.Parse(inc.LevelofProblem == null ? "177980000" : inc.LevelofProblem);
                    entity["hms_lop".ToLower()] = new OptionSetValue(value);
                    
                    entity["hms_roProblem".ToLower()] = inc.Problem;
                    entity["hms_roTimeOccur".ToLower()] = GlobalParam.TimeOccurList.Find(item => item.Code == inc.TimeOccur).Name ;
                    entity["hms_roTimeFreq".ToLower()] = GlobalParam.TimeFreqList.Find(item => item.Code == inc.TimeFreq).Name;//= inc.TimeFreq;
                    entity["hms_roTimeFreqAmount".ToLower()] = inc.TimeFreqAmount+"";
                    entity["hms_roTimeFreqIn".ToLower()] = inc.TimeFreqIn;
                    entity["hms_roWeather".ToLower()] = GlobalParam.WhetherList.Find(item => item.Code == inc.Weather).Name;// inc.Weather;
                    entity["hms_roWeatherOther".ToLower()] = inc.WeatherOther;
                    entity["hms_roRoadCondition".ToLower()] = GlobalParam.RoadConditionList.Find(item => item.Code == inc.RoadCondition).Name;// inc.RoadCondition;
                    entity["hms_roRoadConditionOther".ToLower()] = inc.RoadConditionOther;
                    entity["hms_roRoadFloor".ToLower()] = GlobalParam.RoadFloorList.Find(item => item.Code == inc.RoadFloor).Name;//inc.RoadFloor;
                    entity["hms_roRoadFloorOther".ToLower()] = inc.RoadFloorOther;
                    entity["hms_roGear".ToLower()] = GlobalParam.GearList.Find(item => item.Code == inc.Gear).Name;// inc.Gear;
                    entity["hms_roGearOther".ToLower()] = inc.GearOther;
                    entity["hms_roTire".ToLower()] = GlobalParam.TireList.Find(item => item.Code == inc.Tire).Name;//inc.Tire;
                    entity["hms_roTireOther".ToLower()] = inc.TireOther;
                    entity["hms_roTread".ToLower()] = GlobalParam.TreadList.Find(item => item.Code == inc.Tread).Name;//  inc.Tread;
                    entity["hms_roTreadOther".ToLower()] = inc.TreadOther;
                    entity["hms_roMaintenanceHistory".ToLower()] = GlobalParam.MaintenanceHistoryList.Find(item => item.Code == inc.MaintenanceHistory).Name;// inc.MaintenanceHistory;
                    entity["hms_roMaintenanceHistoryOther".ToLower()] = inc.MaintenanceHistoryOther;
                    entity["hms_roAccident".ToLower()] = GlobalParam.AccidentList.Find(item => item.Code == inc.Accident).Name;//inc.Accident;
                    entity["hms_roAccidentOther".ToLower()] = inc.AccidentOther;
                    entity["hms_roTransformCar".ToLower()] = GlobalParam.TransformCarList.Find(item => item.Code == inc.TransformCar).Name;// inc.TransformCar;
                    entity["hms_roTransformCarOther".ToLower()] = inc.TransformCarOther;

                    int newsystemstatus = int.Parse("177980000");
                    entity["hms_newsystemstatus".ToLower()] = new OptionSetValue(newsystemstatus);
                    entity["hms_roapp".ToLower()] = true;
                    entity["hms_rocaseid".ToLower()] = inc.Id + "";
                    entity["hms_tafno".ToLower()] = inc.TafNo.Trim() + "";
                    // var subject = getAccount(inc.CaseSubject);
                    //entity["hms_subject".ToLower()] = new EntityReference("hms_subject",new Guid(inc.CaseSubject));
                    entity["hms_subject".ToLower()] = new EntityReference("hms_subject", new Guid("242C4A30-F0A4-E711-9406-0050568E127D"));
                    






                    //entity["createdby"] = "SYSTEM";
                    // entity["createdon"] = DateTime.Now;
                    //  entity["modifiedby"] = "SYSTEM";
                    //   entity["modifiedon"] = DateTime.Now;

                    Guid id = _service.Create(entity);

                    var getcase = getCase(id);
                    inc.CaseId = getcase["ticketnumber"].ToString();
                    if (UpdateROCase(getcase["ticketnumber"].ToString(), inc.Out_rocode))
                    {
                        var json = JsonConvert.SerializeObject(inc);
                        return new ResultModel() { Status = "S", Message = "Create Complete", Result = json };
                    }
                    else
                    {
                        return new ResultModel() { Status = "E", Message = "ไม่มี Dealer code" };
                    }

                    return new ResultModel() { Status = "S", Message = "Create Complete", Result = getcase["ticketnumber"] };
                }
                else
                {
                    return new ResultModel() { Status = "E", Message = "ไม่มี Dealer code" };
                }

            }
            catch (Exception ex)
            {
                return new ResultModel() { Status = "E", Message = "----" + ex.Message + "---" + Json(entity) };
            }
        }
        private ResultModel CreateCRMCase(ROCaseModel inc, StringBuilder sb)
        {
            Entity entity = new Entity("incident");
            try
            {
                if (_service == null)
                {
                    ConnectToMSCRM();
                }


                //var model = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(myentity);
                var account = getAccount(inc.Out_offcde);
                if (account != null)
                {
                    entity["customerid".ToLower()] = new EntityReference("account", account.Id);
                    entity["hms_outOffcde".ToLower()] = inc.Out_offcde;
                    entity["hms_outCmpcde".ToLower()] = inc.Out_cmpcde;
                    entity["hms_outRocode".ToLower()] = inc.Out_rocode;
                    entity["hms_outCustDate".ToLower()] = inc.Out_cust_date;
                    entity["hms_outRoStatus".ToLower()] = inc.Out_ro_status;
                    entity["hms_outRodate".ToLower()] = inc.Out_rodate;
                    entity["hms_outRotime".ToLower()] = inc.Out_rotime;
                    entity["hms_outWarrantyDate".ToLower()] = inc.Out_warranty_date;
                    entity["hms_outExpiryDate".ToLower()] = inc.Out_expiry_date;
                    entity["hms_outLicense".ToLower()] = inc.Out_license;
                    entity["hms_outChasno".ToLower()] = inc.Out_chasno;
                    entity["hms_outEngno".ToLower()] = inc.Out_engno;
                    entity["hms_outModel".ToLower()] = inc.Out_model;

                    entity["hms_outKiloLast".ToLower()] = inc.Out_kilo_last;
                    entity["hms_outIdno".ToLower()] = inc.Out_idno;
                    entity["hms_outCusname".ToLower()] = inc.Out_cusname;
                    entity["hms_outMobile".ToLower()] = inc.Out_mobile;
                    entity["hms_outAddress".ToLower()] = inc.Out_address;
                    entity["hms_outProvince".ToLower()] = inc.Out_province;
                    entity["hms_outZipcode".ToLower()] = inc.Out_zipcode;
                    entity["hms_outCustype".ToLower()] = inc.Out_custype;
                    entity["hms_aCode".ToLower()] = inc.A_code;
                    entity["hms_bCode".ToLower()] = inc.B_code;
                    entity["hms_cCode".ToLower()] = inc.C_code;
                    entity["hms_levelofProblem".ToLower()] = inc.LevelofProblem;
                    entity["hms_caseTypetext".ToLower()] = inc.CaseType;
                    //entity["hms_caseSubject".ToLower()] = inc.CaseSubject;
                    // entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    entity["hms_caseDescription".ToLower()] = inc.CaseDescription;

                    //entity["hms_statusCode".ToLower()] = inc.StatusCode;
                    //    entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    entity["hms_levelofProblem".ToLower()] = inc.LevelofProblem;

                    //entity["hms_caseType".ToLower()] = inc.CaseType;
                    entity["hms_caseSubject".ToLower()] = inc.CaseSubject;
                    // entity["hms_statusCodeText".ToLower()] = inc.statusCodeText;
                    //entity["hms_caseDescription".ToLower()] = inc.CaseDescription;

                    entity["title".ToLower()] = inc.CaseTitle;

                    entity["hms_background".ToLower()] = inc.CaseDescription;
                    int value = int.Parse(inc.LevelofProblem == null ? "177980000" : inc.LevelofProblem);
                    entity["hms_lop".ToLower()] = new OptionSetValue(value);

                    //   inc.hms_levelofProblem   Options  ;



                    //entity["createdby"] = "SYSTEM";
                    // entity["createdon"] = DateTime.Now;
                    //  entity["modifiedby"] = "SYSTEM";
                    //   entity["modifiedon"] = DateTime.Now;

                    Guid id = _service.Create(entity);

                    var getcase = getCase(id);
                    if (UpdateROCase(getcase["ticketnumber"].ToString(), inc.Out_rocode))
                    {
                        return new ResultModel() { Status = "S", Message = "Create Complete", Result = getcase["ticketnumber"] };
                    }
                    else
                    {
                        return new ResultModel() { Status = "E", Message = "ไม่มี Dealer code" };
                    }

                    return new ResultModel() { Status = "S", Message = "Create Complete", Result = getcase["ticketnumber"] };
                }
                else
                {
                    return new ResultModel() { Status = "E", Message = "ไม่มี Dealer code" };
                }

            }
            catch (Exception ex)
            {
                return new ResultModel() { Status = "E", Message = "----" + ex.Message + "---" + Json(entity) };
            }
        }
        private bool UpdateROCase(string caseID, string ROCode)
        {
            try
            {
                string conString = "";

                if (TechLineCaseAPI.Properties.Settings.Default.DEV)
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringHMS;

                }
                else
                {
                    conString = TechLineCaseAPI.Properties.Settings.Default.ConStringMMTH;

                }
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(
                        "UPDATE ro_case  set caseid='" + caseID + "'  where out_rocode='" + ROCode + "'",
                        connection))
                    {

                        var retId = command.ExecuteNonQuery();
                        return true;

                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }


        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private static int? CreateROCaseLog(string caseId, string source, string data, string createdBy)
        {
            try
            {
                int? rologid;

                if (caseId == null) return null;
                if (source == null) return null;
                if (data == null) return null;
                if (createdBy == null) return null;

                using (mmthapiEntities entity = new mmthapiEntities())
                {
                    var record = new ro_case_log()
                    {
                        CASEID = caseId,
                        STATUS_CODE_FROM = source,
                        STATUS_CODE_TO = data,

                        CREATED_BY = createdBy,
                        CREATED_ON = DateTime.Now,
                        MODIFIED_BY = createdBy,
                        MODIFIED_ON = DateTime.Now,
                    };

                    entity.ro_case_log.AddObject(record);
                    entity.SaveChanges();
                    entity.Refresh(RefreshMode.StoreWins, record);

                    rologid = record.id;
                }

                return rologid;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}