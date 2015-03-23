using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toDev.Abstract
{
    public interface IFileObject
    {
        IList<string> Directories { get;}
        string FullFileName { get; }
        string StoreIndependentFileName { get; }
        string FileName { get; }
        void Load(string fullfileName, string storeIndependentFileName);
        System.IO.Stream FileStream { get; }
        IFileObject Create(string str, string storeIndependentFileName);
        void Close();
    }
}
