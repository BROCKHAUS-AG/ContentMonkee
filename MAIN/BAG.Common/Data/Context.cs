using DevMentor.Context;
using BAG.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ChinhDo.Transactions;
using System.Transactions;

namespace BAG.Common.Data
{
    public partial class Context : DevMentor.Context.FileContext
#if ENABLE_EF
        , IObjectContextAdapter
#endif
    {
        // falls true, werden Daten in unterschiedlichen Dateien abgelegt. Nur für UnitTest interessant. Dieser Wert sollte ansonsten nicht geändert werden.
        public static bool DistributedData = true;

        IStoreStrategy baseStore;

        public Context()
            : base("name=ContentItemContext")
        {
        }

        public string prefixPath = _Globals.Instance.CurrentSiteSettingId.ToString() + "/";
        public override string PrefixPath
        {
            get
            {
                return DistributedData ? this.prefixPath : null;
            }
        }

        public Context(IStoreStrategy store) :
            base("name=ContentItemContext", store)
        {
            this.baseStore = store;
        }

        public FileSet<User> Users { get; set; }

        [FileSetUsePrefixPath]
        public FileSet<SiteManager> SiteManagers { get; set; }

        public FileSet<SiteSetting> SiteSettings { get; set; }
        public FileSet<Employee> Employees { get; set; }

        [FileSetUsePrefixPath]
        public FileSet<WidgetManager> WidgetManagers { get; set; }

        public System.Data.Entity.Core.Objects.ObjectContext ObjectContext
        {
            get
            {
                var objectContext = (this as IObjectContextAdapter);
                if (objectContext != null)
                    return (this as IObjectContextAdapter).ObjectContext;
                else
                    return null;
            }
        }

    }
}
