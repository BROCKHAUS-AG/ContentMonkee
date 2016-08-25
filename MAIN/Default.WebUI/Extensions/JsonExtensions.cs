using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this Object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
            settings.Converters.Add(new JavaScriptDateTimeConverter());
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}