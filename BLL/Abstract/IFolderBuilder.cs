using BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BLL.Concrete.Folder;

namespace BLL.Abstract
{
    public interface IFolderBuilder
    {
        Folder Build();
        FolderBuilder CheckForCreate();
        FolderBuilder CheckForDelete();
        FolderBuilder CheckForUpdate();
        FolderBuilder SetReplicaRoot(string replicaRoot);
        FolderBuilder SetRoot(string root);
    }
}
