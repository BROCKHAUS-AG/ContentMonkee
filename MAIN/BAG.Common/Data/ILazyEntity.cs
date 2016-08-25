using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data
{
    public interface ILazyEntity
    {
        void OnLoad(UnitOfWork unit);
        void OnSave(UnitOfWork unit);
    }
}
