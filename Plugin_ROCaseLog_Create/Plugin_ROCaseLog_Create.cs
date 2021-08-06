using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;

namespace Plugin_ROCaseLog_Create
{
    public class Plugin_ROCaseLog_Create : IPlugin
    {
        private string _pluginname = "Plugin_ROCaseLog_Create";
        private IOrganizationService _service;
        private Guid _currentUserId;
        private IPluginExecutionContext context;
        private ITracingService tracer;

        private string _Url = @"https://phoebe.hms-cloud.com:4430/api/rocase/updatecrm";

        public void Execute(IServiceProvider serviceProvider)
        {
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            tracer.Trace("Execute");

            try
            {   
                if (context.Depth > 1) return;

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity preEntity = (Entity)context.InputParameters["Target"];
                    Entity postEntity = (Entity)context.InputParameters["Target"];

                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    _service = serviceFactory.CreateOrganizationService(context.UserId);
                    preEntity = _service.Retrieve(preEntity.LogicalName, preEntity.Id, new ColumnSet(true));
                    _currentUserId = context.InitiatingUserId;

                    tracer.Trace("Execute : Update New System Status");

                    //////////////////////////////////////////////////////////////////
                    
                    Main(preEntity, postEntity);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ThrowLogMessage("Execute", ex.Message);
            }
        }

        private void Main(Entity preEntity, Entity postEntity)
        {
            tracer.Trace("Main");

            ///////////////////////////////////////////////////////////////////////

            try
            {
                //string caseid = GetEntityMessage(preEntity, "ticketnumber");
                //int source = ((OptionSetValue)GetEntityValue(preEntity, "hms_newsystemstatus")).Value;
                //int data = ((OptionSetValue)GetEntityValue(postEntity, "hms_newsystemstatus")).Value; 
                //string modifiedBy = _currentUserId.ToString();

                string id = GetEntityMessage(preEntity, "hms_rocaseid");
                int preStatus = ((OptionSetValue)GetEntityValue(preEntity, "hms_newsystemstatus")).Value;
                int postStatus = ((OptionSetValue)GetEntityValue(postEntity, "hms_newsystemstatus")).Value;
                string modifiedBy = GetEntityLookupName(preEntity, "ownerid", "fullname");
                string microsoftteamlink = GetEntityMessage(postEntity, "hms_microsoftteamlink");
                string solutionfordealer = GetEntityMessage(postEntity, "hms_solutionfordealer");

                int status;

                switch (postStatus)
                {
                    case 177980001: status = 2; break;
                    case 177980002: status = 3; break;
                    case 177980003: status = 4; break;
                    case 177980004: status = 5; break;
                    case 177980005: status = 6; break;
                    case 177980006: status = 7; break;
                    default: status = 1; break;
                };

                var model = @"{ " +
                    "'Id': '" + id + "'," +
                    "'StatusCode': '" + status + "'," +
                    "'ModifiedBy': '" + modifiedBy + "'," +
                    "'MicrosoftTeamLink': '" + microsoftteamlink + "'," +
                    "'SolutionForDealer': '" + solutionfordealer + "'" +
                    "}";

                //               var data = {

                //                'Id': rocaseid,
                //	'StatusCode':value,
                //	'ModifiedBy': '999',
                //	'MicrosoftTeamLink': Xrm.Page.getAttribute("hms_microsoftteamlink").getValue(),
                //	'SolutionForDealer': Xrm.Page.getAttribute("hms_solutionfordealer").getValue(),
                //};

                //var model = @"{ " + 
                //        "'CaseId': '" + caseid + "'," +
                //        "'StatusCodeFrom': '" + source + "'," +
                //        "'StatusCodeTo': '" + data + "'," +
                //        "'CreatedBy': '" + modifiedBy + "'" +
                //        "}";

                CallUpdateCRMAPI(model);
            }
            catch (Exception ex)
            {
                ThrowLogMessage("Main", ex.Message);
            }
        }

        private void CallUpdateCRMAPI(string model)
        {
            tracer.Trace("CallUpdateCRMAPI");

            ///////////////////////////////////////////////////////////////////////

            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                StringContent httpContent = new StringContent(model, Encoding.UTF8, "application/json");

                form.Add(httpContent, "Model");

                var webRequest = new HttpRequestMessage(HttpMethod.Post, _Url)
                {
                    Content = form
                    //Content = new StringContent(model, Encoding.UTF8, "application/json")
                };

                var response = client.SendAsync(webRequest);
                var result = ContentReader(response.Result.Content);
                
                if (result.Status != "S")
                {
                    ThrowLogMessage("CallUpdateCRMAPI (E)", result.Message);
                }
            }
            catch (Exception ex)
            {
                ThrowLogMessage("CallUpdateCRMAPI", ex.Message);
            }
        }

        public ResultMessage ContentReader(HttpContent requestContent)
        {
            var js = new JavaScriptSerializer();
            var json = requestContent.ReadAsStringAsync().Result;
            ResultMessage model = js.Deserialize<ResultMessage>(json);

            return model;
        }

        /////////////////////////////////////////////////////////////////////////////

        private object GetEntityValue(Entity entity, string field)
        {
            if (entity != null && entity.Contains(field))
                return entity[field];
            else
                return null;
        }

        private string GetEntityMessage(Entity entity, string field)
        {
            object obj = GetEntityValue(entity, field);
            if (obj != null)
                return entity[field].ToString();
            else
                return null;
        }

        private string GetEntityLookupName(Entity entity, string field, string fieldname)
        {
            try
            {
                object obj = GetEntityValue(entity, field);
                if (obj != null)
                {
                    EntityReference lookup = (EntityReference)obj;
                    Entity lookupEntity = _service.Retrieve(lookup.LogicalName, lookup.Id, new ColumnSet(true));

                    return lookupEntity[fieldname].ToString();
                }
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void ThrowLogMessage(string functionname, object msg)
        {
            if (msg != null)
            {
                string mymsg = _pluginname + "(" + functionname + ") : " + msg.ToString();
                EventLog.WriteEntry("Application", mymsg, EventLogEntryType.Error);
                throw new InvalidPluginExecutionException(mymsg);
            }
        }
    }
}
