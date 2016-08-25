using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BAG.Common.Data.Entities
{
    public class WidgetComposite : Widget, ILazyEntity
    {
        public WidgetComposite()
            : base()
        {
            Widgets = new List<WidgetManager>();
            WidgetIds = new List<Guid>();
            Name = "Composite";
            Horizontal = true;
            WidgetDescription = "A Widget that combines other Widget";
        }
        public bool Horizontal { get; set; }
        public string ClassName { get; set; }
        public List<Guid> WidgetIds { get; set; }
        [NotMapped]
        [XmlIgnore]
        public List<WidgetManager> Widgets { get; set; }
        [NotMapped]
        [XmlIgnore]
        public Boolean CompositeLoaded { get; private set; }
        public override void OnLoad(UnitOfWork unit)
        {
            base.OnLoad(unit);
            if (CompositeLoaded)
            {
                return;
            }
            IEnumerable<WidgetManager> wms = WidgetIds.Where(g => g != this.Id).Select(wid => unit.WidgetManagerRepository.GetByID(wid)).Where(wm => wm != null);
            WidgetIds = wms.Select(wm => wm.Id).ToList();
            Widgets = wms.ToList();
            Widgets.ForEach(w => w.OnLoad(unit));
            CompositeLoaded = true;
        }
        public override string GetContent()
        {
            var content = "";
            if (Widgets.Count() != 0)
            {
                content = Widgets.Select(w => w.GetContent()).Aggregate((acc, s) => acc + " " + (s == null ? string.Empty : s));
            }

            return " " + content + " ";
        }
        public override List<BaseEntity> RemapAllGuids(Func<Guid, bool, Guid> map)
        {
            base.RemapAllGuids(map);

            WidgetIds = WidgetIds.Select(w => map(w, false)).ToList();
            return null;
        }

        public override bool IsDistinct()
        {
            if (Widgets == null || Widgets.Count <= 0)
            {
                return false;
            }
            return Widgets.Select(wm => wm.IsDistinct).Aggregate((s, n) => s || n);
        }
    }



}
