using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Default.WebUI.Extensions;

namespace Default.WebUI.Models
{
    public class LayoutGroup
    {
        public LayoutGroup(List<string> viewPaths)
        {
            Views = new List<LayoutView>();

            foreach (var viewPath in viewPaths)
            {
                var fileInfo = new FileInfo(viewPath);
                var newViewItem = new LayoutView
                {
                    Name = fileInfo.Name,
                    VirtualPath = viewPath.ToVirtualPath()
                };

                Views.Add(newViewItem);
            }

            Initialize();
        }

        private void Initialize()
        {
            var layoutView = Views.FirstOrDefault();
            if (layoutView != null)
            {
                var fullViewPath = HttpContext.Current.Server.MapPath(layoutView.VirtualPath);
                OpenFilePath = layoutView.VirtualPath;

                if(File.Exists(fullViewPath))
                    OpenFileContent = System.IO.File.ReadAllText(fullViewPath);
            }
        }

        public List<LayoutView> Views { get; set; }

        public string OpenFilePath { get; set; }

        public string OpenFileContent { get; set; }
    }

    public class LayoutView
    {
        public string Name { get; set; }

        public string VirtualPath { get; set; }
    }
}