using BAG.Common;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Default.WebUI.Models
{
    public class AdminWidgetViewModel : IWidgetContainer
    {
        public AdminWidgetViewModel()
        {
            Sites = new List<Site>();
            Users = new List<User>();
            Widgets = new List<Widget>();
            Layouts = new List<System.IO.FileInfo>();

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
        public List<Site> Sites { get; set; }
        public List<User> Users { get; set; }
        public List<Widget> Widgets { get; set; }
        public List<Widget> NewWidgets { get; set; }
        public Widget CurrentWidget { get; set; }
        public List<Employee> Employees { get; set; }
        public List<System.IO.FileInfo> Layouts { get; set; }
        public Guid SiteId
        {
            get { return Guid.Empty; }
        }
        public Guid CurrentSiteId { get; set; }
        public Guid CurrentWidgetId { get; set; }
        public bool IsNavigation { get; internal set; }
        public string[] WidgetsList { get; set; }

        public SEOModel SEOModel { get; set; }
        public SiteSetting CurrentSiteSetting { get; set; }
        public List<WidgetManager> WidgetManagers { get; set; }

        public void SetSEOModel(Widget widget)
        {
            SEOModel result = new SEOModel();
            string keywordsString = (widget.MetaKeywords == null ? string.Empty : widget.MetaKeywords).ToLower();
            IEnumerable<string> keywords = keywordsString.Split(',').Select(s => s.Trim());

            string content = widget.GetContentExceptTags().ToLower();
            content = content == null ? string.Empty : content;
            content = content.Replace("-\n", "").Replace("- ", "");

            result.Title = widget.MetaTitle == null ? string.Empty : widget.MetaTitle;
            result.Description = widget.MetaDescription == null ? string.Empty : widget.MetaDescription;
            result.WordCount = Regex.Matches(content, @"[\w\-_]+").Count;
            result.WordCountFirstWidget = -1;
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
                keywordModel.CountFirstWidget = -1;
                keywordModel.IsInMetaDescription = result.Description.Contains(keyword);
                keywordModel.IsInTitle = result.Title.Contains(keyword);
                keywordModel.IsInUrl = widget.Url.Contains(keyword);
                result.Keywords.Add(keywordModel);
            }
            this.SEOModel = result;
        }

    }
}