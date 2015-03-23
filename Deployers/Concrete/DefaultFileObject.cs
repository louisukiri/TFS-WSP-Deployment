using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployers;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using toDev.Abstract;

namespace toDev.Concrete
{
    /// <summary>
    /// DefaultFIleObject reads data from the TFS server
    /// TFS server URI is made available through the settings
    /// </summary>
    public class DefaultFileObject : IFileObject
    {
        private IList<string> _directories;

        //an array of directories based on the store independent filenames
        //
        public IList<string> Directories
        {
            get
            {
                if (_directories == null)
                {
                    string [] _directoriesArr = StoreIndependentFileName.Split(new string[] { "/", @"\" }, StringSplitOptions.RemoveEmptyEntries);
                    if (_directoriesArr.Length <= 1)
                    {
                        //it's in the base directory
                        _directories = new List<string> { string.Empty };
                    }
                    else
                        _directories = _directoriesArr.Take(_directoriesArr.Length - 1).ToList();
                }
                return _directories;
            }
        }
        private string _fullfilename = string.Empty;
        private string _filename = string.Empty;
        private string _storeIndependentFileName = string.Empty;

        /// <summary>
        /// The fullfilename (Source Format). For example, This will be the TFS formatted filename for TFS FileObject(the defaultfileobjec)
        /// </summary>
        public string FullFileName
        {
            get
            {
                return _fullfilename;
            }
        }

        public string FileName
        {
            get 
            {
                if (string.IsNullOrEmpty(_filename) && (!String.IsNullOrEmpty(StoreIndependentFileName) || !string.IsNullOrEmpty(FullFileName)))
                {
                    string fileName = (string.IsNullOrEmpty(StoreIndependentFileName)) ? FullFileName : StoreIndependentFileName;
                    _filename = fileName.Split(new string[] { "/", @"\" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                }
                return _filename;
            }
        }

        private Stream _filestream;
        /// <summary>
        /// This class gets the file stream from TFS
        /// during Adds, the files dont exist in TFS and this results in the returning of a null string
        /// </summary>
        public virtual System.IO.Stream FileStream
        {
            get 
            {
                if (!string.IsNullOrEmpty(this.FullFileName) && _filestream == null)
                {
                    _filestream = get(this.FullFileName);
                    /*
                    TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(TFSSettings.TFSServerURL));
                    VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));
                    string fName = this.FullFileName;
                    Item it = vcs.GetItems(fName).Items.FirstOrDefault();
                    if (it != null)
                    {
                        _filestream = it.DownloadFile();
                    }*/
                }
                return _filestream;
                
            }
        }
        /// <summary>
        /// This class gets the file stream from TFS
        /// during Adds, the files dont exist in TFS and this results in the returning of a null string
        /// </summary>
        public static System.IO.Stream get(string FullFileName)
        {
            return get(TFSSettings.TFSServerURL, FullFileName);
        }
        /// <summary>
        /// This class gets the file stream from TFS
        /// during Adds, the files dont exist in TFS and this results in the returning of a null string
        /// </summary>
        public static System.IO.Stream get(string serverURL, string FullFileName)
        {
            Stream fileStream = null;
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(serverURL));
            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));
            string fName = FullFileName;
            Item it = vcs.GetItems(fName).Items.FirstOrDefault();
            if (it != null)
            {
                fileStream = it.DownloadFile();
            }
            return fileStream;
        }
        public virtual void Close()
        {
            if (_filestream != null)
            {
                _filestream.Close();
                _filestream = null;
            }
        }
        public IFileObject Create(string FullFName, string storeIndependentFName)
        {
            return new DefaultFileObject { _fullfilename = FullFName.Replace("/", @"\"), _storeIndependentFileName = storeIndependentFName.Replace("/", @"\") };
        }


        public string StoreIndependentFileName
        {
            get { return _storeIndependentFileName; }
        }


        public void Load(string fullfileName, string storeIndependentFileName)
        {
            this._fullfilename = fullfileName;
            this._storeIndependentFileName = storeIndependentFileName;
        }
    }
}
