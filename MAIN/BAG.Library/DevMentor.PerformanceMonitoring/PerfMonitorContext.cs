using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace DevMentor.PerformanceMonitoring
{
    public class PerfMonitorContext
    {
        public static string ClassName = "PerfLogExtensions";
        public string ConfigNameFormat = "DevMentor.Perf.Format";
        public string ConfigNameFilePath = "DevMentor.Perf.FilePath";
        public string ConfigNameEnabled = "DevMentor.Perf.Enabled";
        public PerfMonitorContext()
        {
            Id = Guid.NewGuid();
        }

        public static PerfMonitorContext Current
        {
            get
            {
                PerfMonitorContext result = null;
                if (HttpContext.Current != null)
                { result = HttpContext.Current.Items[PerfMonitorContext.ClassName] as PerfMonitorContext; }
                if (result == null)
                {
                    result = new PerfMonitorContext();
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Items[PerfMonitorContext.ClassName] = result;
                    }
                }
                return result;
            }
        }
        public string Format
        {
            get
            {
                string result = WebConfigurationManager.AppSettings[ConfigNameFormat];
                if (string.IsNullOrEmpty(result))
                {
                    result = "{7:N} {0:s} {1} {2} {3} {4}ms {5} - {6}";
                }

                return result;
            }
        }
        public string FilePath
        {
            get
            {
                string result = WebConfigurationManager.AppSettings[ConfigNameFilePath];
                if (string.IsNullOrEmpty(result))
                {
                    result = @"~\App_Data\perflog.log";
                }
                if (result.StartsWith("~"))
                {
                    result = result.Replace("~", HttpRuntime.AppDomainAppPath);
                    result = result.Replace("\\\\", "\\");
                }
                result = string.Format(result, DateTime.Now);
                return result;
            }
        }
        public bool Enabled
        {
            get
            {
                bool result = true;
                var enabledStr = WebConfigurationManager.AppSettings[ConfigNameEnabled];
                if (!string.IsNullOrEmpty(enabledStr) && enabledStr == "false")
                {
                    result = false;
                }
                return result;
            }
        }
        public Guid Id { get; set; }

        public void Write(PerfMonitorLine log)
        {
            if (!this.Enabled)
                return;
            System.IO.Directory.CreateDirectory(new FileInfo(this.FilePath).Directory.FullName);

            try
            {
                using (FileStream fs = new FileStream(this.FilePath, FileMode.OpenOrCreate,
                    System.Security.AccessControl.FileSystemRights.AppendData,
                    FileShare.Write, 4096, FileOptions.None))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.AutoFlush = true;

                        if (log.Id != Guid.Empty)
                            Id = log.Id;

                        var msg = string.Format(this.Format,
                            log.DateTime,
                            log.Method,
                            log.Url,
                            log.Status,
                            log.Duration,
                            log.MemberName,
                            log.Message,
                            Id);
                        writer.WriteLine(msg);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: write Log Error
            }
        }
    }
}
