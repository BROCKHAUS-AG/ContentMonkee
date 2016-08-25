using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class WidgetContact : Widget
    {
        public WidgetContact()
        {
            Name = "Contact";
            Title = "Noch Fragen? <br /> Nehmen Sie einfach Kontakt auf.";
            WidgetDescription = "Widget with a contact form";
        }

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string SmtpEmail { get; set; }
        public string AnswerBody { get; set; }
        public string AnswerSubject { get; set; }
        public bool UseVCard { get; set; }

        public string EmployeeName { get; set; }
        public Guid UserId { get; set; }
        [XmlIgnore]
        public GenericRepository<Employee> employees { get; private set; }
        public override void OnLoad(UnitOfWork unit)
        {
            base.OnLoad(unit);
            this.employees = unit.EmployeeRepository;
        }

        public override string GetContent()
        {
            return " "+ Description + this.EmployeeName + " ";
        }
    }

}
