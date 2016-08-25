using Default.WebUI.Models;
using BAG.Framework.QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BAG.Common.Data.Entities;
using BAG.Common.Data;
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;

namespace Default.WebUI.Controllers
{
    public class QRCodeController : Controller
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

        public ActionResult GetQRImage(Guid employeeId, int pixelsPerModule)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(GetVCardFromGuid(employeeId), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);            
            Bitmap bitmap = qrCode.GetGraphic(pixelsPerModule, ColorTranslator.FromHtml("#b71e3f"), Color.Transparent, true);

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bitmapBytes = stream.ToArray();

            return File(bitmapBytes, "image/jpeg");
        }

        public ActionResult GetVCardDownload(Guid employeeId)
        {
            string result = GetVCardFromGuid(employeeId);
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(result);
            writer.Flush();
            stream.Position = 0;
            Employee e = Unit.EmployeeRepository.Find(w => w.Id == employeeId);
            string name = e.DisplayName.ToLower().Replace(" ", "_");
            return File(stream, "text/x-vcard", name + ".vcf");
        }

        private string GetVCardFromGuid(Guid id)
        {
            Employee e = Unit.EmployeeRepository.Find(w => w.Id == id);
            if (e == null)
            {
                return id.ToString() + " not found!";
            }

            string tel = Regex.Replace(e.Telephone, @"[^0-9+]+", "");
            string fax = Regex.Replace(e.Fax, @"[^0-9+]+", "");
            string result = "BEGIN:VCARD\n" +
                            "VERSION:2.1" + "\n" +
                            "N:" + e.LastName + ";" + e.FirstName + ";;;" + "\n" +
                            "FN:" + e.DisplayName + "\n" +
                            "TITLE:" + e.Position + "\n" +
                            "ORG:BROCKHAUS AG" + "\n" +
                            "TEL;WORK;VOICE: " + tel + "\n" +
                            "TEL;FAX;VOICE: " + fax + "\n" +
                            "ADR;WORK:;;Pierbusch 17;Luenen;;44536;Deutschland\n" +
                            "EMAIL;PREF;INTERNET:" + e.Email + "\n" +
                            "REV:"+ e.Updated.ToString("yyyyMMddTHmmssZ") + "\n" +
                            "END:VCARD";

            return result;
        }
    }
}