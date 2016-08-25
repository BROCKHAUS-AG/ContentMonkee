using System.Globalization;
using System.Web.Http;
using System.Web.Mvc;
using BAG.Framework.Geolocation.Models;
using System;
using BAG.Common.Data;
using BAG.Common.Data.Entities;
using BAG.Framework.Geolocation;
//using Default.WebUI.Mail;
using Default.WebUI.Models;
//using SitefinityWebApp.Mail;
using System.Collections.Generic;
using System.Web.Services.Description;

namespace Default.WebUI.Controllers
{
    public class AjaxController : Controller
    {
        public const string VisitorLocationKey = "EXACT_VISITOR_LOCATION";
        UnitOfWork unit=new UnitOfWork();
        //public JsonResult SaveLocation()
        //{
        //    string loc = Request.Params["location"];
        //    if (!string.IsNullOrWhiteSpace(loc) && loc.Contains(","))
        //    {
        //        string[] locTiles = loc.Split(',');
        //        double longtitude, latitude;

        //        if (double.TryParse(locTiles[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out latitude) &&
        //            double.TryParse(locTiles[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out longtitude))
        //        {
        //            Coordinate visitorLocation = new Coordinate(latitude, longtitude);
        //            Session[VisitorLocationKey] = visitorLocation;
        //        }
        //    }

        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult Contact(ContactForm form)
        //{
        //   MailTemplate template = MailTemplate.LoadTemplate("ContactFormMail");
        //   Manufactory manufactory = unit.ManufactoryRepository.GetByID(form.ManufactoryId);
           
        //   template.From = form.From;
        //   template.Subject = "Kontaktanfrage von edition-tischler.de";
        //   template.Model = new ContactMailModel
        //   {
        //       FirstName = form.FirstName,
        //       LastName = form.LastName,
        //       Mail = form.Mail,
        //       Message = form.Msg,
        //       Phone = form.Telephone
        //   };

        //   if (form.DoSendCopyToMe)
        //   {
        //       template.Bcc = form.Mail;
        //   }

        //   template.Render();

        //    if (AppConfig.IsProductiveEnvironment)
        //    {
        //        template.To = manufactory.Mail; 
        //    }
        //    else
        //    {
        //        template.To = AppConfig.TestMail;
        //    }
          
        //   try
        //   {
        //       template.Send();
        //       form.State = "Success";
        //       unit.ContactFormRepository.Insert(form);
        //       unit.Save();
        //       return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //   }
        //   catch (Exception ex)
        //   {
        //       form.State = "Error: "+ex.Message;
        //       unit.ContactFormRepository.Insert(form);
        //       unit.Save();
        //       return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
        //   }
        //}

        //public JsonResult EditionsOrder(EditionsOrderForm form)
        //{
        //    MailTemplate template = MailTemplate.LoadTemplate("OrderFormMail");
        //    Manufactory manufactory = unit.ManufactoryRepository.GetByID(form.ManufactoryId);

        //    List<OrderItem> items = new List<OrderItem>();
        //    form.Editions.ForEach(e =>
        //    {
        //        var edition = unit.EditionRepository.GetByID(e.EditionId);
        //        items.Add(new OrderItem()
        //        {
        //            Count = e.Count,
        //            Note = e.Note,
        //            UnitPrice = edition.Price,
        //            Title = edition.Title
        //        });
        //    });

        //    template.From = form.From;
        //    template.Subject = "Anfrage von edition-tischler.de";
        //    template.Model = new OrderMailModel()
        //    {
        //        FirstName = form.FirstName,
        //        LastName = form.LastName,
        //        Mail = form.Mail,
        //        Phone = form.Telephone,
        //        Items = items,
        //        Text = form.Msg
        //    };

        //    if (form.DoSendCopyToMe)
        //    {
        //        template.Bcc = form.Mail;
        //    }

        //    template.Render();

        //    if (AppConfig.IsProductiveEnvironment)
        //    {
        //        template.To = manufactory.Mail;
        //    }
        //    else
        //    {
        //        template.To = AppConfig.TestMail;
        //    }

        //    try
        //    {
        //        template.Send();
        //        form.State = "Success";
        //        unit.ContactFormRepository.Insert(form);
        //        unit.Save();
        //        return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        form.State = "Error: " + ex.Message;
        //        unit.ContactFormRepository.Insert(form);
        //        unit.Save();
        //        return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

       

        
        
    }
}