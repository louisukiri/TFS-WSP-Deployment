using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ninject;
using toDev.Abstract;
using toDev.Concrete;

namespace toDev.DirectoryHelpers
{
    public abstract class FileDeployment : IFileDeployHandler
    {
        protected StandardKernel Kernel = new StandardKernel();
        protected IList<IFileObject> _Files = new List<IFileObject>();
        protected IList<string> _Directories;
        protected List<string> _sourceDir;
        protected string _deploymentDir;
        protected IDirectoryHelper _dir;

        public abstract IDirectoryHelper Dir { get; set; }
        public FileDeployment() { init(); }
        public FileDeployment(List<string> baseDir, string deploymentDir)
            : this()
        {
            _sourceDir = baseDir;
            _deploymentDir = deploymentDir;
        }
        public virtual void init()
        {
            if (!Kernel.GetBindings(typeof(IFileObject)).Any())
            {
                Kernel.Bind<IFileObject>().To<DefaultFileObject>();
            }
        }
        public IList<string> Directories
        {
            get { return _Directories; }
            set { _Directories = value; }
        }

        #region IFileDeploy Handler implementation

        public IList<IFileObject> FileObjects
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new List<IFileObject>();
                }
                return _Files;
            }
        }
        public virtual void LoadFiles(IList<string> FileList)
        {
            //load each item in the list into a IFileObject
            foreach (string str in FileList)
            {

                string ModifiedFullFileName = str;
                //remove the basedir
                foreach (string str2 in SourceDir)
                {
                    if (str.StartsWith(str2))
                    {
                        ModifiedFullFileName = str.Replace(str2, "");
                    }
                }

                var currentFile = FileObjects.Where(a => a.StoreIndependentFileName == ModifiedFullFileName).FirstOrDefault();
                //if the current file in the list does not exists
                if (currentFile == null)
                {
                    //_Files.Add(new IFileObject())
                    //IFileObject fobj2 = Kernel.Get<IFileObject>();
                    IFileObject fobj2 = new DefaultFileObject();
                    _Files.Add(fobj2.Create(str, ModifiedFullFileName));
                }
            }
        }
        public List<string> SourceDir
        {
            get
            {
                return _sourceDir;
            }
            set
            {
                _sourceDir = value;
            }
        }

        public string DeploymentDir
        {
            get
            {
                return _deploymentDir;
            }
            set
            {
                _deploymentDir = value;
            }
        }
        public virtual string Deploy()
        {
           return Deploy(FileObjects);
        }
        public abstract string Deploy(IList<IFileObject> fobjs);

        #endregion



        public string BuildNumber
        {
            get;
            set;
        }

        public string UID
        {
            get;
            set;
        }

        private string _comment = string.Empty;
        private string _tag = string.Empty;
        public string Tag
        {
            get { return _tag; }
        }
        public string Comment
        {
            get
            {
                return _comment;
            }
        }
        public void SetComment(string str)
        {
                string a = str ;
                Regex reg = new Regex(@"%([^%]+)\%");
                if (reg.IsMatch(a))
                {
                    Match b = reg.Match(a);
                    _tag = Regex.Replace(b.Groups[1].Value.Trim(), @"\W", "_");
                }
                else
                {
                    //if %% isn't found in the comment, use the first 15 characters
                    int charlength = (str.Length > 50) ? 50 : str.Length;
                    _tag = Regex.Replace(str.Substring(0, charlength), @"\W", "_");
                }
                _comment = a;
                //if (string.IsNullOrEmpty(_tag))
                  //  _tag = "Build" + this.BuildNumber;
            //This makes sure the zip file is recreated for every changeset that's hit
                Dir = null;
        }


        public string BuildDir
        {
            get;
            set;
        }

        public virtual void DeleteDeploy()
        {
            DeleteDeploy(this.FileObjects);
        }
        public virtual void DeleteDeploy(IList<IFileObject> fobjs)
        {
            foreach (IFileObject fobj in fobjs)
            {
                string filename = DeploymentDir + fobj.StoreIndependentFileName;
                File.WriteAllText(@"//development.stamats.com/BuildingsDev/_build/test.txt", filename);
                if(File.Exists(filename))
                    File.Delete(filename);
            }
        }
    }

}
