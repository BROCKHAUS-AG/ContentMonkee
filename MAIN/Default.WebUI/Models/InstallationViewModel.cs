using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class InstallationViewModel
    {
        public InstUser User { get; set; }
        public InstSettings Settings { get; set; }
    }

    public class InstUser
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RPassword { get; set; }
        public string Mail { get; set; }

    }

    public class InstSettings
    {
        public string Name { get; set; }
        public string Domain { get; set; }
    }
}