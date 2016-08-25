using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DevMentor.PerformanceMonitoring
{
    public class PerfMonitorActionFilterAttribute : ActionFilterAttribute
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, PerfMonitor> dir = new System.Collections.Concurrent.ConcurrentDictionary<string, PerfMonitor>();


        public PerfMonitorActionFilterAttribute()
        {

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!PerfMonitorContext.Current.Enabled)
                return;

            var key = filterContext.ActionDescriptor.UniqueId;

            PerfMonitor monitor;
            monitor = new PerfMonitor("PerfLogActionFilter");
            monitor.Line.Url = "/" + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + filterContext.ActionDescriptor.ActionName;
            dir.TryAdd(key, monitor);

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!PerfMonitorContext.Current.Enabled)
                return;

            var key = filterContext.ActionDescriptor.UniqueId;
            PerfMonitor monitor;
            if (dir.TryRemove(key, out monitor))
            {
                if (filterContext.Exception != null)
                {
                    monitor.Line.Message = filterContext.Exception.Message;
                    monitor.Line.Status = 520;
                }
                monitor.Dispose();
            }
            base.OnActionExecuted(filterContext);
        }

        string GenerateKey(ControllerContext filterContext)
        {
            var result = string.Empty;
            foreach (var item in filterContext.RouteData.Values)
            {
                result += "." + item.Value.ToString();
            }
            return System.Threading.Thread.CurrentThread.ManagedThreadId + "TID " + result;
        }
    }
}
