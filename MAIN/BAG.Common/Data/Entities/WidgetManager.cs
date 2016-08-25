using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAG.Common.Data.Entities
{
    public class WidgetManager : Base
    {


        public Widget PreRelease { get; set; }
        public Widget Release { get; set; }
        public Guid SitesettingId { get; set; }
        
        public bool IsDistinct
        {
            get
            {
                return Release == null || !Release.Updated.Equals(PreRelease.Updated) || Release.IsDistinct();
            }
        }

        public void Publish()
        {
            if (PreRelease != null)
            {
                Release = PreRelease;
            }
        }

        public void Reset()
        {
            if (Release != null)
            {
                PreRelease = Release;
            }
        }

        public void OnLoad(UnitOfWork unit)
        {
            if (PreRelease != null)
                PreRelease.OnLoad(unit);
            if (Release != null)
                Release.OnLoad(unit);
        }

        public string GetContent()
        {
            if (Release != null)
            {
                return Release.GetContent();
            }
            return "";
        }

        public string GetContentExceptTags()
        {
            return Release.GetContentExceptTags();
        }

        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);
            SitesettingId = map(SitesettingId, false);
            List<BaseEntity> bes = new List<BaseEntity>();
            bes.Add(Release);
            bes.Add(PreRelease);
            return bes;
        }
    }

}
