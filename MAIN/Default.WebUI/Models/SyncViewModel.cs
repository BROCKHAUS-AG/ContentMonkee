using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Default.WebUI.Models
{
    public class SyncViewModel
    {
        public SyncViewModel()
        {
            Items = new List<SyncItem>();
        }
        public List<SyncItem> Items { get; set; }
    }

    public class SyncItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string DownloadUrl { get; set; }
        public long Size { get; set; }
        public string Hash { get; set; }
    }
}