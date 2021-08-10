using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_AutoNumberRequest
{
    public class Plugin_AutoNumberRequest : IPlugin
    {
        string _pluginname = "Plugin_ProblemList_AutoNum";
        IOrganizationService _service;
        IPluginExecutionContext context;
        ITracingService tracer;



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
            tracer.Trace("Main");

            try
            {
                StringBuilder sb = new StringBuilder();
                List<Entity> list = QueryEntityList("hms_autonumberrequest", "hms_name", entity.LogicalName);

                if (list.Count() > 0)
                {
                    Entity myentity = list[0];

                    string field = GetEntityString(myentity, "hms_field");
                    int number = (int)GetEntityMoney(myentity, "hms_runningnumber").Value;
                    int length = (int)GetEntityMoney(myentity, "hms_numberlength").Value;
                    int year = (int)GetEntityMoney(myentity, "hms_year").Value;
                    int yearlength = (int)GetEntityMoney(myentity, "hms_yearlength").Value;                    
                    string pattern = GetEntityString(myentity, "hms_pattern");
                    
                    if (field != null && pattern != null)
                    {
                        number = number + 1;

                        if (year == 0)
                        {
                            year = DateTime.Now.Year;
                        }
                        else
                        {
                            if (year < DateTime.Now.Year)
                            {
                                number = 1;
                            }
                        }

                        tracer.Trace("number: " + number);
                        tracer.Trace("length: " + length);
                        tracer.Trace("year: " + year);
                        tracer.Trace("yearlength: " + yearlength);
                        
                        sb = sb
                        .Append(pattern)
                        .Replace("[Number]", GetFormatNumber(number, length))
                        .Replace("[Year]", GetYear(yearlength));
                        
                        tracer.Trace(sb.ToString());

                        entity[field] = sb.ToString();
                        myentity["hms_runningnumber"] = decimal.Parse(number.ToString());
                        myentity["hms_year"] = decimal.Parse(year.ToString());

                        tracer.Trace("Main: Update entity");
                        _service.Update(entity);

                        tracer.Trace("Main: Update myentity");
                        _service.Update(myentity);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                ThrowLogMessage("Main", ex);
            }
        }

        private string GetYear(int length)
        {
            tracer.Trace("GetYear");

            return DateTime.Now.Year.ToString().Substring(4 - length);
        }

        private string GetFormatNumber(int number, int length)
        {
            tracer.Trace("GetNumber");

            StringBuilder sb = new StringBuilder();

            if (number.ToString().Length > length)
            {
                return null;
            }

            for (int i = 0; i < length - number.ToString().Length; i++)
            {
                sb.Append("0");
            }

            return sb.Append(number).ToString();
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

        private Money GetEntityMoney(Entity entity, string field)
        {
            if (entity != null && entity.Contains(field))
                return new Money(decimal.Parse(entity[field].ToString()));
            else
                return new Money(0);
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
