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
    public class ZipDeploy : FileDeployment
    {
        /// <summary>
        /// Zip Deploy Constructor
        /// </summary>
        /// <param name="BaseDir">The base directory of the source file being deployed</param>
        /// <param name="DeploymentDir">The directory where the file is being deployed to (target directory)</param>
        /// <param name="BuildDir">The directory where the build artifact (the result of running the build) is stored</param>
        /// <param name="BuildNumber">A number used in the naming the build artifact when there's no descriptive title</param>
        public ZipDeploy(List<string> SourceDir, string DeploymentDir, string BuildDir, string BuildNumber)
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
            return DeployListOf(fobjs);
        }

        private string DeployListOf(IList<IFileObject> fobjs)
        {

            ZipDirectoryHelper zdh = Dir as ZipDirectoryHelper;
            string logStr = string.Format("\r\n Deploying {0} to {1} \r\nPackaging to {2}\r\n", Tag, DeploymentDir, zdh.zipFile);
            string logStr2 = string.Empty;
            try
            {
                foreach (IFileObject fobj in fobjs)
                {
                    
                    string fulldeployedFileName = DeploymentDir + fobj.StoreIndependentFileName;
                    if (!fobj.StoreIndependentFileName.StartsWith("$"))
                    {
                        using (
                        Stream stream = fobj.FileStream
                            )
                        {
                            using (ZipFile zip = new ZipFile(zdh.zipFile))
                            {
                                ZipEntry ze = null;
                                //stream is null on deletes
                                if (stream != null)
                                {
                                    string filename = fobj.StoreIndependentFileName;
                                    if (!zip.ContainsEntry(filename))
                                    {
                                        logStr += string.Format("\t     adding file {0} \r\n", filename);
                                        ze = zip.AddEntry(filename, stream);
                                    }
                                    else
                                    {
                                        logStr += string.Format("\t     updating file {0} \r\n", filename);
                                        //zip.RemoveEntry(filename);
                                        //zip.UpdateFile(filename);
                                        ze = zip.UpdateEntry(filename, stream);
                                        //zip.AddEntry(filename, stream);
                                    }

                                }
                                else if (stream == null && File.Exists(fulldeployedFileName))
                                {

                                    logStr += string.Format("\t     {0} cannot be found in tfs. This might be due to a deletion in TFS", fulldeployedFileName);
                                    //remove file from zip
                                    //using (ZipFile zip = zdh.Zip)
                                    //{
                                    if (zip.ContainsEntry(fobj.StoreIndependentFileName))
                                    {
                                        logStr += string.Format("\t     removing file {0} \r\n", fobj.StoreIndependentFileName);
                                        zip.RemoveEntry(fobj.StoreIndependentFileName);
                                        //zip.Save();
                                    }
                                    //}
                                    File.Delete(fulldeployedFileName);
                                }
                                zip.Save();
                                if (ze != null)
                                {
                                    ze.Extract(DeploymentDir, ExtractExistingFileAction.OverwriteSilently);
                                    logStr2 +=string.Format("     \tsuccessfully deployed {0} to {1}",fobj.FullFileName,DeploymentDir+fobj.StoreIndependentFileName);
                                }

                            }
                            //
                        }
                        fobj.Close();
                    }
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
                File.WriteAllText(string.Format(@"{0}\{1}.{2}", BuildDir, Tag + BuildNumber, "txt"), logStr);
            }/**/
            logStr += logStr2;
            return logStr;
        }
    }
}
