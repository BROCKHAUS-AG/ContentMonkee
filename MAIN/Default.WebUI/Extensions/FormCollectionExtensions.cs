using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System.Globalization;

namespace Default.WebUI.Extensions
{
    public static class FormCollectionExtensions
    {

        public static bool GetBool(this FormCollection collection, string name, bool defaultValue = false)
        {
            bool result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var value = collection[name];
                result = value == "on" || value == "true,false" || value == "true";
            }
            else
            {
                result = false;
            }
            return result;
        }



        public static string GetColorString(this FormCollection collection, string name, string defaultValue = "#777777")
        {
            string result = GetString(collection, name, defaultValue);
            result = result.ToUpperInvariant();
            try
            {
                bool isValidColor = result.StartsWith("#") & (result.Length == 7 | result.Length == 4) & result.Substring(1).All(c => "ABCDEF0123456789".IndexOf(Char.ToUpper(c)) != -1);
                if (!isValidColor)
                {
                    result = defaultValue;
                }
            }
            catch (Exception)
            {
                result = defaultValue;
            }

            return result;
        }

        public static string GetString(this FormCollection collection, string name, string defaultValue = "")
        {
            string result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                result = collection[name];
            }
            return result;
        }

        public static E GetEnum<E>(this FormCollection collection, string name, E defaultValue) where E : struct
        {
            E result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                if (Enum.TryParse<E>(str, out result))
                {
                    //success
                }
            }
            return result;
        }

        public static double GetDouble(this FormCollection collection, string name, double defaultValue = 0, string replace = null)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                if (!string.IsNullOrEmpty(replace))
                    str = str.Replace(replace, "").Trim();
                
                var style = NumberStyles.Number | NumberStyles.AllowDecimalPoint;
                var culture = CultureInfo.InvariantCulture;
                double.TryParse(str, style, culture, out result);
            }
            return result;
        }

        public static double ConvertDouble(this FormCollection collection, string value, double defaultValue = 0)
        {
            var result = defaultValue;
            double.TryParse(value, out result);
            return result;
        }

        public static int GetInt(this FormCollection collection, string name, int defaultValue = 0)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                int.TryParse(str, out result);
            }
            return result;
        }

        public static DateTime GetDate(this FormCollection collection, string name, DateTime defaultValue)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                if (!string.IsNullOrEmpty(str))
                    DateTime.TryParse(str, out result);

                result = result.Fix();
            }
            return result;
        }

        public static DateTime Min = new DateTime(1753, 1, 1);
        public static DateTime Max = new DateTime(2100, 1, 1);


        public static DateTime Fix(this DateTime date)
        {
            if (date < Min)
                return Min;
            if (date > Max)
                return Max;
            return date;
        }
        public static DateTime Fix(this DateTime date, DateTime min)
        {
            if (date < Min)
                return min;
            if (date > Max)
                return Max;
            return date;
        }

        public static DateTime? GetNullableDate(this FormCollection collection, string name, DateTime? defaultValue)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                DateTime date = DateTime.Now;
                var str = collection[name];
                if (!string.IsNullOrEmpty(str))
                    if (DateTime.TryParse(str, out date))
                    {
                        result = date;
                    }
            }
            return result;
        }

        public static TimeSpan GetTimeSpan(this FormCollection collection, string name, TimeSpan defaultValue, string replace = null)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                if (!string.IsNullOrEmpty(replace))
                    str = str.Replace(replace, "").Trim();
                if (!string.IsNullOrEmpty(str))
                    TimeSpan.TryParse(str, out result);
            }
            return result;
        }

        public static Guid GetGuid(this FormCollection collection, string name, Guid defaultValue)
        {
            var result = defaultValue;
            if (collection.AllKeys.Contains(name))
            {
                var str = collection[name];
                if (!string.IsNullOrEmpty(str))
                    Guid.TryParse(str, out result);
            }
            return result;
        }

        public static List<Guid> GetGuidList(this FormCollection collection, string name)
        {
            var result = new List<Guid>();
            if (name.EndsWith("*"))
            {
                var prefix = name.TrimEnd('*');
                foreach (var key in collection.AllKeys)
                {
                    if (key.StartsWith(prefix))
                    {
                        var s = collection[key];
                        Guid g = Guid.Empty;
                        if (!string.IsNullOrEmpty(s))
                            if (Guid.TryParse(s, out g))
                            {
                                if (!result.Contains(g))
                                    result.Add(g);
                            }
                    }
                }
            }
            else
            {
                if (collection.AllKeys.Contains(name))
                {
                    var strList = collection[name];
                    var strArray = strList.Split(',');
                    foreach (var item in strArray)
                    {
                        var s = item.Trim();
                        Guid g = Guid.Empty;
                        if (!string.IsNullOrEmpty(s))
                            if (Guid.TryParse(s, out g))
                            {
                                if (!result.Contains(g))
                                    result.Add(g);
                            }
                    }

                }
            }
            return result;
        }
    }
}