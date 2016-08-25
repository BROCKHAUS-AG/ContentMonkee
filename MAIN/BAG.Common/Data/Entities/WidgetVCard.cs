using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class WidgetVCard : Widget
    {
        public WidgetVCard()
        {
            Name = "V Card";
            UserId = Guid.Empty;
            WidgetDescription = "Widget wich Proviede Employees Data";
        }
        public string TitleVCard { get; set; }
        public string EmployeeName { get; set; }

        public Guid UserId { get; set; }
        public string TitleFlyer { get; set; }
        public string FlyerFileUrl { get; set; }
        public string FlyerText { get; set; }
        public string TitleReferences { get; set; }
        public string ReferencesFileUrl { get; set; }
        public string ReferencesText { get; set; }

        [XmlIgnore]
        public GenericRepository<Employee> employees { get; private set; }


        public override void OnLoad(UnitOfWork unit)
        {
            base.OnLoad(unit);
            this.employees = unit.EmployeeRepository;
        }

        public override string GetContent()
        {
            return " " + this.EmployeeName + " " + this.FlyerText + " " + this.ReferencesText;
        }
    }
}
