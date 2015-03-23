using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toDev.Abstract;

namespace toDev.Concrete
{
    public class FolderDirectoryHelper : IDirectoryHelper
    {

        #region IDirectory Helper
        public bool Exists(string Path)
        {
           return Directory.Exists(Path);
        }

        public object Create(string Path)
        {
            return Directory.CreateDirectory(Path);
        }
        #endregion
    }
}
