using Microsoft.VisualStudio.TestTools.UnitTesting;
using BAG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAG.Common.Data.Entities;
using System.Web;
using System.IO;

namespace BAG.Common.Data.Tests
{
    [TestClass()]
    public class UnitOfWorkTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://tempuri.org", null),
                new HttpResponse(null));
        }

        [TestMethod()]
        public void DISTRIBUTED_DATA_Test()
        {

            List<WidgetManager> awms = new List<WidgetManager>();
            List<SiteManager> asms = new List<SiteManager>();
            IEnumerable<Guid> ssids = new List<Guid>();

            {
                _Globals.Instance.ChangeSiteSettingId(Guid.Empty, null);
                Context.DistributedData = false;
                UnitOfWork unit = new UnitOfWork();
                awms = unit.WidgetManagerRepository.Get().ToList();
                asms = unit.SiteManagerRepository.Get().ToList();
                ssids = unit.SiteSettingRepository.Get().Select(ss => ss.Id);
            }

            foreach (Guid ssid in ssids)
            {
                _Globals.Instance.ChangeSiteSettingId(ssid,null);
                Context.DistributedData = false;
                UnitOfWork unit = new UnitOfWork();
                foreach (Guid id in asms.Where(s => s.SiteSettingId != ssid).Select(s => s.Id))
                {
                    if (id==null)
                    {
                        continue;
                    }
                    unit.SiteManagerRepository.Delete(id);
                }
                foreach (Guid id in awms.Where(w => w.SitesettingId != ssid).Select(w => w.Id))
                {
                    if (id == null)
                    {
                        continue;
                    }
                    unit.WidgetManagerRepository.Delete(id);
                }
                Context.DistributedData = true;
                unit.Save();
            }
            foreach (Guid ssid in ssids)
            {
                _Globals.Instance.ChangeSiteSettingId(ssid,null);
                Context.DistributedData = true;
                UnitOfWork unit = new UnitOfWork();

                foreach (WidgetManager wm in unit.WidgetManagerRepository.Get(w => true).ToList())
                {
                    IEnumerable<WidgetManager> rwm = awms.Where(w => w.Id == wm.Id);
                    if (rwm.Count() != 1)
                    {
                        Assert.Fail();
                    }
                    Assert.IsTrue(awms.Remove(rwm.First()));
                }
                foreach (SiteManager sm in unit.SiteManagerRepository.Get(s => true).ToList())
                {
                    IEnumerable<SiteManager> rsm = asms.Where(s => s.Id == sm.Id);
                    if (rsm.Count() != 1)
                    {
                        Assert.Fail();
                    }
                    Assert.IsTrue(asms.Remove(rsm.First()));
                }
            }

            foreach (SiteManager sm in asms)
            {
                Assert.IsFalse(ssids.Contains(sm.SiteSettingId));
            }
            foreach (WidgetManager wm in awms)
            {
                Assert.IsFalse(ssids.Contains(wm.SitesettingId));
            }
        }
        
    }
}