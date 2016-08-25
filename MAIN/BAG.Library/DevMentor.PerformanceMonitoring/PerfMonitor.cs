using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMentor.PerformanceMonitoring
{
    public class PerfMonitor : IDisposable
    {
        Stopwatch stopwatch;
        PerfMonitorContext perfLog;
        PerfMonitorLine line;
        public PerfMonitor([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
            : base()
        {
            perfLog = PerfMonitorContext.Current;
            this.stopwatch = Stopwatch.StartNew();
            line = new PerfMonitorLine();
            line.MemberName = memberName;
        }

        public PerfMonitorLine Line { get { return line; } }

        public void Dispose()
        {
            if (line.Status == 0)
                line.Status = System.Runtime.InteropServices.Marshal.GetExceptionCode();
            stopwatch.Stop();
            line.Duration = stopwatch.ElapsedMilliseconds;
            perfLog.Write(line);
        }
    }
}
