using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Default.WebUI.FileTree
{
    public class FilesystemElement
    {
        private readonly FileInfo _fileInfo;

        private FileTree _treeRoot;

        public FilesystemElement(string path, FileTree treeRoot)
        {
            _fileInfo = new FileInfo(path);
            _treeRoot = treeRoot;

            Title = _fileInfo.Name;
            Name = _fileInfo.Name;
            Type = IsDir ? ElementType.Directory : ElementType.File;

            Siblings = new List<FilesystemElement>();

            Initialize(path);
        }

        private void Initialize(string path)
        {
            if (IsDir)
            {
                var files = Directory.GetFileSystemEntries(path);
                foreach (var file in files)
                {
                    var newSibling = new FilesystemElement(file, _treeRoot);
                    Siblings.Add(newSibling);
                }
            }
            else
            {
                VirtualPath = ToVirtualPath(path);
            }
        }

        private bool IsDir
        {
            get { return (_fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory; }
        }

        public string HtmlId
        {
            get { return _treeRoot.Name + "_" + "folder_" + Name.Replace(".", "_"); } 
        }

        public string VirtualPath { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public ElementType Type { get; set; }

        public List<FilesystemElement> Siblings { get; set; }

        private static string ToVirtualPath(string stringValue)
        {
            return "~/" + stringValue.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], string.Empty).Replace("\\", "/");
        }

        public override string ToString()
        {
            var markup = "<li>";

            markup += IsDir 
                ? "<a role=\"button\" data-toggle=\"collapse\" href=\"#" + HtmlId + "\"><span class=\"glyphicon glyphicon-folder-close\"></span> " + Name + "</a>"
                : "<a href=\"?name=" + VirtualPath + "&selectedTab=" + _treeRoot.Name + "\" class=\"le-file-element\" data-virtual-path=\"" + VirtualPath + "\"><span class=\"glyphicon glyphicon-file\"></span> " + Name + "</a>";

            if (Siblings.Any())
            {
                var listMarkup = string.Format("<ul id=\"{0}\" {1}>", HtmlId,
                    IsDir ? "class=\"collapse\"" : string.Empty);
               
                foreach (var element in Siblings)
                {
                    listMarkup += element;
                }
                listMarkup += "</ul>";
                markup += listMarkup;
            }

            return (markup += "</li>");
        }
    }
}