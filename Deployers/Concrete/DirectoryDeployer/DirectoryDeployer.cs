using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toDev.Abstract;
using toDev.DirectoryHelpers;
using Ninject;
using System.IO;

namespace toDev.Concrete.DirectoryDeployer
{
    public class DirectoryDeployer : FileDeployment
    {

        public DirectoryDeployer()
        {
            Kernel.Bind<IFileObject>().To<DefaultFileObject>();
        }
        public override IDirectoryHelper Dir
        {
            get
            {
                if (_dir == null)
                {
                    if (!Kernel.GetBindings(typeof(IDirectoryHelper)).Any())
                        Kernel.Bind<IDirectoryHelper>().To<FolderDirectoryHelper>();
                    _dir = Kernel.Get<IDirectoryHelper>();
                }
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }
        public override string Deploy(IList<IFileObject> fobjs)
        {
            //dont want to have to search directories multiple times, save found directories here
            List<string> existingDirs = new List<string>();


            //To deploy
            //determine the new directory based on the current directory and the basedirectory
            //base directory is based on the expected directory of the FileObjects
            //base direcotry is a subset of the the expected directory of the FileObjects
            //loop through each directory after the base directory and check the deployment directory if they exist
            //If they dont, create
            //else go to next
            //at the end of directory, write file
            foreach (IFileObject fobj in fobjs)
            {
                string FullDirectory = DeploymentDir + @"\" + fobj.Directories.Aggregate((s, j) => s + @"\" + j);

                if (!Dir.Exists(FullDirectory))
                {

                    object dinf = Dir.Create(FullDirectory);
                }
                using (Stream stream = fobj.FileStream)
                {
                    if(stream != null)
                        using (FileStream fs = File.Create(DeploymentDir + @"\" + fobj.StoreIndependentFileName))
                        {
                            byte[] buf = new byte[8192];

                            for (; ; )
                            {
                                int numRead = stream.Read(buf, 0, buf.Length);
                                if (numRead == 0)
                                    break;
                                fs.Write(buf, 0, numRead);
                            }
                        }
                }

            }
            return "String Deployment";
        }
    }
}
