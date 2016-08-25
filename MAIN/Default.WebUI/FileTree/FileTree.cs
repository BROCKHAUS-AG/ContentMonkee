using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Default.WebUI.FileTree
{
    public class FileTree
    {
        private string _rootPath;

        public FileTree(string rootPath)
        {
            _rootPath = rootPath;

            Siblings = new List<FilesystemElement>();

            foreach (var file in Directory.GetFileSystemEntries(rootPath))
            {
                var newSibling = new FilesystemElement(file, this);
                Siblings.Add(newSibling);
            }
        }

        public FileTree()
        {
            Siblings = new List<FilesystemElement>();
        }

        public string Name { get; set; }

        public string Root
        {
            get
            {
                return _rootPath;
            }
        }

        public void Insert(string path)
        {
            var newElement = new FilesystemElement(path, this);
            Siblings.Add(newElement);
        }

        public override string ToString()
        {
            var markup = "<ul class=\"file-tree\">";
            foreach (var element in Siblings)
            {
                markup += element;
            }
            return markup += "</ul>";
        }

        public List<FilesystemElement> Siblings { get; set; }
    }
}