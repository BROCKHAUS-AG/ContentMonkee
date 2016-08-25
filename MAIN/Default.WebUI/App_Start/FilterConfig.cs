using BAG.Common;
using DevMentor.PerformanceMonitoring;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Default.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleCustomErrorAttribute());
        }
    }
    public class HandleCustomErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //    StreamWriter sw = null;
            //string path = filterContext.HttpContext.Server.MapPath(@"~/App_Data/ErrorLog.txt");
            //    if (!File.Exists(path))
            //    {
            //        sw = File.CreateText(path);
            //        sw.Close();
            //    }

            //    sw = File.AppendText(path);

            //string message = filterContext.Exception.StackTrace;
            string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            //sw.WriteLine(timestamp + ":  " + message);

            //sw.Close();

            var monitor = new PerfMonitor(_Globals.Instance.CurrentLoginUserName);
            //monitor.Line.Message = timestamp + ": \n" + GetExceptionMessage(filterContext.Exception) + "\nStackTrace:\n" + filterContext.Exception.StackTrace + "\n\n";


            monitor.Line.Message = timestamp + ": \r\n" + GetExceptionMessage(filterContext.Exception) + "\r\n\r\nStackTrace:\r\n" + filterContext.Exception.StackTrace + "\r\n\r\n";

            monitor.Dispose();

            throw new HttpException(350, "Sorry, we will fix this!");
        }

        private string GetExceptionMessage(Exception e)
        {
            string innerMessage = string.Empty;
            if (e.InnerException != null)
            {
                innerMessage = e.InnerException.Message;
            }
            return e.Message + "\n" + innerMessage;
        }
    }


}

