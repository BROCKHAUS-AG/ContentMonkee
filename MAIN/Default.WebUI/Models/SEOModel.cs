using BAG.Framework.TagCloud;
using LemmaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{

    public class SEOModel
    {
        public SEOModel()
        {
            Keywords = new List<SEOKeywordModel>();
            WordCount = 0;
            Content = "";
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SEOKeywordModel> Keywords { get; set; }
        public int WordCount { get; set; }
        public int WordCountFirstWidget { get; set; }
        public string Content { get; set; }


        public IEnumerable<BAG.Framework.TagCloud.TagCloudTag> GetTags()
        {

            var analyzer = new TagCloudAnalyzer(new TagCloudSetting(LanguagePrebuilt.German));

            var tags = analyzer.ComputeTagCloud(Content.Split(new char[] { ' ' }));

            tags = tags.Shuffle();
            return tags;
        }
    }
    public class SEOKeywordModel
    {
        public SEOKeywordModel()
        {
            Keyword = string.Empty;
            Count = 0;
            IsInTitle = false;
            IsInMetaDescription = false;
        }
        public string Keyword { get; set; }
        public int CountFirstWidget { get; set; }
        public int Count { get; set; }
        public bool IsInFirstWidget { get { return CountFirstWidget != 0; } }
        public bool IsInContent { get { return Count != 0; } }
        public bool IsInTitle { get; set; }
        public bool IsInMetaDescription { get; set; }
        public bool IsInUrl { get; set; }
    }
}