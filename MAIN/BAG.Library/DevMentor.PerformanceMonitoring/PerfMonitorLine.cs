using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DevMentor.PerformanceMonitoring
{
    public class PerfMonitorLine
    {
        public PerfMonitorLine()
        {
            DateTime = DateTime.Now;
            if (HttpContext.Current != null)
            {
                Method = HttpContext.Current.Request.HttpMethod;
                Url = HttpContext.Current.Request.RawUrl;
            }
            else
            {
                Method = "METHOD";
                Url = "HttpContext==null";
            }
        }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Method { get; set; }
        public string MemberName { get; set; }
        public string Url { get; set; }
        public long Duration { get; set; }
        public int Status { get; set; }
    }
}
