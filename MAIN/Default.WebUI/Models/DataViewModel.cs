using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class DataViewModel
    {
        public DataViewModel()
        {
            Employees = new List<Employee>();
        }
        public List<Employee> Employees { get; set; }

        public ISiteSelection SiteSelectionViewModel { get; set; }
        

        public SiteSetting CurrentSiteSetting { get; set; }
        public List<SiteSetting> SiteSettings { get; set; }
        string _tabKey = string.Empty;
        public string TabKey
        {
            get
            {
                return _tabKey;
            }
            set
            {
                _tabKey = value;
                if (_tabKey == null)
                {
                    _tabKey = string.Empty;
                }
            }
        }
        public string GetTabActiveString(string tabKey)
        {


            if (tabKey == TabKey)
                return "active";

            return string.Empty;
        }
    }
}