using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toDev.Abstract;
using toDev.Concrete.DirectoryDeployer;
using toDev.DirectoryHelpers;
using Ninject;
using System.IO;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace toDev.Concrete.ZipDeployer
{
    public class WinZipDeploy : FileDeployment
    {
        /// <summary>
        /// Zip Deploy Constructor
        /// </summary>
        /// <param name="BaseDir">The base directory of the source file being deployed</param>
        /// <param name="DeploymentDir">The directory where the file is being deployed to (target directory)</param>
        /// <param name="BuildDir">The directory where the build artifact (the result of running the build) is stored</param>
        /// <param name="BuildNumber">A number used in the naming the build artifact when there's no descriptive title</param>
        public WinZipDeploy(List<string> SourceDir, string DeploymentDir, string BuildDir, string BuildNumber)
        {
            this.SourceDir = SourceDir;
            this.DeploymentDir = DeploymentDir;
            this.BuildDir = BuildDir;
            this.BuildNumber = BuildNumber;

        }
        public override IDirectoryHelper Dir
        {
            get
            {
                if (_dir == null)
                {

                    if (!Kernel.GetBindings(typeof(IDirectoryHelper)).Any())
                        Kernel.Bind<IDirectoryHelper>().To<ZipDirectoryHelper>().WithConstructorArgument("zipPath", string.Format(BuildDir + @"\{0}.zip", Tag));
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
            ZipDirectoryHelper zdh = Dir as ZipDirectoryHelper;
            string logStr = string.Format("Deploying {0} to {1} \r\n Creating zip...\r\n", Tag, DeploymentDir);

            try
            {
                foreach (IFileObject fobj in fobjs)
                {
                    string fulldeployedFileName = DeploymentDir + fobj.StoreIndependentFileName;
                    if (!fobj.StoreIndependentFileName.StartsWith("$"))
                    {
                        using (Stream stream = fobj.FileStream)
                        {
                            //stream is null on deletes
                            if (stream != null)
                            {
                                using (ZipFile zip = zdh.Zip)
                                {
                                    string filename = fobj.StoreIndependentFileName;

                                    if (!zip.ContainsEntry(filename))
                                    {
                                        logStr += string.Format("\tadding file {0} \r\n", filename);
                                        ZipEntry ze = zip.AddEntry(filename, stream);
                                        zip.Save();
                                    }
                                    else
                                    {
                                        logStr += string.Format("\tupdating file {0} \r\n", filename);
                                        zip.RemoveEntry(filename);
                                        zip.AddEntry(filename, stream);
                                        zip.Save();
                                    }
                                }

                            }
                            else if (stream == null && File.Exists(fulldeployedFileName))
                            {

                                logStr += string.Format("{0} cannot be found in tfs. This might be due to a deletion in TFS", fulldeployedFileName);
                                //remove file from zip
                                using (ZipFile zip = zdh.Zip)
                                {
                                    if (zip.ContainsEntry(fobj.StoreIndependentFileName))
                                    {
                                        logStr += string.Format("removing file {0} \r\n", fobj.StoreIndependentFileName);
                                        zip.RemoveEntry(fobj.StoreIndependentFileName);
                                        zip.Save();
                                    }
                                }
                                File.Delete(fulldeployedFileName);
                            }
                            //zip.Save();
                        }
                        fobj.Close();

                    }
                }
                using (ZipFile zip = zdh.Zip)
                {
                    //if (!Directory.Exists(DeploymentDir)) { Directory.CreateDirectory(DeploymentDir); }// throw new Exception("Deployment failed because directory " + DeploymentDir + " does not exist. Create the directory and try again");

                    logStr += string.Format("Deploying to {0} \r\n", DeploymentDir);
                    zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;

                    zip.ExtractAll(DeploymentDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception e)
            {
                logStr += string.Format("\r\nException: {0} at {1}\r\n\t{2}\r\n", e.Message, e.Source, e.StackTrace);
                throw new Exception(e.Message);
            }
            finally
            {
                zdh.Zip.Dispose();
                zdh = null;
                //File.WriteAllText(string.Format(@"{0}\{1}.{2}", BuildDir, Tag + BuildNumber, "txt"), logStr);
            }/**/
            return logStr;
        }
    }
}
