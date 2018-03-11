using SendSMSHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendSMSHost.ViewModels
{
    public class SummaryIndexViewModel
    { 
        public List<SummaryInterval> SummaryIntervals { get; set; }

        public SummaryIndexViewModel()
        {
            SummaryIntervals = new List<SummaryInterval>();
        }
    }
}