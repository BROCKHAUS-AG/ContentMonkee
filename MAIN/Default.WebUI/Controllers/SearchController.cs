using BAG.Common.Data;
using BAG.Common.Data.Entities;
using Default.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Default.WebUI.Extensions;
using BAG.Common;
using BAG.Common.Authorization;

namespace Default.WebUI.Controllers
{
    [System.Web.Mvc.RoutePrefix("search/{action}")]
    [WebAnonymousAttribute()]
    public class SearchController : Controller
    {
        [Obsolete]
        private UnitOfWork unit = null;
        private UnitOfWork Unit
        {
            get
            {
#pragma warning disable 612, 618
                if (unit == null)
                {
                    this.unit = new UnitOfWork();
                }
                return this.unit;
#pragma warning restore 612, 618
            }
            set
            {
#pragma warning disable 612, 618
                this.unit = Unit;
#pragma warning restore 612, 618
            }
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new EmptyResult();

            var keys = q.Split(' ', '&');
            var model = new GlobalViewModel();


            var sitemanagers = Unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId && s.Release != null);

            foreach (var sitemanager in sitemanagers)
            {
                var site = sitemanager.Release;
                site.OnLoad(Unit); //load widgets - property data

                int quality = 0;
                foreach (var skey in keys)
                {
                    var key = skey.Trim();
                    if (site.Name.ContainsIgnoreCase(key))
                    {
                        quality++;
                    }

                    if (site.Keywords.ContainsIgnoreCase(key))
                    {
                        quality++;
                    }

                    if (site.Description.ContainsIgnoreCase(key))
                    {
                        quality++;
                    }

                    foreach (var widgetpart in site.Widgets)
                    {
                        var widget = widgetpart.Release;
                        if (widget.GetContent().ContainsIgnoreCase(key))
                        {
                            quality++;
                        }
                        if (widget.Name.ContainsIgnoreCase(key))
                        {
                            quality++;
                        }
                    }
                }
                if (quality > 0)
                {
                    model.Results.Add(new SearchResult { Site = site, Quality = quality });
                }
            }
            var sitemanagersmodel = Unit.SiteManagerRepository.Get(s => s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId && s.Release != null).ToList();
            var sites = new List<Site>();
            sitemanagersmodel.ForEach(s => sites.Add(s.Release));
            model.Sites = sites;
            model.SiteSetting = Unit.SiteSettingRepository.GetByID(_Globals.Instance.CurrentSiteSettingId);
            model.IsPreview = false;

            return View(model);
        }

        //[System.Web.Mvc.HttpGet]
        //[System.Web.Mvc.HttpPost]
        public ActionResult Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new string[] { }, JsonRequestBehavior.AllowGet);
            }
            string lastTerm = term.Split(' ').Last();


            var resultsUnfolded = from s in Unit.SiteManagerRepository.FindAll(r => r.Release != null && r.Release.Keywords != null)
                                  from k in s.Release.Keywords.Split(' ')
                                  where k.ContainsIgnoreCase(lastTerm) &&
                                            s.SiteSettingId == _Globals.Instance.CurrentSiteSettingId
                                  select k;

            var resultsRanked = from f in resultsUnfolded.Distinct()
                                select
                                new
                                {
                                    keyword = f,
                                    count = resultsUnfolded.Where(u => f.ContainsIgnoreCase(u)).Count()
                                };

            resultsRanked.OrderBy(f => f.count);


            var results = resultsRanked.Take(10).Select(r => r.keyword);

            //int id = 0;
            //List<Autocomplete> results = new List<Autocomplete>();

            //foreach (Autocomplete a in resultsRanked.Take(10).Select(r => new Autocomplete() { Name = r.keyword, Id = id++ }))
            //{
            //    results.Add(a);
            //}
            
            return Json(results, JsonRequestBehavior.AllowGet);
        }        
    }

}