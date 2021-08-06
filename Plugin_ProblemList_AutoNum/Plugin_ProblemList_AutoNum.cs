using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_ProblemList_AutoNum
{
    public class Plugin_ProblemList_AutoNum : IPlugin
    {
        string _pluginname = "Plugin_ProblemList_AutoNum";
        IOrganizationService _service;
        ITracingService tracer;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                if (context.Depth > 2) return;

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];

                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    _service = serviceFactory.CreateOrganizationService(context.UserId);
                    entity = _service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));

                    Main(entity);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ThrowLogMessage("Execute", ex);
            }
        }

        private void Main(Entity entity)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<Entity> list = QueryEntityList("hms_autonumberrequest", "hms_name", entity.LogicalName);

                if (list.Count() > 0)
                {
                    Entity myentity = list[0];

                    string field = GetEntityString(myentity, "hms_field");
                    int number = GetEntityInt(myentity, "hms_runningnumber");
                    int length = GetEntityInt(myentity, "hms_numberlength");
                    int yearlength = GetEntityInt(myentity, "hms_yearlength");
                    string pattern = GetEntityString(myentity, "hms_pattern");

                    if (field != null && pattern != null)
                    {
                        sb = sb
                        .Append(pattern)
                        .Replace("[Number]", GetNumber(number, length))
                        .Replace("[Year]", GetYear(yearlength));

                        entity[field] = sb.ToString();

                        _service.Update(entity);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ThrowLogMessage("Execute", ex);
            }
        }

        private string GetYear(int length)
        {
            return DateTime.Now.Year.ToString().Substring(length);
        }

        private string GetNumber(int number, int length)
        {
            StringBuilder sb = new StringBuilder();

            if (number.ToString().Length > length)
            {
                return null;
            }

            for (int i = 0; i < length - number.ToString().Length; i++)
            {
                sb.Append("0");
            }

            return sb.Append(number + 1).ToString();
        }

        //////////////////////////////////////////////////////////////

        private object GetEntityValue(Entity entity, string field)
        {
            if (entity != null && entity.Contains(field))
                return entity[field];
            else
                return null;
        }

        private string GetEntityString(Entity entity, string field)
        {
            if (entity != null && entity.Contains(field))
                return entity[field].ToString();
            else
                return null;
        }

        private int GetEntityInt(Entity entity, string field)
        {
            if (entity != null && entity.Contains(field))
                return Int32.Parse(entity[field].ToString());
            else
                return 0;
        }

        private List<Entity> QueryEntityList(string entityName, string fieldName, object value)
        {
            tracer.Trace("QueryEntityList");

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
                                Operator = ConditionOperator.Equal,
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
                //WriteLogMessage("QueryEntityList", ex);
                return null;
            }
        }

        private void WriteLogMessage(string functionname, object msg)
        {
            if (msg != null)
            {
                string mymsg = _pluginname + "(" + functionname + ") : " + msg.ToString();
                EventLog.WriteEntry("Application", mymsg, EventLogEntryType.Error);
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
