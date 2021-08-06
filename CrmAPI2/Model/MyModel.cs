using System;
using System.Collections.Generic;

namespace CrmAPI2.Model
{
    public class ResultMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Value { get; set; }
    }

    public class TotalModel
    {
        public int CaseTotal { get; set; }
        public int Pending { get; set; }
        public int DtsEff { get; set; }
        public int TlEff { get; set; }
        public int FtsEff { get; set; }
    }

    public class ChartCaseModel
    {
        public ChartCaseModel()
        {
            Detail = new List<ChartCaseSubModel>();
        }

        public List<string> Header { get; set; }
        public List<ChartCaseSubModel> Detail { get; set; }
    }

    public class ChartCaseSubModel
    {
        public ChartCaseSubModel()
        {
            Value = new List<int>();
        }

        public string Subject { get; set; }
        public List<int> Value { get; set; }
    }

    public class KeyValuePairModel
    {
        public KeyValuePairModel()
        {
            Value = new List<KeyValuePair<string, int>>();
        }

        public string Header { get; set; }
        public string Detail { get; set; }
        public List<KeyValuePair<string, int>> Value { get; set; }
    }

    public class DateFromModel
    {
        public DateFromModel()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
        }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}