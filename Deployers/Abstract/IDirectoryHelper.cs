using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toDev.Abstract
{
    //helps the directory deployer handle directory tasks
    //
    public interface IDirectoryHelper
    {
        bool Exists(string Path);
        object Create(string Path);
    }
}
