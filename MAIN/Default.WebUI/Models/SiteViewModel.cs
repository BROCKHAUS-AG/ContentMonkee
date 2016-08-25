using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BAG.Common.Data.Entities;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using BAG.Framework.TagCloud;
using BAG.Framework.TagCloud.Properties;
using LemmaSharp;

namespace Default.WebUI.Models
{
    public class SiteViewModel
    {
        public SiteViewModel()
        {
            Widgets = new List<Widget>();
            NewWidgets = new List<Widget>();

            var type = typeof(Widget);
            var attributes = type.GetCustomAttributes(typeof(XmlIncludeAttribute), true).ToList();
            attributes.ForEach(a =>
                {
                    var t = ((XmlIncludeAttribute)a).Type;
                    NewWidgets.Add((Widget)Activator.CreateInstance(t));
                }
            );
            NewWidgets = NewWidgets.OrderBy(o => o.Name).ToList();
        }
        public Site Site { get; set; }

        //All Widgets
        public List<Widget> Widgets { get; set; }
        //New Widgets
        public List<Widget> NewWidgets { get; set; }
        public bool IsNavigation { get; set; }
        public string TrackingCode { get; set; }
        public string[] WidgetsList { get; set; }

        public SiteSetting CurrentSiteSetting { get; set; }


        public SEOModel SEOModel { get; private set; }
        public bool IsDistinct { get; set; }

        public void SetSEOModel(Site site, List<Widget> widgets)
        {
            if (widgets == null || widgets.Count < 1)
            {
                SEOModel = new SEOModel();
                return;
            }
            SEOModel result = new SEOModel();
            var keywordsString = (site.Keywords == null ? string.Empty : site.Keywords).ToLower();
            var keywords = keywordsString.Split(',').Select(s => s.Trim());
            var firstContent = widgets.First().GetContentExceptTags().ToLower();
            firstContent = firstContent == null ? string.Empty : firstContent;
            var content = widgets.Select(w => w.GetContentExceptTags())
                                 .Aggregate((acc, s) => acc + (s == null ? string.Empty : s)).ToLower();
            content = content.Replace("-\n", "").Replace("- ","");
            firstContent = content.Replace("-\n", "").Replace("- ", "");

            result.Title = site.Title == null ? string.Empty : site.Title;
            result.Description = site.Description == null ? string.Empty : site.Description;
            result.WordCount = Regex.Matches(content, @"[\w\-_]+").Count;
            result.WordCountFirstWidget = Regex.Matches(firstContent, @"[\w\-_]+").Count;
            result.Content = content;

            foreach (string keyword in keywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }
                SEOKeywordModel keywordModel = new SEOKeywordModel();
                keywordModel.Keyword = keyword;
                keywordModel.Count = Regex.Matches(content, keyword).Count;
                keywordModel.CountFirstWidget = Regex.Matches(firstContent, keyword).Count;
                keywordModel.IsInMetaDescription = result.Description.Contains(keyword);
                keywordModel.IsInTitle = result.Title.Contains(keyword);
                keywordModel.IsInUrl = site.Url.Contains(keyword);
                result.Keywords.Add(keywordModel);
            }
            this.SEOModel = result;
        }


        private IWidgetContainer widgetContainer = null;
        public IWidgetContainer WidgetContainer
        {
            get
            {
                if (widgetContainer == null)
                    widgetContainer = new WidgetContainer(Site);
                return widgetContainer;
            }

        }
    }
}