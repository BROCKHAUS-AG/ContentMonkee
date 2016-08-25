using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;
using BAG.Common.Authorization;
using Default.WebUI.Extensions;
using BAG.Common.Data.Entities;
using System.Net;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Default.WebUI.Controllers
{
    [WebAnonymousAttribute()]
    public class SendContactController : Controller
    {
        [Obsolete]
        private UnitOfWork unit = null;
        private UnitOfWork Unit
        {
            get
            {
                if (unit == null)
                {
                    this.unit = new UnitOfWork();
                }
                return this.unit;
            }
            set
            {
                this.unit = Unit;
            }
        }
        [HttpPost]
        public ActionResult SendMessage(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork unit = new UnitOfWork();
                var id = form.GetGuid("id", Guid.Empty);
                var widgetdata = unit.WidgetManagerRepository.GetByID(id).PreRelease;
                var widget = widgetdata as WidgetContact;
                string formname = form.GetString("fullname");
                string formtelefon = form.GetString("telefon");
                string formemail = form.GetString("email");
                string formmessage = form.GetString("message");

                try
                {
                    MailMessage msg = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    MailDefinition md = new MailDefinition();
                    StringBuilder sb = new StringBuilder();

                    msg.To.Add(widget.SmtpEmail);
                    msg.From = new MailAddress(widget.SmtpEmail);
                    msg.Subject = "Kontakt";
                    msg.IsBodyHtml = false;

                    sb.Append("Name: " + formname);
                    sb.Append(Environment.NewLine);
                    sb.Append("Telefon: " + formtelefon);
                    sb.Append(Environment.NewLine);
                    sb.Append("E-Mail: " + formemail);
                    sb.Append(Environment.NewLine);
                    sb.Append("Nachricht: " + formmessage);

                    msg.Body = sb.ToString();

                    var credential = new NetworkCredential
                    {
                        UserName = widget.SmtpUserName,
                        Password = widget.SmtpPassword
                    };
                    smtp.Credentials = credential;
                    smtp.Host = widget.SmtpHost;
                    smtp.Port = widget.SmtpPort;
                    smtp.EnableSsl = widget.SmtpEnableSsl;

                    md.From = widget.SmtpEmail;
                    md.IsBodyHtml = true;
                    md.Subject = widget.AnswerSubject;

                    ListDictionary replacements = new ListDictionary();
                    replacements.Add("{name}", formname);
                    replacements.Add("{message}", formmessage);

                    string bodytwo = widget.AnswerBody;

                    MailMessage msg2 = md.CreateMailMessage(formemail, replacements, bodytwo, new System.Web.UI.Control());


                    smtp.Send(msg);
                    smtp.Send(msg2);
                    msg.Dispose();
                    msg2.Dispose();
                    return RedirectToAction("Index","Home");
                }
                catch (Exception)
                {
                    return Content("Error");
                }
            }
            return Content("Ok");
        }
    }
}