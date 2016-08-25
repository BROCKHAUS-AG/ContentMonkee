using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Default.WebUI.Classes
{
    public class DirectoryManagement
    {
        private string fileName, fileNameOld;
        private Guid folderName;
        public DirectoryManagement(string fileName, string fileNameOld, Guid folderName)
        {
            this.fileName = fileName;
            this.fileNameOld = fileNameOld;
            this.folderName = folderName;
        }

        public DirectoryManagement()
        {
        }
        public void Copy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                Directory.CreateDirectory(sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, false);
                }
                catch (Exception) { }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    Copy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void CreateFolderAndFile(string path)
        {
            try
            {
                fileName = Regex.Replace(fileName, @"[\\/:*?""<>|]", string.Empty);
                fileNameOld = Regex.Replace(fileNameOld, @"[\\/:*?""<>|]", string.Empty);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Regex-failed" + e);
            }
            var localfileName = fileName + ".txt";
            var localfileNameOld = fileNameOld + ".txt";
            string fullPath = Path.Combine(path, localfileName);
            string fullPathOld = Path.Combine(path, localfileNameOld);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                if (!File.Exists(fullPath))
                {
                    using (File.Create(fullPath)) { }
                }
            }
            else
            {
                if (fileNameOld != fileName)
                {
                    using (File.Create(fullPath)) { }
                    File.Delete(fullPathOld);
                }
            }
        }
        public void Move(string source, string destination)
        {
            Directory.Move(source, destination);
        }
    }
}