using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Default.WebUI.Models
{
    public class LayoutEditorModel
    {
        public LayoutEditorModel(string websiteRootPath, string sharedRootPath, string homeRootPath, string widgetsRootPath)
        {
            WebsiteFiles = new FileTree.FileTree(websiteRootPath);
            WebsiteFiles.Name = "website";

            SharedFiles = new FileTree.FileTree(sharedRootPath);
            SharedFiles.Name = "fallback";

            HomeFiles = new FileTree.FileTree(homeRootPath);
            HomeFiles.Name = "fallback";

            WidgetFiles = new FileTree.FileTree(widgetsRootPath);
            WidgetFiles.Name = "widgets";

            ConfigFiles = new FileTree.FileTree();
            ConfigFiles.Name = "config";
        }

        public FileTree.FileTree ConfigFiles { get; set; }

        public FileTree.FileTree WebsiteFiles { get; set; }
        public FileTree.FileTree SharedFiles { get; set; }
        public FileTree.FileTree HomeFiles { get; set; }
        public FileTree.FileTree WidgetFiles { get; set; }

        public string WebConfigFile { get; set; }

        public string CurrentTabName { get; set; }
        public string OpenFilePath { get; set; }
        public string OpenFileContent { get; set; }
        public string Alert { get; set; }
    }


}