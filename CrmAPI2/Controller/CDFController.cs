using CrmAPI2.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CrmAPI2.Controller
{
    public class CDFController : ApiController
    {
        private CrmServiceClient _service;

        public CDFController()
        {
            try
            {
                string UserName = Properties.Settings.Default.UserName;
                string Password = Properties.Settings.Default.Password;
                string SoapOrgServiceUri = Properties.Settings.Default.SoapOrgServiceUri;

                //string UserName = @"bigth\crmadm";
                //string Password = "Crm@bigth";
                //string SoapOrgServiceUri = "https://bqas.bigth.com:444/XRMServices/2011/Organization.svc";

                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);

                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                //_service = (IOrganizationService)proxy;

                _service = new CrmServiceClient(proxy);
                //bool isReady = service.IsReady;
            }
            catch (Exception)
            {
                _service = null;
            }
        }

        [HttpPost]
        [Route("api/incident/post/total")]
        public IHttpActionResult GetTotal()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            //if (mymodel.DateFrom == null) return new ResultMessage() { Status = "E", Message = "Require Date From" };
            //if (mymodel.DateTo == null) return new ResultMessage() { Status = "E", Message = "Require Date To" };

            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            int total = list.Count();
            int opencase = list.Where(o => GetNewStatus(o) == 177980000).Count();
            int onproc = list.Where(o => GetNewStatus(o) == 177980001).Count();
            int scrmbproc = list.Where(o => GetNewStatus(o) == 177980002).Count();
            int techcomp = list.Where(o => GetNewStatus(o) == 177980003).Count();
            int dealcomp = list.Where(o => GetNewStatus(o) == 177980004).Count();
            int scrmbcomp = list.Where(o => GetNewStatus(o) == 177980005).Count();
            int closed = list.Where(o => GetNewStatus(o) == 177980006).Count();

            TotalModel model = new TotalModel();
            model.CaseTotal = total;
            model.Pending = onproc + scrmbproc;
            model.DtsEff = GetPercent(techcomp + scrmbcomp + closed, total);
            model.TlEff = GetPercent(techcomp + closed, total - scrmbproc - scrmbcomp);
            model.FtsEff = GetPercent(scrmbcomp, scrmbproc + scrmbcomp);

            return Json(model);
        }

        private int GetNewStatus(Entity entity)
        {
            if (entity.Contains("hms_newsystemstatus"))
            {
                return ((OptionSetValue)entity["hms_newsystemstatus"]).Value;
            }
            else
            {
                return -1;
            }
        }

        private int GetPercent(int value, int maxvalue)
        {
            if (maxvalue > 0)
            {
                return value * 100 / maxvalue;
            }
            else
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("api/incident/post/dailycase")]
        public IHttpActionResult GetDailyCase()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            ChartCaseModel model = new ChartCaseModel();
            List<string> header = new List<string>();
            ChartCaseSubModel submodel1 = new ChartCaseSubModel();
            ChartCaseSubModel submodel2 = new ChartCaseSubModel();
            ChartCaseSubModel submodel3 = new ChartCaseSubModel();

            header.Add("Owner");
            submodel1.Subject = "Pick";
            submodel2.Subject = "Complete";
            submodel3.Subject = "On Process";

            if(list.Count() > 0)
            {
                foreach (Entity entity in list)
                {
                    string name = ((EntityReference)entity["ownerid"]).Name;

                    if (!header.Contains(name))
                    {
                        header.Add(name);

                        int pick = 0;
                        int complete = 0;
                        int process = 0;

                        if (entity.Contains("hms_newsystemstatus"))
                        {
                            int value = ((OptionSetValue)(entity["hms_newsystemstatus"])).Value;

                            switch (value)
                            {
                                case 177980000: pick++; break;
                                case 177980001: process++; break;
                                case 177980002: process++; break;
                                case 177980003: complete++; break;
                                case 177980004: process++; break;
                                case 177980005: complete++; break;
                                case 177980006: complete++; break;
                            };
                        }

                        submodel1.Value.Add(pick);
                        submodel2.Value.Add(complete);
                        submodel3.Value.Add(process);
                    }
                    else
                    {
                        if (entity.Contains("hms_newsystemstatus"))
                        {
                            int index = header.FindIndex(o => o.Contains(name)) - 1;
                            int value = ((OptionSetValue)(entity["hms_newsystemstatus"])).Value;

                            switch (value)
                            {
                                case 177980000: submodel1.Value[index] += 1; break;
                                case 177980001: submodel3.Value[index] += 1; break;
                                case 177980002: submodel3.Value[index] += 1; break;
                                case 177980003: submodel2.Value[index] += 1; break;
                                case 177980004: submodel3.Value[index] += 1; break;
                                case 177980005: submodel2.Value[index] += 1; break;
                                case 177980006: submodel2.Value[index] += 1; break;
                            };
                        }
                    }
                }
            }
            else
            {
                header.Add("N/A");
                submodel1.Value.Add(0);
                submodel2.Value.Add(0);
                submodel3.Value.Add(0);
            }

            model.Header = header;
            model.Detail.Add(submodel1);
            model.Detail.Add(submodel2);
            model.Detail.Add(submodel3);

            return Json(model);
        }

        [HttpPost]
        [Route("api/incident/post/effdtsteamweek")]
        public IHttpActionResult GetEfficiencyDTSTeamWeek()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            ChartCaseModel model = new ChartCaseModel();
            List<string> header = new List<string>();
            header.Add("Team");
            header.Add("DTS. Team");
            header.Add("Tech-line");
            header.Add("Scamble");

            List<Entity> techLinelist = GroupTeamByStatus("TechLine", list);
            List<Entity> scramblelist = GroupTeamByStatus("Scramble", list);
            List<Entity> dealerlist = GroupTeamByStatus("Dealer", list);

            model.Header = header;

            for (int i = 0; i < 4; i++)
            {
                ChartCaseSubModel submodel = new ChartCaseSubModel();
                submodel.Subject = GetDateOn("Week", i * 7, (i + 1) * 7);
                submodel.Value.Add(techLinelist.Where(o => IsWeekOn(o["createdon"], i * 7, (i + 1) * 7)).Count());
                submodel.Value.Add(scramblelist.Where(o => IsWeekOn(o["createdon"], i * 7, (i + 1) * 7)).Count());
                submodel.Value.Add(dealerlist.Where(o => IsWeekOn(o["createdon"], i * 7, (i + 1) * 7)).Count());

                model.Detail.Insert(0, submodel);
            }

            return Json(model);
        }

        [HttpPost]
        [Route("api/incident/post/effdtsteammonth")]
        public IHttpActionResult GetEfficiencyDTSTeamMonth()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            ChartCaseModel model = new ChartCaseModel();
            List<string> header = new List<string>();
            header.Add("Team");
            header.Add("DTS. Team");
            header.Add("Tech-line");
            header.Add("Scamble");

            List<Entity> techLinelist = GroupTeamByStatus("TechLine", list);
            List<Entity> scramblelist = GroupTeamByStatus("Scramble", list);
            List<Entity> dealerlist = GroupTeamByStatus("Dealer", list);

            model.Header = header;

            for (int i = 0; i < 12; i++)
            {
                ChartCaseSubModel submodel = new ChartCaseSubModel();
                submodel.Subject = GetDateOn("Month", i, i + 1);
                submodel.Value.Add(techLinelist.Where(o => IsMonthOn(o["createdon"], i, i + 1)).Count());
                submodel.Value.Add(scramblelist.Where(o => IsMonthOn(o["createdon"], i, i + 1)).Count());
                submodel.Value.Add(dealerlist.Where(o => IsMonthOn(o["createdon"], i, i + 1)).Count());

                model.Detail.Insert(0, submodel);
            }

            return Json(model);
        }

        [HttpPost]
        [Route("api/incident/post/effdtsteamyear")]
        public IHttpActionResult GetEfficiencyDTSTeamYear()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            ChartCaseModel model = new ChartCaseModel();
            List<string> header = new List<string>();
            header.Add("Team");
            header.Add("DTS. Team");
            header.Add("Tech-line");
            header.Add("Scamble");

            List<Entity> techLinelist = GroupTeamByStatus("TechLine", list);
            List<Entity> scramblelist = GroupTeamByStatus("Scramble", list);
            List<Entity> dealerlist = GroupTeamByStatus("Dealer", list);

            model.Header = header;

            for (int i = 0; i < 4; i++)
            {
                ChartCaseSubModel submodel = new ChartCaseSubModel();
                submodel.Subject = GetDateOn("Year", i, i + 1);
                submodel.Value.Add(techLinelist.Where(o => IsYearOn(o["createdon"], i, i + 1)).Count());
                submodel.Value.Add(scramblelist.Where(o => IsYearOn(o["createdon"], i, i + 1)).Count());
                submodel.Value.Add(dealerlist.Where(o => IsYearOn(o["createdon"], i, i + 1)).Count());

                model.Detail.Insert(0, submodel);
            }

            return Json(model);
        }

        private string GetDateOn(string type, int end, int start)
        {
            switch (type)
            {
                case "Week": return "Week " + (start / 7);
                case "Month":
                    {
                        DateTime date = DateTime.Now.AddMonths(end * -1);

                        switch (date.Month)
                        {
                            case 1: return "JAN," + date.Year;
                            case 2: return "FEB," + date.Year;
                            case 3: return "MAR," + date.Year;
                            case 4: return "APL," + date.Year;
                            case 5: return "MAY," + date.Year;
                            case 6: return "JUN," + date.Year;
                            case 7: return "JUL," + date.Year;
                            case 8: return "AUG," + date.Year;
                            case 9: return "SEP," + date.Year;
                            case 10: return "OCT," + date.Year;
                            case 11: return "NOV," + date.Year;
                            case 12: return "DEC," + date.Year;
                            default: return date.Year.ToString();
                        }
                    }
                case "Year": return DateTime.Now.AddYears(end * -1).Year.ToString();
                default: return null;
            }
        }

        private bool IsWeekOn(object obj, int end, int start)
        {
            DateTime datenow = DateTime.Now;
            DateTime mydatenow = new DateTime(datenow.Year, datenow.Month, datenow.Day);

            DateTime mydate = (DateTime)obj;
            DateTime datefrom = mydatenow.AddDays(start * -1);
            DateTime dateto = mydatenow.AddDays(end * -1);

            return datefrom <= mydate && mydate <= dateto ? true : false;
        }

        private bool IsMonthOn(object obj, int end, int start)
        {
            DateTime datenow = DateTime.Now;
            DateTime mydatenow = new DateTime(datenow.Year, datenow.Month, datenow.Day);

            DateTime mydate = (DateTime)obj;
            DateTime datefrom = mydatenow.AddMonths(start * -1);
            DateTime dateto = mydatenow.AddMonths(end * -1);

            return datefrom <= mydate && mydate <= dateto ? true : false;
        }

        private bool IsYearOn(object obj, int end, int start)
        {
            DateTime datenow = DateTime.Now;
            DateTime mydatenow = new DateTime(datenow.Year, datenow.Month, datenow.Day);

            DateTime mydate = (DateTime)obj;
            DateTime datefrom = mydatenow.AddYears(start * -1);
            DateTime dateto = mydatenow.AddYears(end * -1);

            return datefrom <= mydate && mydate <= dateto ? true : false;
        }

        [HttpPost]
        [Route("api/incident/post/inchargecase")]
        public IHttpActionResult GetInChargeCase()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            KeyValuePairModel model = new KeyValuePairModel();
            int techline = GroupTeamByStatus("TechLine", list).Count();
            int scramble = GroupTeamByStatus("Scramble", list).Count();
            int ps = 0;
            int dealer = GroupTeamByStatus("Dealer", list).Count();

            model.Header = "In Change Case";
            model.Detail = "Case by Team";
            model.Value.Add(new KeyValuePair<string, int>("Tech-line", techline));
            model.Value.Add(new KeyValuePair<string, int>("Scramble", scramble));
            model.Value.Add(new KeyValuePair<string, int>("PS.", ps));
            model.Value.Add(new KeyValuePair<string, int>("Dealer", dealer));

            return Json(model);
        }

        private List<Entity> GroupTeamByStatus(string type, List<Entity> list)
        {
            if (type == "TechLine")
            {
                return list.Where(o =>
                    o.Contains("hms_newsystemstatus") &&
                    (
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980000 ||
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980001 ||
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980002
                    )
                ).ToList();
            }
            else if (type == "Scramble")
            {
                return list.Where(o =>
                    o.Contains("hms_newsystemstatus") &&
                    (
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980003 ||
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980005
                    )
                ).ToList();
            }
            else if (type == "Dealer")
            {
                return list.Where(o =>
                    o.Contains("hms_newsystemstatus") &&
                    (
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980004 ||
                        ((OptionSetValue)o["hms_newsystemstatus"]).Value == 177980006
                    )
                ).ToList();
            }
            else
            {
                return new List<Entity>();
            }
        }

        [HttpPost]
        [Route("api/incident/post/categorycase")]
        public IHttpActionResult getCategoryCase()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            KeyValuePairModel model = new KeyValuePairModel();

            model.Header = "CategoryCase";
            model.Detail = "Detail";
            model.Value.Add(new KeyValuePair<string, int>("Close case delay", 2));
            model.Value.Add(new KeyValuePair<string, int>("New problem & New car.", 4));
            model.Value.Add(new KeyValuePair<string, int>("Difficult investigation", 5));
            model.Value.Add(new KeyValuePair<string, int>("Waiting data from DLR", 19));

            return Json(model);
        }

        [HttpPost]
        [Route("api/incident/post/scramble")]
        public IHttpActionResult getScramble()
        {
            var js = new JavaScriptSerializer();
            var json = HttpContext.Current.Request.Form["Model"];

            DateFromModel mymodel = js.Deserialize<DateFromModel>(json);
            List<Entity> list = QueryEntityDateList("incident", "createdon", mymodel.DateFrom, mymodel.DateTo);

            KeyValuePairModel model = new KeyValuePairModel();

            model.Header = "Scramble";
            model.Detail = "Detail";

            var levelgroups = list
                .Where(o => o.Contains("hms_lop"))
                .Select(o => new
                {
                    Text = o.FormattedValues["hms_lop"].ToString(),
                    Value = ((OptionSetValue)o["hms_lop"]).Value
                })
                .OrderBy(o => o.Value)
                .GroupBy(o => o.Text);

            if (levelgroups.Count() > 0)
            {
                foreach (var group in levelgroups)
                {
                    model.Value.Add(new KeyValuePair<string, int>(group.Key, group.Count()));
                }
            }
            else
            {
                model.Value.Add(new KeyValuePair<string, int>("N/A", 0));
            }

            return Json(model);
        }

        [HttpGet]
        [Route("api/incident/get/caseid/{caseid}")]
        public IHttpActionResult GetCaseByCaseId(string caseid)
        {
            List<Entity> list = QueryEntityList("incident", "hms_rocaseid", ConditionOperator.Equal, caseid);

            if (list.Count() > 0)
            {
                return Json(list[0]);
            }
            else
            {
                return null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////

        private Guid QueryGuid(string entityname, string attr, string val)
        {
            try
            {
                Guid returnGuid = Guid.Empty;

                var query = new QueryExpression
                {
                    EntityName = entityname,
                    ColumnSet = new ColumnSet() { AllColumns = true },
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = attr,
                                Operator = ConditionOperator.Equal,
                                Values = { val }
                            },
                        }
                    }
                };

                var q = _service.RetrieveMultiple(query);
                if (q.Entities != null && q.Entities.Count > 0)
                {
                    Entity data = q.Entities[0];
                    returnGuid = data.Id;
                }
                return returnGuid;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        private List<Entity> QueryEntityList(string entityName, string fieldName, ConditionOperator op, object value)
        {
            try
            {
                Guid returnGuid = Guid.Empty;

                var query = new QueryExpression
                {
                    EntityName = entityName,
                    ColumnSet = new ColumnSet() { AllColumns = true },
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = fieldName,
                                Operator = op,
                                Values = { value }
                            },
                        }
                    }
                };

                var q = _service.RetrieveMultiple(query);

                return q.Entities.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<Entity> QueryEntityDateList(string entityName, string fieldName, DateTime value1, DateTime value2)
        {
            try
            {
                Guid returnGuid = Guid.Empty;

                value1 = new DateTime(value1.Year, value1.Month, value1.Day, 0, 0, 0);
                value2 = new DateTime(value2.Year, value2.Month, value2.Day, 23, 59, 59);

                var query = new QueryExpression
                {
                    EntityName = entityName,
                    ColumnSet = new ColumnSet() { AllColumns = true },
                    Criteria = new FilterExpression
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                            //new ConditionExpression(fieldName, ConditionOperator.GreaterEqual, value1),
                            //new ConditionExpression(fieldName, ConditionOperator.LessEqual, value2)
                            new ConditionExpression(fieldName, ConditionOperator.Between, new Object[] { value1, value2 })
                        }
                    }
                };

                var q = _service.RetrieveMultiple(query);

                return q.Entities.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
