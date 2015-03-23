using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toDev.Abstract
{
    public interface IFileDeployHandler
    {
        IList<IFileObject> FileObjects { get;}
        void LoadFiles(IList<string> FileList);
        string Deploy(IList<IFileObject> fobjs);
        void DeleteDeploy(IList<IFileObject> fobjs);
        string Deploy();
        List<string> SourceDir { get; set; }
        string DeploymentDir { get; set; }
        string BuildDir { get; set; }
        IDirectoryHelper Dir { get; set; }
        string BuildNumber { get; set; }
        string UID { get; set; }
        void SetComment(string comment);
        string Tag { get; }
    }
}
