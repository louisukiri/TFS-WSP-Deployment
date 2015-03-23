using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toDev.Abstract;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace toDev.Concrete.DirectoryDeployer
{
    public class WinZipDirectoryHelper : IDirectoryHelper
    {

        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        //
        // Recurses down the folder structure
        //
        private string _zippath;

        public string Zippath
        {
            get { return _zippath; }
        }
        private string _zipName;

        public string ZipName
        {
            get { return _zipName; }
        }

        string zipFile
        {
            get
            {
                return Zippath + ZipName;
            }
        }
        public WinZipDirectoryHelper()
        { }
        public WinZipDirectoryHelper(string zipPath)
        {
            string reg = @"(.*?)[/\\](\w+\.zip)$";
            if (Regex.IsMatch(zipPath, reg))
            {
                createDirectory(Regex.Match(zipPath, reg).Groups[1].Value, Regex.Match(zipPath, reg).Groups[2].Value);
            }
            else
                throw new Exception("invalid zip path");
        }
        public WinZipDirectoryHelper(string zipPath, string zipName)
        {
            createDirectory(zipPath, ZipName);
        }

        private void createDirectory(string zipPath, string zipName)
        {
            _zippath = zipPath;
            _zipName = zipName;

            if (!Directory.Exists(zipPath))
            {
                Directory.CreateDirectory(zipPath);
            }
            //throw new Exception("did not hit");
        }
        #region IDirectoryHelper
        public bool Exists(string Path)
        {
            /*
             var selection = from e in zip.Entries 
                where e.FileName.StartsWith(directoryName)
                select e;
             */
            var selection = Zip.Entries.Where(
                a => a.FileName.StartsWith(Path)
                ).FirstOrDefault();

            return selection == null;
        }
        ZipFile _zip;
        public ZipFile Zip
        {
            get
            {
                if (_zip == null)
                {
                    _zip = new ZipFile(zipFile);
                }
                return _zip;
            }
        }
        public object Create(string Path)
        {
            return Zip.AddDirectoryByName(Path);
        }
        #endregion
    }
}
