using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class SiteManager : Base
    {
        public Site PreRelease { get; set; }
        public Site Release { get; set; }
        public Guid SiteSettingId { get; set; }


        public bool IsDistinct
        {
            get
            {
                return Release == null || !Release.Updated.Equals(PreRelease.Updated);
            }
        }

        public void Publish()
        {
            if (PreRelease != null)
            {
                Release = PreRelease;
            }

        }

        public void OnLoad(UnitOfWork unit)
        {
            if (PreRelease != null)
                PreRelease.OnLoad(unit);
            if (Release != null)
                Release.OnLoad(unit);
        }
        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            SiteSettingId = map(SiteSettingId, false);
            List<BaseEntity> bes = new List<BaseEntity>();
            bes.Add(Release);
            bes.Add(PreRelease);
            return bes;
        }
    }
}
