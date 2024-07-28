using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstract
{
    public interface IFolder
    {
        void CheckBasedOnCheckers();
        void CheckCreated();
        void CheckDeleted();
        void CheckUpdated();
    }
}
