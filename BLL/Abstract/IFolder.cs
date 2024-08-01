using BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstract
{
    public interface IFolder
    {
        /*void CheckBasedOnCheckers(string? rootPath = null);
        void CheckCreated(string rootPath);
        void CheckDeleted(string rootPath);
        void CheckUpdated(string rootPath);*/
        void CheckBasedOnCheckers(Folder folder = null);
        void CheckCreated(Folder folder);
        void CheckDeleted(Folder folder);
        void CheckUpdated(Folder folder);
    }
}
